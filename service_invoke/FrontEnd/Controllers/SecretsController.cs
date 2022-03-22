using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecretsController : ControllerBase
    {
        private readonly ILogger<SecretsController> logger;

        private readonly DaprClient daprClient;

        private readonly IConfiguration _configuration;

        public SecretsController(ILogger<SecretsController> logger, DaprClient daprClient, IConfiguration configuration)
        {
            this.logger=logger;
            this.daprClient=daprClient;
            _configuration=configuration;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, string> secrets = await daprClient.GetSecretAsync("secrets01", "RabbitMQConnectStr");

            return Ok(secrets);
        }


        [HttpGet("get01")]
        public async Task<ActionResult> Get01Async()
        {
            return Ok(_configuration["RabbitMQConnectStr"]);
        }

    } 
}
