using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("v3/registration3-gz")]
	public class RegistrationGzController : ApiController
	{
		class CatalogEntry
		{
			[JsonProperty(PropertyName = "@id")]
			public string DId { get; set; }
			[JsonProperty(PropertyName = "id")]
			public string Id { get; set; }

			[JsonProperty(PropertyName = "version")]
			public string Version { get; set; }
		}

		class InnerItems
		{
			[JsonProperty(PropertyName = "@id")]
			public string DId { get; set; }

			[JsonProperty(PropertyName = "catalogEntry")]
			public CatalogEntry CatalogEntry { get; set; }


			[JsonProperty(PropertyName = "packageContent")]
			public string PackageContent { get; set; }

			[JsonProperty(PropertyName = "registration")]
			public string Registration { get; set; }



		}

		class Item
		{
			[JsonProperty(PropertyName = "@id")]
			public string DId { get; set; }


			[JsonProperty(PropertyName = "count")]
			public int Count { get; set; }

			[JsonProperty(PropertyName = "items")]
			public InnerItems[] Items { get; set; }

			[JsonProperty(PropertyName = "lower")]
			public string Lower { get; set; }

			[JsonProperty(PropertyName = "upper")]
			public string Upper { get; set; }

		}
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
							    PackageContent = "https://api.nuget.org/v3-flatcontainer/nuget.server.core/3.0.0-beta/nuget.server.core.3.0.0-beta.nupkg",
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
