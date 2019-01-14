using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender.common
{
	public class AppSettings
	{
		public string StorageConnection { get; set; }
		public string LocalDriveLeterForMpowerFinaloutput { get; set; }
		public string MpowerHostName { get; set; }
		public int MpowerTcpPort { get; set; }
		public string LocalJobAssetCacheFolder {get;set;}
	}
}
