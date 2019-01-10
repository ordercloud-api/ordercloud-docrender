using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender
{
	public class JsonImplementationFile
	{
		public List<DocRenderImplementation> implementations { get; set; }
	}
	public class DocRenderImplementation
	{
		public string ID { get; set; }
		public List<string> ClientIDs { get; set; }
	}
}
