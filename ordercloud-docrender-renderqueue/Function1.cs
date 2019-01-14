using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;
using OrderCloud.DocRender.common;
using OrderCloud.DocRender.common.Models;

namespace OrderCloud.DocRender.renderqueue
{
    public static class Function1
    {
        [FunctionName("ProcessDocRenderJob")]
        public static async Task Run([QueueTrigger(Consts.RenderJobQueueName, Connection = Consts.StorageConnectionSettingName)]string myQueueItem, ILogger log)
        {
			var qm = JsonConvert.DeserializeObject<QueueMessage>(myQueueItem);
			log.LogInformation($"username: {qm.UserContext.UserName}\r\n clientid: {qm.UserContext.ClientID} \r\n orderid: {qm.UserContext.OrderID}");
			
			var table = Container.Get<TableService>();
	        var queue = Container.Get<QueueService>();
			var renderService = Container.Get<IDocRenderer>();
			var blob = Container.Get<BlobService>();
	        
			var jobFileBody = await blob.ReadTextFileBlobAsync(Consts.ContentBlobContainerName, Path.Combine(qm.UserContext.DocRenderConfigurationID, qm.LineItemJob.BlobFolder, Consts.JobVarFolderName), Consts.JobVarFileName);
			var jobFile = JsonConvert.DeserializeObject<JsonJobVarsFile>(jobFileBody);

			//await renderService.SubmitRenderJobAsync(jobFile.Specs, MoveFileOpAsync);
			qm.LineItemJob.JobStatus = JobStatus.complete.ToString();
			await table.InsertOrReplaceAsync(qm.LineItemJob);
	        await queue.QueueMessageAsync(Consts.CompletedJobQueueName, qm);
        }

	    private static async Task MoveFileOpAsync(string filepath)
	    {
		    throw new NotImplementedException();
	    }
    }
}
