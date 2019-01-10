using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderCloud.AzureStorage;

namespace OrderCloud.DocRender
{
	public class DocRenderImplementationService
	{
		private readonly BlobService _blob;
		private JsonImplementationFile _implementationData = null;

		public DocRenderImplementationService(BlobService blob)
		{
			_blob = blob;
			
		}

		
		public async Task<DocRenderImplementation> GetByClientIDAsync(string clientID)
		{
			if (_implementationData == null)
			{
				var file = await _blob.ReadTextFileBlobAsync("docrenderapps", "implementations.json");
				_implementationData = JsonConvert.DeserializeObject<JsonImplementationFile>(file);
			}

			var c =_implementationData.implementations.First(x => x.ClientIDs.Contains(clientID));
			return c;
		}
	}
}