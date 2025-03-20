using System.Text.Json.Serialization;

namespace StoreManager.Core.Entities
{
    public class Store
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required Guid CompanyId { get; set; }
        public ICollection<ProductStore>? ProductStore { get; set; }
    }
}
