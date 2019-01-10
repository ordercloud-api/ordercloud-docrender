using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrderCloud.SDK;

namespace OrderCloud.DocRender
{
	public static class FunctionHelpers
	{
		public static async Task<UserContext> Auth(HttpRequest req, string orderdirection, string orderid, string lineid)
		{
			var t = req.Headers["Authorization"].First().Replace("Bearer ", "");
			var oc = Container.Get<OrderCloudClient>();
			var direction = Enum.Parse<OrderDirection>(orderdirection, true);

			try //validate they have access to this order/lineitem
			{
				await oc.LineItems.GetAsync(direction, orderid, lineid, t);
			}
			catch (Exception e)
			{
				throw new Exceptions.NotAuthorized();
			}
			
			return await Container.Get<OcAuthorizationService>().AuthorizeAsync(t, orderdirection, orderid, lineid);
		}
	}
}
