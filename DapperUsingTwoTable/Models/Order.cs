namespace DapperUsingTwoTable.Models
{
    public class Order
    {
        public int orderId { get; set; }
        public int invoiceNo { get; set; }
        public string customerName { get; set; }
        public long mobileNo { get; set; }
        public string shippingAddress { get; set; }
        
        public string billingAddress { get; set; }
        public List<OrderDetails> orderDetailsList { get; set; }
        public double totalOrderAmount { get; set; }
    }
}
