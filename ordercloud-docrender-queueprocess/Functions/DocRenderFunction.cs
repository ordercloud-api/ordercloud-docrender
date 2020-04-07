using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;
using OrderCloud.DocRender.common;

namespace OrderCloud.DocRender.QueueProcess
{
	public class DocRenderFunction
	{
		private readonly BlobService _blob;
		private readonly TableService _table;
		private readonly QueueService _queue;
		private readonly JobService _jobService;
		private readonly MpowerClient _renderService;

		public DocRenderFunction(BlobService blob, TableService table, QueueService queue, JobService jobService, MpowerClient renderService)
		{
			_blob = blob;
			_table = table;
			_queue = queue;
			_jobService = jobService;
			_renderService = renderService;
		}

		public async Task DocRenderFunctionHandlerAsync([QueueTrigger(Consts.RenderJobQueueName, Connection = Consts.StorageConnectionSettingName)] JobQueueMessage qm, ILogger logger)
		{
			var blockBlob = await _jobService.GetJobFileAsync(qm.UserContext, Consts.JobVarFolderName, Consts.JobVarFileName);
			var jobVarBody = await blockBlob.DownloadTextAsync();
			var jobFile = JsonConvert.DeserializeObject<JsonJobVarsFile>(jobVarBody);
			
			await _renderService.SubmitRenderJobAsync(qm, jobFile, MoveFileOpAsync);
			qm.LineItemJob.JobStatus = JobStatus.complete.ToString();
			await _table.InsertOrReplaceAsync(qm.LineItemJob);
			await _queue.QueueMessageAsync(Consts.CompletedJobQueueName, qm);//this queue could handle notifying the browser the job is done on the web side. It'd be a great use of web sockets.
		}

		private async Task MoveFileOpAsync(UserContext userContext, string filepath)
		{
			using(var outputFile = File.OpenRead(filepath))
			{
				await _jobService.WriteJobFileAsync(userContext, "rendered", Path.GetFileName(filepath), outputFile);
			}
		}
	}
}
