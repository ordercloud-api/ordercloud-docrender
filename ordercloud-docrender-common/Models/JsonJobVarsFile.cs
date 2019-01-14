using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace OrderCloud.DocRender.common.Models
{
	public class JsonJobVarsFile
	{
		public string ProjectID{get;set;}
		public Dictionary<string,string> Specs {get;set;}
	}
}
