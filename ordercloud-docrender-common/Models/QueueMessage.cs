using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender.common
{
	public class QueueMessage
	{
		public UserContext UserContext {get; set;}
		public LineItemJob LineItemJob {get;set;}
	}
}
