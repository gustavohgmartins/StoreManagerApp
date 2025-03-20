using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreManager.Core.Entities;

namespace StoreManager.Application.Interfaces
{
    public interface IStoreService
    {
        Task<Store> GetStoreByIdAsync(Guid storeId);
        Task<IEnumerable<Store>> GetStoresByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<Store>> GetAllStoresAsync();
        Task<Store> CreateStoreAsync(StoreDto store);
        Task UpdateStoreAsync(Store store);
        Task DeleteStoreAsync(Guid storeId);
    }
}
