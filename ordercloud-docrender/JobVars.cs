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
			var UserContext = await FunctionHelpers.Auth(req, orderdirection, orderid, lineid);
			var bob = Container.Get<JobService>();
			await bob.WriteJobFile(UserContext, "JobVars", "job.json", req.Body);
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
			var UserContext = await FunctionHelpers.Auth(req, orderdirection, orderid, lineid);
			var file = await Container.Get<JobService>().GetJobFile(UserContext, "JobVars", "job.json");
			var stream = await file.OpenReadAsync();
			return new FileStreamResult(stream, file.Properties.ContentType);
		}
	}
}
