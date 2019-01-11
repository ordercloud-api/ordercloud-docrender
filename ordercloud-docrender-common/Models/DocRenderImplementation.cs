using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender
{
	public class JsonConfigurationFile
	{
		public List<DocRenderConfiguration> Configurations { get; set; }
	}
	public class DocRenderConfiguration
	{
		public string ID { get; set; }
		public List<string> ClientIDs { get; set; }
	}
}
