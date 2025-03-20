using Microsoft.EntityFrameworkCore;
using StoreManager.Core.Entities;
using StoreManager.Core.Interfaces;
using StoreManager.Infrastructure.Data.Contexts;

namespace StoreManager.Infrastructure.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetByIdAsync(Guid storeId)
        {
            return await _context.Stores
                .SingleOrDefaultAsync(s => s.Id == storeId);
        }

        public async Task<IEnumerable<Store>> GetAllAsync()
        {
            return await _context.Stores
                .ToListAsync();
        }

        public async Task<IEnumerable<Store>> GetByCompanyAsync(Guid companyId)
        {
            return await _context.Stores
                .Where(s => s.CompanyId == companyId)
                .ToListAsync();
        }

        public void Add(Store store)
        {
            _context.Stores.Add(store);
        }

        public void Update(Store store)
        {
            _context.Stores.Update(store);
        }

        public void Remove(Store store)
        {
            _context.Stores.Remove(store);
        }
    }
}
