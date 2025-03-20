using Moq;
using StoreManager.Application.Interfaces;
using StoreManager.Application.Services;
using StoreManager.Core.Entities;
using StoreManager.Core.Interfaces;
using StoreManager.Infrastructure.Data.Contexts;

namespace StoreManagement.Application.Tests
{
    public class StoreServiceTests
    {
        private readonly Mock<IStoreRepository> _storeRepositoryMock;
        private readonly Mock<ApplicationDbContext> _dbContextMock;
        private readonly IStoreService _storeService;

        public StoreServiceTests()
        {
            _storeRepositoryMock = new Mock<IStoreRepository>();
            _dbContextMock = new Mock<ApplicationDbContext>();
            _storeService = new StoreService(_storeRepositoryMock.Object, _dbContextMock.Object);
        }

        [Fact]
        public async Task GetStoreByIdAsync_ValidId_ReturnsStore()
        {
            // Arrange
            var storeId = Guid.NewGuid();

            var store = new Store
            {
                Id = storeId,
                Location = "New York",
                Name = "Test Store",
                CompanyId = Guid.NewGuid()
            };

            _storeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(storeId))
                .ReturnsAsync(store);

            // Act
            var result = await _storeService.GetStoreByIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.Id);
            Assert.Equal("Test Store", result.Name);

            _storeRepositoryMock.Verify(repo => repo.GetByIdAsync(storeId), Times.Once);
        }

        [Fact]
        public async Task GetStoreByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var invalidStoreId = Guid.Empty;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _storeService.GetStoreByIdAsync(invalidStoreId)
            );
        }

        [Fact]
        public async Task GetStoreByIdAsync_StoreNotFound_ThrowsApplicationException()
        {
            // Arrange
            var invalidStoreId = Guid.NewGuid();

            _storeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(invalidStoreId))
                .ReturnsAsync((Store)null);

            // Act and Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _storeService.GetStoreByIdAsync(invalidStoreId));

            _storeRepositoryMock.Verify(repo => repo.GetByIdAsync(invalidStoreId), Times.Once);
        }

        [Fact]
        public async Task GetStoresByCompanyId_ValidCompanyId_ReturnsStores()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var stores = new List<Store>
            {
                new Store
                {
                    Name = "Store A",
                    Location = "New York",
                    CompanyId = companyId
                },
                new Store
                {
                    Name = "Store B",
                    Location = "New York",
                    CompanyId = companyId
                }
            };

            _storeRepositoryMock
                .Setup(repo => repo.GetByCompanyAsync(companyId))
                .ReturnsAsync(stores);

            // Act
            var result = await _storeService.GetStoresByCompanyIdAsync(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, store => Assert.Equal(companyId, store.CompanyId));

            _storeRepositoryMock.Verify(repo => repo.GetByCompanyAsync(companyId), Times.Once);
        }

        [Fact]
        public async Task GetStoresByCompanyId_InvalidId_ThrowsException()
        {
            // Arrange
            var invalidCompanyId = Guid.Empty;

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _storeService.GetStoresByCompanyIdAsync(invalidCompanyId)
            );
        }

        [Fact]
        public async Task GetStoresByCompanyId_NoStores_ReturnsEmptyList()
        {
            // Arrange
            var companyId = Guid.NewGuid();

            _storeRepositoryMock
                .Setup(repo => repo.GetByCompanyAsync(companyId))
                .ReturnsAsync(Enumerable.Empty<Store>());

            // Act
            var result = await _storeService.GetStoresByCompanyIdAsync(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _storeRepositoryMock.Verify(repo => repo.GetByCompanyAsync(companyId), Times.Once);
        }

        [Fact]
        public async Task GetAllStores_ShouldReturnAllStores()
        {
            // Arrange
            var stores = new List<Store>
            {
                new Store
                {
                    Location = "New York",
                    Name = "Store A",
                    CompanyId = Guid.NewGuid()
                },
                new Store
                {
                    Location = "New York",
                    Name = "Store B",
                    CompanyId = Guid.NewGuid()
                },
            };

            _storeRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(stores);

            // Act
            var result = await _storeService.GetAllStoresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(stores.Count, result.Count());
            Assert.Equal(stores, result);

            _storeRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }


        [Fact]
        public async Task CreateAsync_ValidStore_CreatesStore()
        {
            // Arrange
            var newStoreDto = new StoreDto
            {
                Name = "New Store",
                Location = "New York",
                CompanyId = Guid.NewGuid()
            };

            // Act
            var createdStore = await _storeService.CreateStoreAsync(newStoreDto);

            // Assert
            Assert.IsType<Store>(createdStore);
            _storeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Store>()), Times.Once);
            _dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_InvalidStore_ThrowsArgumentException()
        {
            // Arrange
            StoreDto store = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _storeService.CreateStoreAsync(store));
            _storeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Store>()), Times.Never);
            _dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact]
        public async Task UpdateStoreAsync_ValidStore_UpdatesStore()
        {
            // Arrange
            var store = new Store
            {
                Name = "Updated Store",
                Location = "New York",
                CompanyId = Guid.NewGuid()
            };

            _storeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(store.Id))
                .ReturnsAsync(store);

            // Act
            await _storeService.UpdateStoreAsync(store);

            // Assert
            _storeRepositoryMock.Verify(repo => repo.GetByIdAsync(store.Id), Times.Once);
            _dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStoreAsync_InvalidStore_ThrowsArgumentException()
        {
            // Arrange
            Store store = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _storeService.UpdateStoreAsync(store));

            _storeRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _storeRepositoryMock.Verify(repo => repo.Update(store), Times.Never);
            _dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStoreAsync_StoreNotFound_ThrowsApplicationException()
        {
            // Arrange
            var store = new Store
            {
                Id = Guid.NewGuid(),
                Location = "New York",
                Name = "Nonexistent Store",
                CompanyId = Guid.NewGuid()
            };

            _storeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(store.Id))
                .ReturnsAsync((Store)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _storeService.UpdateStoreAsync(store));

            _storeRepositoryMock.Verify(repo => repo.GetByIdAsync(store.Id), Times.Once);
            _storeRepositoryMock.Verify(repo => repo.Update(It.IsAny<Store>()), Times.Never);
            _dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteStoreAsync_ValidId_DeletesStore()
        {
            // Arrange
            var store = new Store
            {
                Id = Guid.NewGuid(),
                Location = "New York",
                Name = "Store A",
                CompanyId = Guid.NewGuid()
            };

            _storeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(store.Id))
                .ReturnsAsync(store);

            // Act
            await _storeService.DeleteStoreAsync(store.Id);

            // Assert
            _storeRepositoryMock.Verify(repo => repo.GetByIdAsync(store.Id), Times.Once);
            _storeRepositoryMock.Verify(repo => repo.Remove(store), Times.Once);
            _dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteStoreAsync_StoreNotFound_ThrowsApplicationException()
        {
            // Arrange
            var store = new Store
            {
                Id = Guid.NewGuid(),
                Location = "New York",
                Name = "Store A",
                CompanyId = Guid.NewGuid()
            };

            _storeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(store.Id))
                .ReturnsAsync((Store)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _storeService.DeleteStoreAsync(store.Id));

            _storeRepositoryMock.Verify(repo => repo.GetByIdAsync(store.Id), Times.Once);
            _storeRepositoryMock.Verify(repo => repo.Remove(It.IsAny<Store>()), Times.Never);
            _dbContextMock.Verify(d => d.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
