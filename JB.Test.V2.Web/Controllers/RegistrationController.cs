using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("v3/registration3")]
	public class RegistrationController : ApiController
	{
		[Route("{id}/index.json"), HttpGet]
		public IHttpActionResult Get(string id)
		{
			return Ok(new
			{
				count = 1,
				items = new[]
				{
					new   Item
					{
						DId = "https://api.nuget.org/v3/registration3/nuget.server.core/index.json#page/3.0.0-beta/3.0.0-beta",
						Count = 1,
						Items = new []
						{
							new InnerItems
							{
							    DId = "https://api.nuget.org/v3/registration3/nuget.server.core/3.0.0-beta.json",
							    CatalogEntry = new CatalogEntry
								{
									DId = "https://api.nuget.org/v3/catalog0/data/2017.10.05.18.41.33/nuget.server.core.3.0.0-beta.json",
									Id="moqx",
									Version = "4.8.2"
								},
							    PackageContent = $"http://localhost:65370/api/nuget/Newtonsoft.Json/11.0.1/package.json",
							    Registration = "https://api.nuget.org/v3/registration3/nuget.server.core/index.json"
							}
						},
						Lower = "1.0.0",
						Upper = "5.0.0"
					}
				},

			});
		}
	}
}
