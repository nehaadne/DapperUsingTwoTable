using DapperUsingTwoTable.Models;
using DapperUsingTwoTable.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DapperUsingTwoTable.Controllers
{
    [Route("Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        public OrderController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }


        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _orderRepo.GetOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/id")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepo.GetOrderById(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(Order od)
        {
            try
            {
                var order = await _orderRepo.Insert(od);
                return Ok(order);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder(Order od)
        {
            try
            {
                var order = await _orderRepo.Update(od);
                return Ok(order);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _orderRepo.Delete(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

    }

}
