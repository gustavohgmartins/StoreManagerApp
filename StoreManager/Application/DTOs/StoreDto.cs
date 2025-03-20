namespace StoreManager.Core.Entities
{
    public class StoreDto
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required Guid CompanyId { get; set; }
        public ICollection<ProductStore>? ProductStore { get; set; }
    }
}
