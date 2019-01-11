using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using OrderCloud.AzureStorage;

namespace OrderCloud.DocRender
{
	public class JobService
	{
		private readonly BlobService _blob;
		private readonly TableService _table;


		public JobService(BlobService blob, TableService table)
		{
			_blob = blob;
			_table = table;
		}

		public async Task WriteJobFile(UserContext userContext, string folder, string fileid, Stream s)
		{
			var lineJob = await GetOrSetLineJobAsync(userContext);
			await _blob.WriteBlockBlobFromStreamAsync("docrenderapps", $"{userContext.DocRenderConfigurationID}/{lineJob.BlobFolder}/{folder}", fileid, s);
		}

		public async Task<List<IListBlobItem>> ListAssets(UserContext userContext)
		{
			var lineJob = await GetOrSetLineJobAsync(userContext);
			return await _blob.ListAllBlobsAsync("docrenderapps", $"{userContext.DocRenderConfigurationID}/{lineJob.BlobFolder}/assets");
		}
		public async Task DeleteJobFile(UserContext userContext, string folder, string fileid)
		{
			var t = await GetOrSetLineJobAsync(userContext);
			var containerRef = _blob.BlobClient.GetContainerReference("docrenderapps");
			var d = containerRef.GetDirectoryReference($"{userContext.DocRenderConfigurationID}/{t.BlobFolder}/{folder}");
			var blob = d.GetBlockBlobReference(fileid);
			await blob.DeleteAsync();
		}

		public async Task<CloudBlockBlob> GetJobFile(UserContext userContext, string folder, string fileid)
		{
			var t = await GetOrSetLineJobAsync(userContext);
			var containerRef = _blob.BlobClient.GetContainerReference("docrenderapps");
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
