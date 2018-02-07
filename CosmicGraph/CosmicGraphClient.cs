using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;

namespace CosmicGraph
{
    public class CosmicGraphClient : ICosmicGraphClient
    {
        private DocumentCollection collection;
        private DocumentClient documentClient;
        
        public CosmicGraphClient(DocumentClient documentClient, DocumentCollection collection)
        {
            this.documentClient = documentClient;
            this.collection = collection;
        }

        public async Task<TVertex> AddChildAsync<TVertex>(string edgeName, string parentId, TVertex child) where TVertex : class, IVertex
        {
            var result = await AddVertexAsync(child);

            await AddEdgeAsync(edgeName, parentId, result);

            return result;
        }

        public async Task<TVertex> AddChildAsync<TVertex>(string edgeName, IVertex parent, TVertex child) where TVertex : class, IVertex
        {
            return await AddChildAsync(edgeName, parent.Id, child);
        }

        public async Task<TVertex> AddChildIfNotExistsAsync<TVertex>(string edgeName, string parentId, TVertex child) where TVertex : class, IVertex
        {
            var builder = new StringBuilder();
            builder.Append($"g.V('{parentId}').outE('{edgeName}').inv().has('{nameof(child.Label).ToCamelCase()}', within('{child.Label.AddEscapeCharacters()}'))");

            if (await ExecuteSingleAsync<TVertex>(builder.ToString(), true) == null)
            {
                return await AddChildAsync(edgeName, parentId, child);
            }
            else
            {
                return await GetVertexAtEdgePathAsync<TVertex>(parentId, new string[] { edgeName }, nameof(child.Label), new string[] { child.Label.AddEscapeCharacters() });
            }
        }

        public async Task<TVertex> AddChildIfNotExistsAsync<TVertex>(string edgeName, IVertex parent, TVertex child) where TVertex : class, IVertex
        {
            return await AddChildIfNotExistsAsync(edgeName, parent.Id, child);
        }

        public async Task AddEdgeAsync(string edgeName, string sourceId, string targetId)
        {
            var builder = new StringBuilder();
            builder.Append($"g.V('{sourceId}').addE('{edgeName}').to(g.V('{targetId}'))");

            var result = await ExecuteSingleAsync<object>(builder.ToString());
        }

        public async Task AddEdgeAsync(string edgeName, IVertex source, string targetId)
        {
            await AddEdgeAsync(edgeName, source.Id, targetId);
        }

        public async Task AddEdgeAsync(string edgeName, string sourceId, IVertex target)
        {
            await AddEdgeAsync(edgeName, sourceId, target.Id);
        }

        public async Task AddEdgeAsync(string edgeName, IVertex source, IVertex target)
        {
            await AddEdgeAsync(edgeName, source.Id, target.Id);
        }

        public async Task<TVertex> AddVertexAsync<TVertex>(TVertex vertex) where TVertex : class, IVertex
        {
            var builder = new StringBuilder();
            builder.Append($"g.addV('{vertex.Label.AddEscapeCharacters()}')");

            if (!string.IsNullOrEmpty(vertex.Id))
            {
                builder.Append($".property('id', '{vertex.Id}')");
            }

            if (vertex.Properties != null)
            {
                foreach (var vertexProperty in vertex.Properties)
                {
                    var property = vertexProperty.Value[0];

                    var key = vertexProperty.Key.ToCamelCase();

                    builder.Append($".property('{key}', '{property.Value.AddEscapeCharacters()}')");
                }
            }

            return await ExecuteSingleAsync<TVertex>(builder.ToString());
        }

        public async Task<TVertex> AddVertexIfNotExistsAsync<TVertex>(TVertex vertex) where TVertex : class, IVertex
        {
            if (await HasVertexAsync(vertex.Id))
            {
                return await GetVertexAsync<TVertex>(vertex.Id);
            }

            return await AddVertexAsync(vertex);
        }

        public async Task ClearEntireGraphAsync()
        {
            await ExecuteSingleAsync<object>("g.V().drop()", true);
        }

        private async Task<TResult> ExecuteSingleAsync<TResult>(string expression, bool returnDefaultIfNoResult = false)
        {
            var query = documentClient.CreateGremlinQuery<object>(collection, expression);

            while (query.HasMoreResults)
            {
                foreach (var result in await query.ExecuteNextAsync<TResult>())
                {
                    return result;
                }
            }

            if (returnDefaultIfNoResult)
            {
                return default(TResult);
            }

            throw new ArgumentNullException("No result found.");
        }

        public async Task<TVertex> GetVertexAsync<TVertex>(string id) where TVertex : class, IVertex
        {
            return await ExecuteSingleAsync<TVertex>($"g.V('{id}')");
        }

        public async Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(string rootId, IEnumerable<string> edgePath) where TVertex : class, IVertex
        {
            var builder = new StringBuilder();
            builder.Append($"g.V('{rootId}')");

            foreach (var edge in edgePath)
            {
                builder.Append($".outE('{edge}').inv()");
            }

            return await ExecuteSingleAsync<TVertex>(builder.ToString());
        }

        public async Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(IVertex root, IEnumerable<string> edgePath) where TVertex : class, IVertex
        {
            return await GetVertexAtEdgePathAsync<TVertex>(root.Id, edgePath);
        }

        public async Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(string rootId, IEnumerable<string> edgePath, string propertyName, IEnumerable<string> propertyPath) where TVertex : class, IVertex
        {
            var builder = new StringBuilder();
            builder.Append($"g.V('{rootId}')");

            var edgeEnumerator = edgePath.GetEnumerator();

            foreach (var property in propertyPath)
            {
                if (!edgeEnumerator.MoveNext())
                {
                    edgeEnumerator.Reset();
                    edgeEnumerator.MoveNext();
                }

                builder.Append($".outE('{edgeEnumerator.Current}').inv().has('{propertyName.ToCamelCase()}', within('{property.AddEscapeCharacters()}'))");
            }

            return await ExecuteSingleAsync<TVertex>(builder.ToString());
        }

        public async Task<TVertex> GetVertexAtEdgePathAsync<TVertex>(IVertex root, IEnumerable<string> edgePath, string propertyName, IEnumerable<string> propertyPath) where TVertex : class, IVertex
        {
            return await GetVertexAtEdgePathAsync<TVertex>(root.Id, edgePath, propertyName, propertyPath);
        }

        public async Task<bool> HasVertexAsync(string id)
        {
            var vertex = await ExecuteSingleAsync<object>($"g.V().has('id','{id}')", true);

            return vertex != null;
        }

        public async Task<bool> HasVertexAsync(IVertex vertex)
        {
            return await HasVertexAsync(vertex.Id);
        }
    }
}
