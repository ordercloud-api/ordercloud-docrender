using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderCloud.DocRender.common;

namespace OrderCloud.DocRender.renderqueue
{
    public static class FileSync
    {
        [FunctionName("FileSync")]
        public static void Run([QueueTrigger(Consts.FileSyncQueueName, Connection = Consts.StorageConnectionSettingName)]string myQueueItem, ILogger log)
        {
            var qitem = JsonConvert.DeserializeObject<FileChangeMessage>(myQueueItem);
			log.LogInformation($"{(qitem.fileAdded ? "copy" : "delete")} a file\r\n container: {qitem.blobContainer}\r\n folder: {qitem.folderpath}\r\n file: {qitem.filename}");
        }
    }
}
