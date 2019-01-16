//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using OrderCloud.AzureStorage;
//using OrderCloud.DocRender.common;
//using OrderCloud.DocRender.common.Models;
//using OrderCloud.DocRender.renderqueue;

//namespace OrderCloud.DocRender.QueueProcess
//{
//	public class DocRenderServcie
//	{
//		public static async Task Run(object queuemessage)
//		{
//			var qm = JsonConvert.DeserializeObject<QueueMessage>((string)queuemessage);
			
//			var table = Container.Get<TableService>();
//			var queue = Container.Get<QueueService>();
//			var renderService = Container.Get<IDocRenderer>();
//			var blob = Container.Get<BlobService>();
	        
//			var jobFileBody = await blob.ReadTextFileBlobAsync(Consts.ContentBlobContainerName, Path.Combine(qm.UserContext.DocRenderConfigurationID, qm.LineItemJob.BlobFolder, Consts.JobVarFolderName), Consts.JobVarFileName);
//			var jobFile = JsonConvert.DeserializeObject<JsonJobVarsFile>(jobFileBody);

//			//await renderService.SubmitRenderJobAsync(jobFile.Specs, MoveFileOpAsync);
//			qm.LineItemJob.JobStatus = JobStatus.complete.ToString();
//			await table.InsertOrReplaceAsync(qm.LineItemJob);
//			await queue.QueueMessageAsync(Consts.CompletedJobQueueName, qm);
//		}

//		private static async Task MoveFileOpAsync(string filepath)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
