using Filmofil.Domain.Entities;
using Filmofil.Neo4J;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Chat
{
    public class ChatRepository : Neo4JRepository<Message>, IChatRepository
    {
        protected override string NodeLabel => "Message";

        public ChatRepository(IGraphDatabase graphDatabase) : base(graphDatabase) { }

        public async override Task<long> CreateAsync(Message message)
        {
            byte[] gb = Guid.NewGuid().ToByteArray();
            message.Id = BitConverter.ToInt32(gb, 0);

            await Graph.Cypher
                .Merge($"(author:User {{Id: {message.Sender}}})")
                .With("author")
                .Merge($"(message:Message {{ Id: \"{message.Id}\", Content: \"{message.Content}\"}})")
                .With("author, message")
                .Merge($"(categorie:Categorie {{Id: {message.Categorie}}})")
                .Merge("(author)<-[:MESSAGE_AUTHOR]-(message)<-[:HAS_MESSAGE]-(categorie)")
                .ExecuteWithoutResultsAsync();

            return message.Id;
        }

        public override Task<Message> FindAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<Message>> FindFrom(List<long> categories)
        {
            return (await Graph.Cypher
                .Match("(user:User)-[author:MESSAGE_AUTHOR]-(message:Message)" +
                    "-[:HAS_MESSAGE]-(categorie:Categorie)")
                .Where("(categorie.Id in { categories })")
                .WithParam("categories", categories)
                .Return((message, categorie, user) => new Message()
                {
                    Id = message.As<Message>().Id,
                    Categorie = categorie.As<Categorie>().Id,
                    Content = message.As<Message>().Content,
                    Sender = user.As<User>().Id
                })
                .ResultsAsync).ToList();
        }
    }
}
