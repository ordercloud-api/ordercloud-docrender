using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using OrderCloud.AzureStorage;

namespace OrderCloud.DocRender.common
{
	public class JobService
	{
		private readonly BlobService _blob;
		private readonly TableService _table;
		private readonly QueueService _queue;

		
		public JobService(BlobService blob, TableService table, QueueService queue)
		{
			_blob = blob;
			_table = table;
			_queue = queue;
		}

		public async Task WriteJobFile(UserContext userContext, string folder, string fileid, Stream s)
		{
			var lineJob = await GetOrSetLineJobAsync(userContext);
			var folderPath = $"{userContext.DocRenderConfigurationID}/{lineJob.BlobFolder}/{folder}";
			await _blob.WriteBlockBlobFromStreamAsync(Consts.ContentBlobContainerName, folderPath, fileid, s);
			if(folder == Consts.AssetsFolderName)
				await _queue.QueueMessageAsync(Consts.FileSyncQueueName, new FileChangeMessage{blobContainer = Consts.ContentBlobContainerName, fileAdded = true, filename = fileid, folderpath = folderPath});
		}

		public async Task<List<IListBlobItem>> ListAssets(UserContext userContext)
		{
			var lineJob = await GetOrSetLineJobAsync(userContext);
			return await _blob.ListAllBlobsAsync(Consts.ContentBlobContainerName, $"{userContext.DocRenderConfigurationID}/{lineJob.BlobFolder}/assets");
		}
		public async Task DeleteJobFile(UserContext userContext, string folder, string fileid)
		{
			var t = await GetOrSetLineJobAsync(userContext);
			var containerRef = _blob.BlobClient.GetContainerReference(Consts.ContentBlobContainerName);
			var folderPath = $"{userContext.DocRenderConfigurationID}/{t.BlobFolder}/{folder}";
			var d = containerRef.GetDirectoryReference(folderPath);
			var blob = d.GetBlockBlobReference(fileid);
			await blob.DeleteAsync();
			if(folder == Consts.AssetsFolderName)
				await _queue.QueueMessageAsync(Consts.FileSyncQueueName, new FileChangeMessage{blobContainer = Consts.ContentBlobContainerName, fileAdded = false, filename = fileid, folderpath = folderPath});
		}

		public async Task<CloudBlockBlob> GetJobFile(UserContext userContext, string folder, string fileid)
		{
			var t = await GetOrSetLineJobAsync(userContext);
			var containerRef = _blob.BlobClient.GetContainerReference(Consts.ContentBlobContainerName);
			var d = containerRef.GetDirectoryReference($"{userContext.DocRenderConfigurationID}/{t.BlobFolder}/{folder}");
			return d.GetBlockBlobReference(fileid);
		}
		public async Task<LineItemJob> GetOrSetLineJobAsync(UserContext context, bool requiredExisiting = false)
		{
			LineItemJob t = null;
			try
			{
				t = await _table.GetAsync<LineItemJob>(context.DocRenderConfigurationID, $"{context.OrderID}-{context.LineItemID}");
			}
			catch (OrderCloud.AzureStorage.Exceptions.NotFoundException)
			when(!requiredExisiting)
			{
				t = new LineItemJob
				{
					RowKey = $"{context.OrderID}-{context.LineItemID}",
					PartitionKey = context.DocRenderConfigurationID,
					BlobFolder = Guid.NewGuid().ToString(),
					JobStatus = JobStatus.unsubmitted.ToString()
				};
				await _table.InsertOrReplaceAsync(t);
			}
			return t;
		}
	}
}
