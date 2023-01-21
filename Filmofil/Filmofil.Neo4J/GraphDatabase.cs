using Neo4jClient;
using System;

namespace Filmofil.Neo4J
{
    public class GraphDatabase : IGraphDatabase
    {
        public IGraphClient GraphClient { get; }

        public GraphDatabase()
        {
            GraphClient = new GraphClient(
                new Uri("http://localhost:7474/db/data"), "neo4j", "edukacija");
            GraphClient.Connect();
        }
    }
}
