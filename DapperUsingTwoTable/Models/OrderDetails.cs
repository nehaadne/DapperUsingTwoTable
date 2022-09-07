namespace DapperUsingTwoTable.Models
{
    public class OrderDetails
    {
        public int orderDetailsId { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public double totalAmount { get; set; }
        
    }
}
