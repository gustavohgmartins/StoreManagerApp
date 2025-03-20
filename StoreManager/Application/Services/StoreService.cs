using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Interfaces;
using StoreManager.Core.Entities;
using StoreManager.Core.Interfaces;
using StoreManager.Infrastructure.Data.Contexts;

namespace StoreManager.Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly ApplicationDbContext _context;

        public StoreService(IStoreRepository storeRepository, ApplicationDbContext context)
        {
            _storeRepository = storeRepository;
            _context = context;
        }

        public async Task<Store> GetStoreByIdAsync(Guid storeId)
        {
            if (storeId == Guid.Empty)
            {
                throw new ArgumentException("The store ID provided is invalid.", nameof(storeId));
            }

            var store = await _storeRepository.GetByIdAsync(storeId);

            if (store is null)
            {
                throw new ApplicationException("Store not found.");
            }

            return store;
        }

        public async Task<IEnumerable<Store>> GetStoresByCompanyIdAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("The company ID provided is invalid.", nameof(companyId));
            }

            return await _storeRepository.GetByCompanyAsync(companyId);
        }

        public async Task<IEnumerable<Store>> GetAllStoresAsync()
        {
            return await _storeRepository.GetAllAsync();
        }

        public async Task<Store> CreateStoreAsync(StoreDto store)
        {
            if (store is null)
            {
                throw new ArgumentException("The store provided is invalid.", nameof(store));
            }

            var newStore = new Store
            {
                Name = store.Name,
                Location = store.Location,
                CompanyId = store.CompanyId,
                ProductStore = store.ProductStore,
            };

            _storeRepository.Add(newStore);

            await _context.SaveChangesAsync();

            return newStore;
        }

        public async Task UpdateStoreAsync(Store store)
        {
            if (store is null)
            {
                throw new ArgumentException("The store provided is invalid.", nameof(store));
            }

            var existingStore = await _storeRepository.GetByIdAsync(store.Id);

            if (existingStore is null)
            {
                throw new ApplicationException("Store not found.");
            }

            existingStore.Name = store.Name;
            existingStore.Location = store.Location;
            existingStore.CompanyId = store.CompanyId;
            existingStore.ProductStore = store.ProductStore;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteStoreAsync(Guid storeId)
        {
            if (storeId == Guid.Empty)
            {
                throw new ArgumentException("The store ID provided is invalid.", nameof(storeId));
            }

            var existingStore = await _storeRepository.GetByIdAsync(storeId);

            if (existingStore is null)
            {
                throw new ApplicationException("Store not found.");
            }

            _storeRepository.Remove(existingStore);

            await _context.SaveChangesAsync();
        }
    }
}
