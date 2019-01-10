using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.StaticFiles;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace OrderCloud.AzureStorage
{
	public class BlobService
	{
		private readonly CloudStorageAccount _storageAccount;
		private readonly CloudBlobClient _blobClient;
		private readonly string _connection;
		//public async Task DestroyAllBlogStorageForDevTestOnly()
		//{
		//	if (_connection != "UseDevelopmentStorage=true;")
		//		throw new Exception("this will delete all your datas. only for dev/test purposes");

		//	_blobClient.ListContainers().ToList().ForEach(async c => await c.DeleteAsync());
		//}
		public CloudBlobClient BlobClient => _blobClient;

		public BlobService(string connectionString)
		{
			_connection = connectionString;
			_storageAccount = CloudStorageAccount.Parse(connectionString);
			_blobClient = _storageAccount.CreateCloudBlobClient();

		}
		public async Task SetAnonAccessOnContainerAsync(string storageContainerName, BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Blob)
		{
			var containerRef = _blobClient.GetContainerReference(storageContainerName);
			await containerRef.CreateIfNotExistsAsync();
			BlobContainerPermissions permissions = await containerRef.GetPermissionsAsync();
			permissions.PublicAccess = accessType;
			await containerRef.SetPermissionsAsync(permissions);
		}
		public async Task<Uri> WriteBlockBlobFromStreamAsync(string storageContainerName, string folderpath, string blobName, Stream fileStream)
		{
			var containerRef = _blobClient.GetContainerReference(storageContainerName);
			await containerRef.CreateIfNotExistsAsync();
			CloudBlockBlob blob = null;
			if (!string.IsNullOrEmpty(folderpath))
			{
				var d = containerRef.GetDirectoryReference(folderpath);
				blob = d.GetBlockBlobReference(blobName);
			}
			else
				blob = containerRef.GetBlockBlobReference(blobName);
			
			blob.Properties.ContentType = MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(blobName));
			await blob.UploadFromStreamAsync(fileStream);
			return blob.Uri;
		}
		public async Task<Uri> WriteBlockBlobFromStringBodyAsync(string storageContainerName, string folderpath, string blobName, string textOfFile)
		{
			var containerRef = _blobClient.GetContainerReference(storageContainerName);
			await containerRef.CreateIfNotExistsAsync();

			CloudBlockBlob blob = null;
			if (string.IsNullOrEmpty(folderpath))
			{
				var d = containerRef.GetDirectoryReference(folderpath);
				d.GetBlockBlobReference(blobName);
			}
			else
				blob = containerRef.GetBlockBlobReference(blobName);

			await blob.UploadTextAsync(textOfFile);
			return blob.Uri;
		}

		public async Task<string> ReadTextFileBlobAsync(string storageContainerName, string blobName)
		{
			var container = _blobClient.GetContainerReference(storageContainerName);
			var blob = container.GetBlockBlobReference(blobName);
			//url = blob.Uri.ToString();
			var body = await blob.DownloadTextAsync();
			return body;
		}

		
		public async Task<List<IListBlobItem>>ListAllBlobsAsync(string storageContainerName, string folderPath)
		{
			var containerRef = _blobClient.GetContainerReference(storageContainerName);
			var dir = containerRef.GetDirectoryReference(folderPath);
			BlobContinuationToken ct = null;
			List<IListBlobItem> results = new List<IListBlobItem>();
			do
			{
				var segmentresponse = await dir.ListBlobsSegmentedAsync(ct);
				ct = segmentresponse.ContinuationToken;
				results.AddRange(segmentresponse.Results);
			}
			while (ct != null);
			return results;

		}
	}
}
