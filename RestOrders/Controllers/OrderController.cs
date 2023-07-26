using Microsoft.AspNetCore.Mvc;
using RestOrdersLib.Producers;
using RestOrdersLib.Models;
using RestOrdersLib.Repositories;
using RestOrdersLib.Services;

namespace RestOrders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IOrderAggregateRepository orderAggregateRepository;
        private readonly IOrderService orderService;
        private readonly IRabbitMQProducer rabbitProducer;

        public OrderController(IOrderAggregateRepository orderAggregateRepository, 
            IOrderService orderService,
            IRabbitMQProducer rabbitProducer)
        {
            this.orderAggregateRepository = orderAggregateRepository;
            this.orderService = orderService;
            this.rabbitProducer = rabbitProducer;
        }

        // POST api/<OrderController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public IActionResult Post([FromBody] OrderRequest order)
        {
            if (!order.EventType.Equals("Add") && !order.EventType.Equals("Remove"))
            {
                ModelState.AddModelError(nameof(order), "EventType should be Add or Remove.");
                return ValidationProblem();
            }

            if (ModelState.IsValid)
            {
                orderService.AddEvent(order);
                rabbitProducer.Send(order);

                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // GET api/<OrderController>
        [HttpGet]
        public IActionResult Get(int orderId)
        {
            var result = orderAggregateRepository.GetOrderAggregate(orderId);

            return Ok(result);
        }
    }
}
