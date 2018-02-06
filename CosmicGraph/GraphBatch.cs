using System.Collections.Generic;

namespace CosmicGraph.Tester
{
    public class GraphBatch
    {
        public Dictionary<string, string> Expressions { get; private set; }

        public GraphBatch(Dictionary<string, string> queries)
        {
            Expressions = queries;
        }
    }
}