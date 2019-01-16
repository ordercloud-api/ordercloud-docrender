using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace OrderCloud.AzureStorage
{
	public class QueueService
	{
		private string _conn;
		private CloudStorageAccount _storageAccount;
		public readonly CloudQueueClient CloudQueueClient;
		public QueueService(string storageConnection)
		{
			_conn = storageConnection;
			_storageAccount	= CloudStorageAccount.Parse(_conn);
			CloudQueueClient = _storageAccount.CreateCloudQueueClient();
		}

		public async Task QueueMessageAsync(string queuename, object message)
		{
			CloudQueue queue = CloudQueueClient.GetQueueReference(queuename);
			await queue.CreateIfNotExistsAsync();
			await queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(message)));
		}
		public async Task<CloudQueueMessage> GetQueueAsync(string queuename)
		{
			CloudQueue queue = CloudQueueClient.GetQueueReference(queuename);
			return await queue.GetMessageAsync(new TimeSpan(0, 0, 10), new QueueRequestOptions(), new OperationContext()); 
		}
	}
}