using Neo4jClient;

namespace Filmofil.Neo4J
{
    public interface IGraphDatabase
    {
        IGraphClient GraphClient { get; }
    }
}
