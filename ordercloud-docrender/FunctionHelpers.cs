using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OrderCloud.DocRender
{
	public static class FunctionHelpers
	{
		public static async Task<UserContext> Auth(HttpRequest req, string orderdirection, string orderid, string lineid)
		{
			return await Container.Get<OcAuthorizationService>().AuthorizeAsync("parse the token out", orderdirection, orderid, lineid);
		}
	}
}
