using CosmicGraph.Tester;
using System.Collections.Generic;
using System.Text;

namespace CosmicGraph
{
    public class GraphBatchBuilder
    {
        private Dictionary<string, string> expressions;

        public GraphBatchBuilder()
        {
            expressions = new Dictionary<string, string>();
        }

        public GraphBatch Build()
        {
            return new GraphBatch(expressions);
        }

        public GraphBatchBuilder AddVertex(IVertex vertex)
        {
            var builder = new StringBuilder();
            builder.Append($"g.addV('{vertex.Label}')");

            var type = vertex.GetType().GetProperties();
            foreach (var property in type)
            {
                if (property.Name == "Label")
                {
                    continue;
                }

                builder.Append($".property('{property.Name}', '{property.GetValue(vertex)}')");
            }

            EnqueueExpression(builder.ToString());
            return this;
        }

        private void EnqueueExpression(string expression)
        {
            expressions.Add(expressions.Count.ToString(), expression);
        }
    }
}
