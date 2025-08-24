using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Products.Api.Controllers;
using Products.Api.Models;
using Products.Api.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Products.Api.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<ICosmosDbService> _mockService;
        private readonly Mock<ILogger<ProductController>> _mockLogger;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockService = new Mock<ICosmosDbService>();
            _mockLogger = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = "1", Name = "Product1", Description = "Description1", Price = 10.0, Category = "Category1" },
                new Product { Id = "2", Name = "Product2", Description = "Description2", Price = 20.0, Category = "Category2" }
            };
            _mockService.Setup(service => service.GetProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count());
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            string productId = "non-existent-id";
            _mockService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtActionResult_WithProduct()
        {
            // Arrange
            var newProduct = new Product { Id = "3", Name = "Product3", Description = "Description3", Price = 30.0, Category = "Category3" };
            _mockService.Setup(service => service.CreateProductAsync(newProduct)).ReturnsAsync(newProduct);

            // Act
            var result = await _controller.CreateProduct(newProduct);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal(newProduct.Id, returnProduct.Id);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var updatedProduct = new Product { Id = "non-existent-id", Name = "UpdatedProduct", Description = "UpdatedDescription", Price = 15.0, Category = "UpdatedCategory" };
            _mockService.Setup(service => service.UpdateProductAsync(updatedProduct.Id, updatedProduct)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateProduct(updatedProduct.Id, updatedProduct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            string productId = "non-existent-id";
            _mockService.Setup(service => service.DeleteProductAsync(productId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}