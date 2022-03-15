

using System.Text;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TestPubSubController : ControllerBase
    {
        private readonly ILogger<TestPubSubController> _logger;
        private readonly DaprClient _daprClient;
        public TestPubSubController(ILogger<TestPubSubController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }


        [Topic("pubsub", "test_topic")]
        [HttpPost("sub")]
        public async Task<ActionResult> Sub()
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            stream.ReadAsync(buffer, 0, buffer.Length);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation("testsub" + content);
            return Ok(content);
        }

        [HttpGet("pub")]
        public async Task<ActionResult> PubAsync()
        {
            var data = new WeatherForecast();
            await _daprClient.PublishEventAsync<WeatherForecast>("pubsub", "test_topic", data);
            return Ok("pub-ok");
        }
    }
}