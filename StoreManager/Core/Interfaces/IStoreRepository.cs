
using StoreManager.Core.Entities;

namespace StoreManager.Core.Interfaces
{
    public interface IStoreRepository
    {
        Task<Store?> GetByIdAsync(Guid id);
        Task<IEnumerable<Store>> GetAllAsync();
        Task<IEnumerable<Store>> GetByCompanyAsync(Guid companyId);
        void Add(Store store);
        void Update(Store store);
        void Remove(Store store);
    }
}
