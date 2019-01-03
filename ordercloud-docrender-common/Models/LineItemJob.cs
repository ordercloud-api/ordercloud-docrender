using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace OrderCloud.DocRender
{
	public class LineItemJob : TableEntity
	{
		public LineItemJob(){}
		public string BlobFolder { get; set; }

	}
}
