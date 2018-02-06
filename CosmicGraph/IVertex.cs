using System.Collections.Generic;

namespace CosmicGraph
{
    public interface IVertex
    {
        string Id { get; set; }
        string Label { get; set; }
        string Type { get; set; }
        Dictionary<string, VertexProp[]> Properties { get; set; }
    }
}