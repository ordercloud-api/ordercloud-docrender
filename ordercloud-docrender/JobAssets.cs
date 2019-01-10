using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;

namespace OrderCloud.DocRender
{
	public static class JobAssets
	{
		
		[FunctionName("ListJobAssets")]
		public static async Task<IActionResult> RunListAssets([HttpTrigger(AuthorizationLevel.Function, "get", Route =
				"jobs/{orderdirection}/{orderid}/{lineid}/assets")]
			HttpRequest req,
			string orderdirection,
			string orderid,
			string lineid)
		{
			var userContext = await FunctionHelpers.Auth(req, orderdirection, orderid, lineid);
			var bob = Container.Get<JobService>();
			var result = await bob.ListAssets(userContext);

			return new OkObjectResult(result.ConvertAll<AssetFile>(x=> new AssetFile{Name = Path.GetFileName(x.Uri.ToString())}));
		}

		public class AssetFile
		{
			public string Name { get; set; }
		}
		[FunctionName("SaveJobAsset")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route =
				"jobs/{orderdirection}/{orderid}/{lineid}/assets/{id}")]
			HttpRequest req,
			string orderdirection,
			string orderid,
			string lineid,
			string id
			)
		{
			var userContext = await FunctionHelpers.Auth(req, orderdirection, orderid, lineid);
			var bob = Container.Get<JobService>();
			await bob.WriteJobFile(userContext, "assets", id, req.Body);
			return new OkObjectResult(new {status = "OK"});
		}
		[FunctionName("GetJobAsset")]
		public static async Task<IActionResult> RunGetJobVar(
			[HttpTrigger(AuthorizationLevel.Function, "Get", Route = "jobs/{orderdirection}/{orderid}/{lineid}/assets/{id}")]
			HttpRequest req,
			string orderdirection,
			string orderid,
			string lineid,
			string id
		)
		{
			var userContext = await FunctionHelpers.Auth(req, orderdirection, orderid, lineid);
			var file = await Container.Get<JobService>().GetJobFile(userContext, "assets", id);
			var stream = await file.OpenReadAsync();
			return new FileStreamResult(stream, file.Properties.ContentType);
		}
		[FunctionName("DeleteJobAsset")]
		public static async Task<IActionResult> RunDeleteJobAsset(
			[HttpTrigger(AuthorizationLevel.Function, "delete", Route =
				"jobs/{orderdirection}/{orderid}/{lineid}/assets/{id}")]
			HttpRequest req,
			string orderdirection,
			string orderid,
			string lineid,
			string id
			)
		{
			var userContext = await FunctionHelpers.Auth(req, orderdirection, orderid, lineid);
			var bob = Container.Get<JobService>();
			await bob.DeleteJobFile(userContext, "assets", id);
			return new OkObjectResult(new { status = "OK" });
		}
	}
}
