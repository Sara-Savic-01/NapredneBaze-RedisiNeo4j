using Filmofil.Domain.Entities;
using Filmofil.Domain.Interop;
using System.Threading.Tasks;

namespace Filmofil.Business.ChatManagement
{
    public interface IChatManager
    {
        Task<Result<LoadChatDataOutput>> LoadChatDataAndSubscribe(SubscribeInput input);

        Task<Result<long>> SendMessage(Message message);

        Task<Result> Subscribe(SubscribeInput input);
    }
}
