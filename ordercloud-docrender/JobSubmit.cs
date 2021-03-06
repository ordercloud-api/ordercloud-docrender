using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrderCloud.AzureStorage;
using OrderCloud.DocRender.common;

namespace OrderCloud.DocRender.webapi
{
    public static class JobSubmit
    {
        [FunctionName("JobSubmit")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobs/{orderdirection}/{orderid}/{lineid}/render/{jobname}")] HttpRequest req,
            ILogger log, string orderdirection, string orderid, string lineid, string jobname)
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
		        var t2 = Container.Get<QueueService>().QueueMessageAsync(Consts.RenderJobQueueName, new JobQueueMessage{JobName = jobname, UserContext = userContext, LineItemJob = job });
		        var t1 = Container.Get<TableService>().InsertOrReplaceAsync(job);
		        Task.WaitAll(t1, t2);
		        return new OkObjectResult(new { status = "OK" });
				//somehow the browser needs to be notified when the render job is done, but it shouldn't block this call. it's either web sockets or browser polling. Right now, the render job drops a message on the queue named Consts.CompletedJobQueueName, but nothing is done with it.
			}
		}
	}
}
