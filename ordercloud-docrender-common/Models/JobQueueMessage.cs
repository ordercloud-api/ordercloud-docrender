using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender.common
{
	public class JobQueueMessage
	{
		public UserContext UserContext {get; set;}
		public LineItemJob LineItemJob {get;set;}
		public string JobName{get;set;} //preview proof production?
	}
}
