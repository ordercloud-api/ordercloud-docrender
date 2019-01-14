using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;
using OrderCloud.DocRender.common;

namespace OrderCloud.DocRender.renderqueue
{
    public static class FileSync
    {
        [FunctionName("FileSync")]
        public static async Task Run([QueueTrigger(Consts.FileSyncQueueName, Connection = Consts.StorageConnectionSettingName)]string myQueueItem, ILogger log)
        {
			var settings = Container.Get<AppSettings>();
            var filechange = JsonConvert.DeserializeObject<FileChangeMessage>(myQueueItem);
			

			var folderpath = Path.Combine(settings.LocalJobAssetCacheFolder, filechange.folderpath);
			
			if(!filechange.fileAdded)
			{
				File.Delete(Path.Combine(folderpath, filechange.filename));
			}
			else
			{
				var blob = Container.Get<BlobService>();
				Directory.CreateDirectory(folderpath);
			
				using (var localfile = File.Create(Path.Combine(folderpath, filechange.filename)))
				{
					using (var fs = await blob.OpenReadonlyBlobStreamAsync(filechange.blobContainer, filechange.folderpath, filechange.filename))
					{
						int bytesRead = -1;
						int bufferSize = 1024 * 1024;

						byte[] bytes = new byte[bufferSize];
						//bytesRead = fs.Read(bytes, 0, bufferSize);
					
						while ((bytesRead = await fs.ReadAsync(bytes, 0, bufferSize)) > 0)
						{
							localfile.Write(bytes, 0, bytesRead);
						}
					}
				}
			}
	        

			log.LogInformation($"{(filechange.fileAdded ? "copy" : "delete")} a file\r\n container: {filechange.blobContainer}\r\n folder: {filechange.folderpath}\r\n file: {filechange.filename}");
        }
    }
}
