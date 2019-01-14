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
using OrderCloud.DocRender.common;

namespace OrderCloud.DocRender.webapi
{
	public static class JobVars
	{
		[FunctionName("SaveJobVars")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobs/{orderdirection}/{orderid}/{lineid}/JobVars")] HttpRequest req,
			string orderdirection,
			string orderid,
			string lineid
			)
		{
			var UserContext = await FunctionHelpers.AuthAsync(req, orderdirection, orderid, lineid);
			var job = Container.Get<JobService>();
			var q = Container.Get<QueueService>();
			await job.WriteJobFile(UserContext, Consts.JobVarFolderName, Consts.JobVarFileName, req.Body);
			return new OkObjectResult(new { status = "OK" });
		}

		[FunctionName("GetJobVars")]
		public static async Task<IActionResult> RunGetJobVar(
			[HttpTrigger(AuthorizationLevel.Function, "Get", Route = "jobs/{orderdirection}/{orderid}/{lineid}/JobVars")]
			HttpRequest req,
			string orderdirection,
			string orderid,
			string lineid
			)
		{
			var UserContext = await FunctionHelpers.AuthAsync(req, orderdirection, orderid, lineid);
			var file = await Container.Get<JobService>().GetJobFile(UserContext, Consts.JobVarFolderName, Consts.JobVarFileName);
			var stream = await file.OpenReadAsync();
			return new FileStreamResult(stream, file.Properties.ContentType);
		}
	}
}
