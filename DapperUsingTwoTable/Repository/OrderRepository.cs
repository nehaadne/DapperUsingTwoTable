using Dapper;
using DapperUsingTwoTable.Context;
using DapperUsingTwoTable.Models;
using DapperUsingTwoTable.Repository.Interface;

namespace DapperUsingTwoTable.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _context;
        public OrderRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderById(int id)
        {
            Order ord = new Order();

            var query = "Select * from TOrder  where orderId=@Id";
            using (var connection = _context.CreateConnection())
            {
                var raworder = await connection.QueryAsync<Order>(query, new { Id = id });
                ord = raworder.FirstOrDefault();
                if (ord != null)
                {
                    var orderdetailsrow = await connection.QueryAsync<OrderDetails>("select * from OrderDetails where orderid=@Id", new { Id = id });
                    ord.orderDetailsList = orderdetailsrow.ToList();
                }
                return ord;
            }
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            List<Order> odlist = new List<Order>();
            var query = "SELECT * FROM TOrder";
            using (var connection = _context.CreateConnection())
            {
                var raworders = await connection.QueryAsync<Order>(query);
                odlist = raworders.ToList();
                foreach (var order in odlist)
                {
                    var res = await connection.QueryAsync<OrderDetails>(@"select * from OrderDetails
                                                                        where orderid=@Id", new { Id = order.orderId });
                    order.orderDetailsList = res.ToList();
                }

                return odlist;
            }
        }

        public async Task<int> Insert(Order order)
        {
            double ret = 0;
            var query = @"Insert into TOrder(invoiceno,customername,mobileno,shippingaddress,
                           billingaddress,totalorderamount) values (@invoiceNo,@customerName,@mobileNo,
                           @shippingAddress,@billingAddress,@totalOrderAmount);SELECT CAST(SCOPE_IDENTITY() as int)";

            List<OrderDetails> odlist = new List<OrderDetails>();
            odlist = order.orderDetailsList.ToList();


            using (var connection = _context.CreateConnection())
            {

                {
                    int result = await connection.QuerySingleAsync<int>(query, order);
                    if (result != 0)
                    {
                        ret = await InsertUpdateOrder(odlist, result);
                        result = await connection.ExecuteAsync("update TOrder set totalOrderAmount=@totalOrderAmount where orderId=@result ", new { totalOrderAmount = ret, result= result });

                    }

                }


            }
            return Convert.ToInt32(ret);

        }

        public async Task<double> InsertUpdateOrder(List<OrderDetails> odlist, int result)
        {

            double grandtotal = 0;
            if (result != 0)
            {
                using (var connection = _context.CreateConnection())
                {
                    foreach (OrderDetails od in odlist)
                    {
                        od.orderId = result;
                        //var pquery = "select price from OrderDetails where orderId = @orderId";
                        var price = od.price;
                        od.totalAmount = price * od.quantity;
                        var qry = @"Insert into OrderDetails(orderId,
                                productid,quantity,totalamount,price ) values (@orderId,
                                @productId,@quantity,@totalAmount,@price) ";

                        var result1 = await connection.ExecuteAsync(qry, od);


                        grandtotal = grandtotal + od.totalAmount;
                    }
                }
            }
            return grandtotal;
        }

        public async Task<int> Update(Order order)
        {
            double ret;
            var query = @"Update TOrder set invoiceno=@invoiceNo,customername=@customerName,mobileno=@mobileNo,
                           shippingaddress=@shippingAddress,billingaddress=@billingAddress,
                            totalorderamount=@totalOrderAmount where orderId=@orderId ";
            using (var connection = _context.CreateConnection())
            {

                var result = await connection.ExecuteAsync(query, order);
                ret = await InsertUpdateOrder(order.orderDetailsList, order.orderId);
                order.totalOrderAmount = (int)ret;

                return result;

            }
        }

        public async Task<int> Delete(int id)
        {
            var query = @"Delete from TOrder where orderid=@Id
                          Delete from OrderDetails where orderid=@Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { Id = id });
                return result;
            }
        }

    }
}