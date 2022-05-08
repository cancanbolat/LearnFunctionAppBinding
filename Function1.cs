using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using TableStorage.Entities;
using System.Linq;

namespace FunctionAppBinding
{
    public static class Function1
    {
        //return table storage ile
        const string routeName = "products";
        [FunctionName("Post")]
        [return: Table(tableName: "products", Connection = "StorageConnectionString")]
        public static async Task<Product> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = routeName)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string body = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(body);

            return product;
        }

        [FunctionName("Get")]
        public static async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = routeName)] HttpRequest req,
            ILogger log, [Table(tableName: "products", Connection = "StorageConnectionString")] CloudTable cloudTable
            )
        {
            return new OkObjectResult(cloudTable.CreateQuery<Product>().AsQueryable());
        }
    }
}
/* Normal binding ile
 [FunctionName("Post")]
        public static async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = routeName)] HttpRequest req,
            ILogger log, [Table(tableName: "products", Connection = "StorageConnectionString")] CloudTable cloudTable)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string body = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(body);

            await cloudTable.CreateIfNotExistsAsync();
            TableOperation operation = TableOperation.Insert(product);
            TableResult tableResult = await cloudTable.ExecuteAsync(operation);

            return new OkObjectResult(tableResult.Result);
        }
 */