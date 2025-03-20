using Microsoft.AspNetCore.Mvc;
using Moq;
using StoreManager.Application.Interfaces;
using StoreManager.Controllers;
using StoreManager.Core.Entities;

namespace StoreManagement.Application.Tests
{
    public class StoresControllerTests
    {
        private readonly Mock<IStoreService> _storeServiceMock;
        private readonly StoreController _controller;

        public StoresControllerTests()
        {
            _storeServiceMock = new Mock<IStoreService>();
            _controller = new StoreController(_storeServiceMock.Object);
        }

        [Fact]
        public async Task GetStore_ExistingId_ReturnsOk()
        {
            // Arrange
            var store = new Store { Name = "Test Store", Location = "New York", CompanyId = Guid.NewGuid() };

            _storeServiceMock.Setup(s => s.GetStoreByIdAsync(store.Id))
                             .ReturnsAsync(store);

            // Act
            var result = await _controller.GetStoreById(store.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStore = Assert.IsType<Store>(okResult.Value);
            Assert.Equal(store.Id, returnedStore.Id);
        }

        [Fact]
        public async Task GetStore_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var message = "Store not found";
            _storeServiceMock.Setup(s => s.GetStoreByIdAsync(storeId))
                             .ThrowsAsync(new ApplicationException(message));

            // Act
            var result = await _controller.GetStoreById(storeId);

            // Assert
            var responseResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnedValue = Assert.IsType<string>(responseResult.Value);
            Assert.Equal(message, returnedValue);
        }

        [Fact]
        public async Task CreateStore_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var storeDto = new StoreDto
            {
                Name = "New Store",
                Location = "New York",
                CompanyId = Guid.NewGuid()
            };

            var createdStore = new Store
            {
                Name = storeDto.Name,
                Location = storeDto.Location,
                CompanyId = storeDto.CompanyId
            };

            _storeServiceMock.Setup(s => s.CreateStoreAsync(It.IsAny<StoreDto>()))
                             .ReturnsAsync(createdStore);

            // Act
            var result = await _controller.CreateStore(storeDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedStore = Assert.IsType<Store>(createdResult.Value);
            Assert.Equal(createdStore, returnedStore);
        }

        [Fact]
        public async Task UpdateStore_ValidId_ReturnsNoContent()
        {
            // Arrange
            var store = new Store
            {
                Name = "New Store",
                Location = "New York",
                CompanyId = Guid.NewGuid()
            };

            _storeServiceMock.Setup(s => s.UpdateStoreAsync(store))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateStore(store);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateStore_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var store = new Store
            {
                Name = "New Store",
                Location = "New York",
                CompanyId = Guid.NewGuid()
            };

            var message = "Store not found";
            _storeServiceMock.Setup(s => s.UpdateStoreAsync(store))
                             .ThrowsAsync(new ApplicationException(message));

            // Act
            var result = await _controller.UpdateStore(store);

            // Assert
            var responseResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnedValue = Assert.IsType<string>(responseResult.Value);
            Assert.Equal(message, returnedValue);
        }

        [Fact]
        public async Task DeleteStore_NonExistingId_ReturnsNoContent()
        {
            // Arrange
            var storeId = Guid.NewGuid();

            var message = "Store not found";
            _storeServiceMock.Setup(s => s.DeleteStoreAsync(storeId))
                             .ThrowsAsync(new ApplicationException(message));

            // Act
            var result = await _controller.DeleteStore(storeId);

            // Assert
            var responseResult = Assert.IsType<NotFoundObjectResult>(result);
            var returnedValue = Assert.IsType<string>(responseResult.Value);
            Assert.Equal(message, returnedValue);
        }

        [Fact]
        public async Task DeleteStore_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var storeId = Guid.NewGuid();

            _storeServiceMock.Setup(s => s.DeleteStoreAsync(storeId))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteStore(storeId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
