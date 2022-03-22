using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FrontEnd.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RabbitBindingController : ControllerBase
    {

        private readonly ILogger<RabbitBindingController> logger;

        private readonly DaprClient daprClient;

        public RabbitBindingController(ILogger<RabbitBindingController> logger, DaprClient daprClient)
        {
            this.logger=logger;
            this.daprClient=daprClient;
        }

        #region 输入

        /// <summary>
        /// 输入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post()
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            stream.ReadAsync(buffer, 0, buffer.Length);
            string content = Encoding.UTF8.GetString(buffer);
            logger.LogInformation(".............binding............." + content);
            return Ok();
        }

        #endregion

        #region 输出

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            await daprClient.InvokeBindingAsync("RabbitBinding", "create", "9999999");
            return Ok();
        }

        #endregion

    }
}
