using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmicGraph
{
    public interface ICosmicGraphClient
    {
        Task<TVertex> AddChildAsync<TVertex>(string edgeName, string parentId, TVertex child) where TVertex : class, IVertex;
        Task<TVertex> AddChildAsync<TVertex>(string edgeName, IVertex parent, TVertex child) where TVertex : class, IVertex;
        Task AddEdgeAsync(string edgeName, string sourceId, string targetId);
        Task AddEdgeAsync(string edgeName, IVertex source, string targetId);
        Task AddEdgeAsync(string edgeName, string sourceId, IVertex target);
        Task AddEdgeAsync(string edgeName, IVertex source, IVertex target);
        Task<TVertex> AddVertexAsync<TVertex>(TVertex vertex) where TVertex : class, IVertex;
        Task<TVertex> AddVertexIfNotExistsAsync<TVertex>(TVertex vertex) where TVertex : class, IVertex;
        Task ClearEntireGraphAsync();
        Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(string rootId, IEnumerable<string> edgePath) where TVertex : class, IVertex;
        Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(IVertex root, IEnumerable<string> edgePath) where TVertex : class, IVertex;
        Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(string rootId, IEnumerable<string> edgePath, string propertyName, IEnumerable<string> propertyPath) where TVertex : class, IVertex;
        Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(IVertex root, IEnumerable<string> edgePath, string propertyName, IEnumerable<string> propertyPath) where TVertex : class, IVertex;
        Task<TVertex> GetVertexAsync<TVertex>(string id) where TVertex : class, IVertex;
        Task<bool> HasVertexAsync(string id);
        Task<bool> HasVertexAsync(IVertex vertex);
    }
}