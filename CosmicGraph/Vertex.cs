using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CosmicGraph
{
    public class Vertex : IVertex
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public Dictionary<string, VertexProp[]> Properties { get; set; }

        protected string Get([CallerMemberName] string property = null)
        {
            return Properties[property.ToCamelCase()][0].Value;
        }

        protected void Set(string value, [CallerMemberName] string property = null)
        {
            if (Properties == null)
            {
                Properties = new Dictionary<string, VertexProp[]>();
            }

            var propertyCamel = property.ToCamelCase();

            if (!Properties.ContainsKey(propertyCamel))
            {
                Properties[propertyCamel] = new VertexProp[1] { new VertexProp() };
            }

            Properties[propertyCamel][0].Value = value;
        }
    }
}