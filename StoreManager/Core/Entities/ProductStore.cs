namespace StoreManager.Core.Entities
{
    public class ProductStore
    {
        public required Guid ProductId { get; set; }
        public required Guid StoreId { get; set; }
        public required int StockQuantity { get; set; }
        public required decimal Price { get; set; }
        public Store Store { get; set; }
        public Product Product { get; set; }
    }
}
