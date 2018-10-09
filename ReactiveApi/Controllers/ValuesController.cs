using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reactive.Linq;
using System.Net.Http;
using System.Reactive.Threading.Tasks;
using System.Reactive.Concurrency;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace ReactiveApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
    
        static readonly HttpClient _client = new HttpClient();
    
        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }
    
        // GET api/values
        [HttpGet]
        public Task<IActionResult> Get()
        {
            return Observable
                    .FromAsync(() => _client.GetAsync("https://jsonplaceholder.typicode.com/todos"))
                    .SubscribeOn(TaskPoolScheduler.Default)
                    .Retry(5)
                    .Timeout(TimeSpan.FromMilliseconds(80))
                    .Do(x => _logger?.LogInformation($"Message Successful? :{x.IsSuccessStatusCode}"))
                    .SelectMany(
                        async x =>
                        {
                            JArray parsedTodos = null;
                            
                            using(var stream = await x.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            using(var sr = new StreamReader(stream))
                            using(var jtr = new JsonTextReader(sr))
                            {
                                parsedTodos = await JArray.LoadAsync(jtr).ConfigureAwait(false);
                            }

                            return parsedTodos.Select(pt => pt.SelectToken("title")).ToList();
                        })
                    .Select(x => new JsonResult(x))
                    .Catch<IActionResult, TimeoutException>(ex => Observable.Return(StatusCode(StatusCodes.Status503ServiceUnavailable)))
                    .Catch<IActionResult, Exception>(ex => Observable.Return(StatusCode(StatusCodes.Status500InternalServerError)))
                    .ToTask();
        }
    }
}
