using Filmofil.Domain.Entities;
using Filmofil.Neo4J;
using System.Linq;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories.Categories
{
    public class CategorieRepository : Neo4JRepository<Categorie>, ICategorieRepository
    {
        public CategorieRepository(IGraphDatabase graphDatabase) : base(graphDatabase) { }

        protected override string NodeLabel => "Categorie";

        public override async Task<long> CreateAsync(Categorie entity)
        {
            await Graph.Cypher
                .Merge("(node:" + NodeLabel + " {" + GetCategorieData(entity) + "})")
                .ExecuteWithoutResultsAsync();

            return entity.Id;
        }

        private string GetCategorieData(Categorie entity)
        {
            return $"Id: {entity.Id}, Title: \"{entity.Title}\"";
        }

        public override async Task<Categorie> FindAsync(long id)
        {
            Categorie categorie = Graph.Cypher
                .Match("(categorie:Categorie)")
                .Where((Categorie categorie) => categorie.Id == id)
                .OptionalMatch("(categorie:Categorie)-[:HAS_USER]->(users:User)")
                .OptionalMatch("(categorie:Categorie)-[:HAS_POST]->(posts:Post)")
                .OptionalMatch("(categorie:Categorie)-[:HAS_MESSAGE]->(message:Message)")
                .Return((categorie, users, posts, message) =>
                    new
                    {
                        categorie.As<Categorie>().Id,
                        categorie.As<Categorie>().Title,
                        Users = users.CollectAsDistinct<User>(),
                        Posts = posts.CollectAsDistinct<Post>(),
                        Messages = message.CollectAsDistinct<Message>()
                    }
                )
                .ResultsAsync.Result.Select(myObject => new Categorie()
                {
                    Id = myObject.Id,
                    Title = myObject.Title,
                    Users = myObject.Users.ToList().ConvertAll(user => user.Id).ToList(),
                    Posts = myObject.Posts.ToList().ConvertAll(post => post.Id).ToList(),
                    Messages = myObject.Messages.ToList().ConvertAll(message => message.Id).ToList()
                })
                .FirstOrDefault();

            return categorie;
        }

        public async Task UpdateCategorieAsync(Categorie categorie)
        {
            await Graph.Cypher.Match($"(cat:{NodeLabel})")
                 .Where((Categorie cat) => cat.Id == categorie.Id)
                 .Set("cat= {entity}")
                 .WithParam("entity", categorie)
                 .ExecuteWithoutResultsAsync();
        }
    }
}
