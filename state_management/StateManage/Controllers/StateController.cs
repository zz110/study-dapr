using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StateManage.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly ILogger<StateController> _logger;

        private readonly DaprClient _daprClient;

        private const string StoreKey = "statestore";

        public StateController(ILogger<StateController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        /// <summary>
        /// 获取一个值
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _daprClient.GetStateAsync<string>(StoreKey, "guid");

            return Ok(result);
        }


        [HttpGet("withtag")]
        public async Task<IActionResult> GetWithEtagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>(StoreKey, "guid");
            return Ok($"value is {value},etag is {etag}");
        }

        /// <summary>
        /// 保存一个值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            await _daprClient.SaveStateAsync(StoreKey, "guid", Guid.NewGuid().ToString());

            return Ok("done");
        }

        /// <summary>
        /// 删除一个值
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync()
        {
            await _daprClient.DeleteStateAsync(StoreKey, "guid");
            return Ok("done");
        }

        /// <summary>
        /// 通过tag防止并发冲突 保存一个值
        /// </summary>
        /// <returns></returns>
        [HttpPost("withtag")]
        public async Task<IActionResult> PostWithTagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>(StoreKey, "guid");

            await _daprClient.TrySaveStateAsync<string>(StoreKey, "guid", Guid.NewGuid().ToString(), etag);

            return Ok("done");
        }

        /// <summary>
        /// 通过tag防止并发冲突,删除一个值
        /// </summary>
        /// <returns></returns>
        [HttpPost("withtag")]
        public async Task<IActionResult> DeleteWithTagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>(StoreKey, "guid");

            return Ok(await _daprClient.TryDeleteStateAsync(StoreKey, "guid", etag));
        }

        /// <summary>
        /// 从绑定获取一个值,键值name从路由模板获取
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet("frombinding/{name}")]
        public async Task<IActionResult> GetFromBindingAsync([FromState(StoreKey, "name")] StateEntry<string> state)
        {
            return Ok(state.Value);
        }

        /// <summary>
        /// 根据绑定获取并修改值,键值name从路由模板获取
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost("withbinding/{name}")]
        public async Task<IActionResult> PostWithBingdingAsync([FromState(StoreKey, "name")] StateEntry<string> state)
        {
            state.Value = Guid.NewGuid().ToString();
            return Ok(await state.TrySaveAsync());
        }

        /// <summary>
        /// 获取多个值
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetListAsync()
        {
            var result = await _daprClient.GetBulkStateAsync(StoreKey, new List<string> { "guid" }, 10);
            return Ok(result);

        }

        /// <summary>
        /// 删除多个值
        /// </summary>
        /// <returns></returns>
        [HttpDelete("list")]
        public async Task<IActionResult> DeleteListAsync()
        {
            var data = await _daprClient.GetBulkStateAsync(StoreKey, new List<string> { "guid" }, 10);
            var removeList = new List<BulkDeleteStateItem>();
            foreach (var item in data)
            {
                removeList.Add(new BulkDeleteStateItem(item.Key, item.ETag));
            }
            await _daprClient.DeleteBulkStateAsync(StoreKey, removeList);
            return Ok("done");
        }
    }
}
