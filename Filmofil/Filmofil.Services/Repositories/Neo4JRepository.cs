using Neo4jClient;
using Filmofil.Domain.Entities;
using Filmofil.Neo4J;
using System.Threading.Tasks;

namespace Filmofil.Services.Repositories
{
    public abstract class Neo4JRepository<T> where T : IEntity
    {
        protected abstract string NodeLabel { get; }

        private readonly IGraphDatabase _graphDatabase;

        public Neo4JRepository(IGraphDatabase graphDatabase)
        {
            _graphDatabase = graphDatabase;
        }

        private IGraphClient _graph;
        protected IGraphClient Graph
        {
            get
            {
                if (_graph == null) _graph = _graphDatabase.GraphClient;
                return _graph;
            }
        }

        public abstract Task<long> CreateAsync(T entity);
        public abstract Task<T> FindAsync(long id);

        public async Task CreateRelationshipAsync(string label1, string label2, long node1Id, long node2Id, string nameOfRelationship)
        {
            await Graph.Cypher
                 .Match($"(node1:{label1})", $"(node2:{label2})")
                 .Where((User node1) => node1.Id == node1Id)
                 .AndWhere((User node2) => node2.Id == node2Id)
                 .CreateUnique($"(node1)-[:{nameOfRelationship}]->(node2)")
                 .ExecuteWithoutResultsAsync();
        }

        public async Task DeleteRelationshipAsync(string label1, string label2, long node1Id, long node2Id, string nameOfRelationship)
        {
            await Graph.Cypher
                 .OptionalMatch($"(node1:{label1})-[r:{nameOfRelationship}]->(node2:{label2})")
                 .Where((User node1) => node1.Id == node1Id)
                 .AndWhere((User node2) => node2.Id == node2Id)
                 .Delete("r")
                 .ExecuteWithoutResultsAsync();
        }

        public void Update(T entity)
        {
            long id = entity.Id;
            Graph.Cypher
                .Match("(node:" + NodeLabel + ")")
                .Where((T node) => node.Id == id)
                .Set("node = {entity}")
                .WithParam("entity", entity)
                .ExecuteWithoutResults();
        }

        public void Delete(long id)
        {
            Graph.Cypher
                .Match("(node: " + NodeLabel + ")")
                .Where((T node) => node.Id == id)
                .Delete("node")
                .ExecuteWithoutResults();
        }
    }
}
