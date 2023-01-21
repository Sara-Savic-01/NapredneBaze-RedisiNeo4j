using Microsoft.AspNetCore.SignalR;
using Filmofil.Domain.Entities;
using Filmofil.Domain.Entities.Enums;
using Filmofil.Domain.Interop;
using Filmofil.Services.Hubs;
using Filmofil.Services.Repositories;
using Filmofil.Services.Repositories.Chat;
using Filmofil.Services.Repositories.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filmofil.Business.ChatManagement
{
    public class ChatManager : IChatManager
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<MessageHub> _hubContext;

        public ChatManager(
            IRedisRepository redisRepository,
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IHubContext<MessageHub> hubContext)
        {
            _redisRepository = redisRepository;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Result<LoadChatDataOutput>> LoadChatDataAndSubscribe(SubscribeInput input)
        {
            if (input == null) return new Result<LoadChatDataOutput>() { Success = false };

            input.Categories.ForEach(async categorie =>
            {
                await _hubContext.Groups.AddToGroupAsync(
                    input.ConnectionId, categorie.ToString()).ConfigureAwait(false);
            });


            List<Message> messages = await _chatRepository.
                FindFrom(input.Categories).ConfigureAwait(false);

            List<User> users = await _userRepository.FindManyUsersAsync(
                messages.ConvertAll(message => message.Sender)).ConfigureAwait(false);

            return new Result<LoadChatDataOutput>()
            {
                Success = true,
                Data = new LoadChatDataOutput()
                {
                    Messages = messages,
                    Users = users
                }
            };
        }

        public async Task<Result<long>> SendMessage(Message message)
        {
            long id = await _chatRepository.CreateAsync(message).ConfigureAwait(false);

            await _hubContext.Clients
                .Group(message.Categorie.ToString())
                .SendAsync("SendMessage", message)
                .ConfigureAwait(false);

            List<Categorie> categories = await _redisRepository
                .GetAsync<List<Categorie>>(RedisKeys.CATEGORIES)
                .ConfigureAwait(false);

            Categorie categorie = categories
                .Find(categorie => categorie.Id == message.Categorie);

            categorie.Messages.Add(id);

            await _redisRepository.SetAsync(RedisKeys.CATEGORIES, categories)
                .ConfigureAwait(false);

            if (id == 0) return new Result<long>() { Success = false };

            return new Result<long>()
            {
                Success = true,
                Data = id
            };
        }

        public async Task<Result> Subscribe(SubscribeInput input)
        {
            if (input == null) return new Result() { Success = false };

            input.Categories.ForEach(async categorie =>
            {
                await _redisRepository.Subscribe(categorie.ToString(), input.ConnectionId)
                    .ConfigureAwait(false);

            });

            return new Result() { Success = true };
        }
    }
}
