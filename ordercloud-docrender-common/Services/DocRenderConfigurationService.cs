﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;

namespace OrderCloud.DocRender
{
	public class DocRenderConfigurationService
	{
		private readonly BlobService _blob;
		private JsonConfigurationFile _configurationFile = null;

		public DocRenderConfigurationService(BlobService blob)
		{
			_blob = blob;
			
		}

		
		public async Task<DocRenderConfiguration> GetByClientIDAsync(string clientID)
		{
			if (_configurationFile == null)
			{
				var file = await _blob.ReadTextFileBlobAsync("docrenderapps", "configurations.json");
				_configurationFile = JsonConvert.DeserializeObject<JsonConfigurationFile>(file);
			}

			var c =_configurationFile.Configurations.First(x => x.ClientIDs.Contains(clientID));
			return c;
		}
	}
}