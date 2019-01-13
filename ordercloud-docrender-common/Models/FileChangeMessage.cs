using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender.common
{
	public class FileChangeMessage
	{
		public string blobContainer {get;set;}
		public string filename {get;set;}
		public bool fileAdded {get;set;}
		public string folderpath {get;set;}
	}
}
