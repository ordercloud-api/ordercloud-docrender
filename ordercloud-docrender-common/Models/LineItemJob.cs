using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace OrderCloud.DocRender
{

	public enum JobStatus { complete, error,unsubmitted, inprogress}
	public class LineItemJob : TableEntity
	{
		public LineItemJob(){}
		public string BlobFolder { get; set; }
		public string JobStatus { get; set; }
		//public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
		//{
		//	var d = base.WriteEntity(operationContext);
		//	d.Add("JobStatus", new EntityProperty(this.JobStatus.ToString()));
		//	return d;
		//}

		//public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
		//{
		//	base.ReadEntity(properties, operationContext);
		//}
	}
}
