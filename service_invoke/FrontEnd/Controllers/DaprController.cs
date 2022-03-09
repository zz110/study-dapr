using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    /// <summary>
    /// 手动创建 DaprClient - 不推荐
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DaprController : ControllerBase
    {
        private readonly ILogger<DaprController> _logger;

        public DaprController(ILogger<DaprController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            // 创建 dapr HttpClient  传入 app-id  , daprEndpoint  
            // DaprClient.CreateInvokeHttpClient() 这样创建实例 默认连接端口为：3500
            using var httpClient = DaprClient.CreateInvokeHttpClient("backend", "http://localhost:50000");
     
            var result = await httpClient.GetAsync("http://backend/WeatherForecast");

            var resultContext = string.Format("result is {0} {1}",
                result.StatusCode,
                await result.Content.ReadAsStringAsync());

            return Ok(resultContext);
        }

        [HttpGet("get2")]
        public async Task<IEnumerable<WeatherForecast>> Get2Async()
        {
            // DaprClientBuilder 创建 daprClient  并指定 http调用端点
            // DaprClientBuilder().Build(); 这样创建实例 默认连接端口为：3500
            using var daprClient = new DaprClientBuilder().UseHttpEndpoint("http://localhost:50000").Build();
            var result = await daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(
                HttpMethod.Get,
                "backend",
                "WeatherForecast"
                );

            return result;
        }
    }
}
