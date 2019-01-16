using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderCloud.AzureStorage;
using OrderCloud.DocRender.common;


namespace OrderCloud.DocRender.QueueProcess
{
	public class FileSyncFunction
	{
		private readonly AppSettings _settings;
		private readonly BlobService _blob;

		public FileSyncFunction(AppSettings settings, BlobService blob)
		{
			_settings = settings;
			_blob = blob;
		}
		public async Task FileSyncHandlerAsync([QueueTrigger(Consts.FileSyncQueueName, Connection = Consts.StorageConnectionSettingName)] FileChangeMessage filechange, ILogger logger)
		{
			var folderpath = Path.Combine(_settings.LocalJobAssetCacheFolder, filechange.folderpath);
			var filePath = Path.Combine(folderpath, filechange.filename);
			
			logger.LogInformation($"{(filechange.fileAdded ? "moving" : "deleting")} file:{filePath}");

			if(!filechange.fileAdded)
			{
				File.Delete(filePath);
			}
			else
			{
				Directory.CreateDirectory(folderpath);
				using (var localfile = File.Create(filePath))
				{
					using (var fs = await _blob.OpenReadonlyBlobStreamAsync(filechange.blobContainer, filechange.folderpath, filechange.filename))
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
		}
	}
}
