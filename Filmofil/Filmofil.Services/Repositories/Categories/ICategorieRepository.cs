using Filmofil.Domain.Entities;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Categories
{
    public interface ICategorieRepository : INeo4JRepository<Categorie>
    {
        Task UpdateCategorieAsync(Categorie categorie);
    }
}
