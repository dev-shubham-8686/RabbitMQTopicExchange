using Common.Models;
using Common.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly RedisStateStore _state;

        private readonly RabbitSender _rabbitSender;

        public OrdersController(RedisStateStore state, RabbitSender rabbitSender)
        {
            _state = state;
            _rabbitSender = rabbitSender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {

            // Generate new id (simplified demo)
            var id = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % 1_000_000);
         
            request.Id = id;

            await _state.SetAsync($"person:{id}", request);

            _rabbitSender.PublishMessage<CreateOrderRequest>(request, "order.cookwaffle");

            return Ok(new { id });

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var order = await _state.GetAsync<Order>($"person:{id}");

            return Ok(order);
        }

        
    }

}
