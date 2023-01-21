using Filmofil.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Chat
{
    public interface IChatRepository : INeo4JRepository<Message>
    {
        Task<List<Message>> FindFrom(List<long> categories);
    }
}
