using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderCloud.DocRender.Functions
{
    public static class JobSubmit
    {
        [FunctionName("JobSubmit")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobs/{orderdirection}/{orderid}/{lineid}/submit")] HttpRequest req,
            ILogger log, string orderdirection, string orderid, string lineid)
        {
			var UserContext = await FunctionHelpers.Auth(req, orderdirection, orderid, lineid);
	        return new OkObjectResult(new {status = "OK"});
        }
	}
}
