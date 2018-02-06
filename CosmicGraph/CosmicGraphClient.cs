using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CosmicGraph.Tester;
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
            builder.Append($"g.addV('{vertex.Label}')");

            if (!string.IsNullOrEmpty(vertex.Id))
            {
                builder.Append($".property('id', '{vertex.Id}')");
            }

            foreach (var vertexProperty in vertex.Properties)
            {
                var property = vertexProperty.Value[0];

                builder.Append($".property('{vertexProperty.Key.ToCamelCase()}', '{property.Value}')");
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

        private async Task<TResult> ExecuteBatchAsync<TResult>(GraphBatch batch)
        {
            var batchResult = default(TResult);

            foreach (var expression in batch.Expressions)
            {
                var query = documentClient.CreateGremlinQuery<object>(collection, expression.Value);

                while (query.HasMoreResults)
                {
                    foreach (var result in await query.ExecuteNextAsync<TResult>())
                    {
                        batchResult = result;
                    }
                }
            }

            return batchResult;
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

        public Task<TVertex> GetVertexAtPathAsync<TVertex>(string rootId, string propertyName, IEnumerable<string> path) where TVertex : class, IVertex
        {
            return Task.FromResult(default(TVertex));
        }

        public async Task<TVertex> GetVertexAtPathAsync<TVertex>(IVertex root, string propertyName, IEnumerable<string> path) where TVertex : class, IVertex
        {
            return await GetVertexAtPathAsync<TVertex>(root.Id, propertyName, path);
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
