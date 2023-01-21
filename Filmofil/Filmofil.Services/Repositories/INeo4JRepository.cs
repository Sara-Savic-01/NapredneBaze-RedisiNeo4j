using Filmofil.Domain.Entities;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories
{
    public interface INeo4JRepository<T> where T : IEntity
    {
        Task<long> CreateAsync(T entity);
        Task<T> FindAsync(long id);
        void Update(T entity);
        void Delete(long id);
        Task CreateRelationshipAsync(string label1, string label2, long node1Id, long node2Id, string nameOfRelationship);
        Task DeleteRelationshipAsync(string label1, string label2, long node1Id, long node2Id, string nameOfRelationship);
    }
}
