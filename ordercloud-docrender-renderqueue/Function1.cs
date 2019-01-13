using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;
using OrderCloud.DocRender.common;

namespace OrderCloud.DocRender.renderqueue
{
    public static class Function1
    {
        [FunctionName("ProcessDocRenderJob")]
        public static async Task Run([QueueTrigger("renderjob", Connection = "StorageConnection")]string myQueueItem, ILogger log)
        {
			var qm = JsonConvert.DeserializeObject<QueueMessage>(myQueueItem);
			log.LogInformation($"username: {qm.UserContext.UserName}\r\n clientid: {qm.UserContext.ClientID} \r\n orderid: {qm.UserContext.OrderID}");
			var t = Container.Get<TableService>();
	        var q = Container.Get<QueueService>();
			qm.LineItemJob.JobStatus = JobStatus.complete.ToString();
			await t.InsertOrReplaceAsync(qm.LineItemJob);
	        await q.QueueMessageAsync("completedjobs", qm);
        }
    }
}
