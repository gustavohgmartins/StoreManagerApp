using Microsoft.AspNetCore.Mvc;
using StoreManager.Application.Interfaces;
using StoreManager.Core.Entities;

namespace StoreManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(Guid id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                return Ok(store);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStores(Guid id)
        {
            var stores = await _storeService.GetAllStoresAsync();
            return Ok(stores);
        }

        [HttpGet("company/{companyId}")]
        public async Task<ActionResult> GetStoresByCompany(Guid companyId)
        {
            var stores = await _storeService.GetStoresByCompanyIdAsync(companyId);
            return Ok(stores);
        }

        [HttpPost]
        public async Task<ActionResult> CreateStore([FromBody] StoreDto store)
        {
            var createdStore = await _storeService.CreateStoreAsync(store);
            return CreatedAtAction(nameof(GetStoreById), new { id = createdStore.Id }, createdStore);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateStore([FromBody] Store store)
        {
            try
            {
                await _storeService.UpdateStoreAsync(store);

                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStore(Guid id)
        {
            try
            {
                await _storeService.DeleteStoreAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
