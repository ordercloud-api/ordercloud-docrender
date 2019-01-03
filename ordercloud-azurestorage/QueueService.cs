//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Blob;
//using Microsoft.WindowsAzure.Storage.Queue;
//using Newtonsoft.Json;
//using System.IO;


//namespace OrderCloud.AzureStorage
//{
	//rethink the way queues are used now that blobs can trigger
	//public class QueueService //: IQueueService
	//{
	//	private string _conn;
	//	public QueueService(string storageConnection)
	//	{
	//		_conn = storageConnection;
	//	}
	//	public async Task DropMessageAsync(string queuename, object message)
	//	{
	//		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_conn);
	//		CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
	//		CloudQueue queue = queueClient.GetQueueReference(queuename);
	//		await queue.CreateIfNotExistsAsync();
			
	//		string fileName = Guid.NewGuid().ToString() + ".txt";

	//		var fileC = storageAccount.CreateCloudBlobClient();
	//		var container = fileC.GetContainerReference("asyncservicefiles");
	//		await container.CreateIfNotExistsAsync();
	//		var blob = container.GetBlockBlobReference(fileName);
	//		await blob.UploadTextAsync(JsonConvert.SerializeObject(message));

	//		await queue.AddMessageAsync(new CloudQueueMessage(fileName));
	//	}

	//	public async Task<string> DropFileByStreamAsync(string queuename, Stream stream, params string[] qparams)
	//	{
	//		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_conn);
	//		CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
	//		CloudQueue queue = queueClient.GetQueueReference(queuename);
	//		await queue.CreateIfNotExistsAsync();
	//		string fileName = Guid.NewGuid().ToString();

	//		var fileC = storageAccount.CreateCloudBlobClient();
	//		var container = fileC.GetContainerReference("asyncservicefiles");
	//		await container.CreateIfNotExistsAsync();
	//		var blob = container.GetBlockBlobReference(fileName);
	//		await blob.UploadFromStreamAsync(stream);
	//		await queue.AddMessageAsync(new CloudQueueMessage(fileName + "\t" + string.Join("\t", qparams)));
	//		return fileName;

	//	}
		//think about not doing this
		//public async Task ReadQueuedFileAsync(string fileName, out string url, out string body)
		//{
		//	CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_conn);
		//	var fileC = storageAccount.CreateCloudBlobClient();
		//	var container = fileC.GetContainerReference("asyncservicefiles");
		//	container.CreateIfNotExists();
		//	var blob = container.GetBlockBlobReference(fileName);
		//	url = blob.Uri.ToString();
		//	body = blob.DownloadText();
		//}

		//public Stream OpenFileStream(string fileName)
		//{
		//	var storageAccount = CloudStorageAccount.Parse(_conn);
		//	var fileC = storageAccount.CreateCloudBlobClient();
		//	var container = fileC.GetContainerReference("asyncservicefiles");
		//	container.CreateIfNotExists();
		//	var blob = container.GetBlockBlobReference(fileName);
		//	return blob.OpenRead();
		//}
		//public void CleanupStorage(TextWriter logger)
		//{
		//	const string StorageContainerName = "asyncservicefiles";
		//	var storageAccount = CloudStorageAccount.Parse(_conn);
		//	var blobClient = storageAccount.CreateCloudBlobClient();
		//	var container = blobClient.GetContainerReference(StorageContainerName);

		//	foreach (CloudBlockBlob blob in container.ListBlobs())
		//	{
		//		if (blob.Properties.LastModified < DateTime.Now.AddDays(-7))
		//		{
		//			blob.DeleteIfExists();
		//			logger.WriteLine($"deleting {blob.Uri.ToString()}");
		//		}
		//	}
		//	logger.WriteLine("Done!");
		//}
	//}
//}
