using Dapr.Actors;
using Dapr.Actors.Client;
using FrontEnd.ActorDefine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {

        private readonly IActorProxyFactory _actorProxyFactory;

        public ActorController(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory=actorProxyFactory;
        }

        [HttpGet("paid/{orderId}")]
        public async Task<IActionResult> PaidAsync(string orderId)
        {
            var actorId = new ActorId("myid-"+orderId);
            var proxy = ActorProxy.Create<IOrderStatusActor>(actorId, "OrderStatusActor");

            var result = await proxy.Paid(orderId);

            return Ok(result);
        }

        [HttpGet("get/{orderId}")]
        public async Task<IActionResult> GetAsync(string orderId)
        {
            var proxy = _actorProxyFactory.CreateActorProxy<IOrderStatusActor>(
                new ActorId("myid-"+orderId),
                "OrderStatusActor");

            return Ok(await proxy.GetStatus(orderId));
        }

        [HttpGet("stoptimer/{orderId}")]
        public async Task<IActionResult> StopTimer(string orderId)
        {
            var proxy = _actorProxyFactory.CreateActorProxy<IOrderStatusActor>(
               new ActorId("myid-"+orderId),
               "OrderStatusActor");

            await proxy.StopTimerAsync("test-timer");

            return Ok();
        }
    }
}
