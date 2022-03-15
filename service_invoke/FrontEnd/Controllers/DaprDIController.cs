using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    /// <summary>
    /// 依赖注入方式创建 DaprClient - 推荐
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DaprDIController : ControllerBase
    {
        private readonly ILogger<DaprDIController> _logger;

        private readonly DaprClient _daprClient;

        public DaprDIController(ILogger<DaprDIController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(
                HttpMethod.Get,
                "backend",
                "WeatherForecast"
                );

            return result;
        }
    }
}
