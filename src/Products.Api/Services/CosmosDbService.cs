using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Products.Api.Models;

namespace Products.Api.Services
{
    public class CosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var query = "SELECT * FROM c";
            var queryIterator = _container.GetItemQueryIterator<Product>(query);
            var results = new List<Product>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            try
            {
                return await _container.ReadItemAsync<Product>(id, new PartitionKey(id));
            }
            catch (CosmosException)
            {
                return null;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var response = await _container.CreateItemAsync(product, new PartitionKey(product.Id));
            return response.Resource;
        }

        public async Task<Product> UpdateProductAsync(string id, Product product)
        {
            await _container.ReplaceItemAsync(product, id, new PartitionKey(id));
            return product;
        }

        public async Task DeleteProductAsync(string id)
        {
            await _container.DeleteItemAsync<Product>(id, new PartitionKey(id));
        }
    }
}