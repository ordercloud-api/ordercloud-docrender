using System;
using System.Collections.Generic;
using System.Text;

namespace OrderCloud.DocRender
{
	public class UserContext
	{
		public string ClientID { get; set; }
		public string DocRenderConfigurationID { get; set; }
		public string UserName { get; set; }
		public string OrderID { get; set; }
		public string LineItemID { get; set;}
		public string OrderDirection { get; set; }
	}
}
