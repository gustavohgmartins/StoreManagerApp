using System.Text.Json.Serialization;

namespace StoreManager.Core.Entities
{
    public class Product
    {
        public Guid? Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public ICollection<ProductStore>? ProductStore { get; set; }
    }
}
