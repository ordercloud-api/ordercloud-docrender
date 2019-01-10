using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrderCloud.SDK;

namespace OrderCloud.DocRender
{
	public static class FunctionHelpers
	{
		public static async Task<UserContext> AuthAsync(HttpRequest req, string orderdirection, string orderid, string lineid)
		{
			var t = req.Headers["Authorization"].First().Replace("Bearer ", "");
			var jwt = new JwtSecurityToken(t);
			var oc = Container.Get<OrderCloudClient>();
			var direction = Enum.Parse<OrderDirection>(orderdirection, true);
			var clientID = jwt.Claims.First(x => x.Type == "cid").Value;

			var implementation = await Container.Get<DocRenderImplementationService>().GetByClientIDAsync(clientID);

			try //validate they have access to this order/lineitem
			{
				await oc.LineItems.GetAsync(direction, orderid, lineid, t);
				return new UserContext 
				{
					ClientID = clientID,
					LineItemID = lineid,
					OrderID = orderid,
					DocRenderImplementationID = implementation.ID,
					UserName = jwt.Claims.First(x => x.Type == "usr").Value,
					OrderDirection = orderdirection
				};
			}
			catch (Exception e)
			{
				throw new Exceptions.NotAuthorized();
			}
		}
	}
}
