using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;

namespace OrderCloud.DocRender.Functions
{
    public static class JobSubmit
    {
        [FunctionName("JobStatus")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobs/{orderdirection}/{orderid}/{lineid}/render")] HttpRequest req,
            ILogger log, string orderdirection, string orderid, string lineid)
        {
			var userContext = await FunctionHelpers.AuthAsync(req, orderdirection, orderid, lineid);
			var job = await Container.Get<JobService>().GetOrSetLineJobAsync(userContext, true);
			if (job.JobStatus == JobStatus.inprogress.ToString())
			{
				return new OkObjectResult(new { status = "Job is already inprogress" });
			}
	        else
	        {
		        job.JobStatus = JobStatus.inprogress.ToString();
		        var t2 = Container.Get<QueueService>().QueueMessageAsync("renderjob", new { userContext, job });
		        var t1 = Container.Get<TableService>().InsertOrReplaceAsync(job);
		        Task.WaitAll(t1, t2);
		        return new OkObjectResult(new { status = "OK" });
			}
        }
	}
}
