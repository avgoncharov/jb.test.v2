using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using JB.Test.V2.DAL.Implementation.Extensions;
using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.Web.Models;
using Serilog;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("v3/registration3-gz")]
	public class RegistrationGzController : ApiController
	{
		private readonly IPackagesRepositoryReader _packagesRepositoryReader;
		private readonly ILogger _logger = Log.Logger.ForContext<RegistrationGzController>();

		public RegistrationGzController(IPackagesRepositoryReader packagesRepositoryReader)
		{
			_packagesRepositoryReader = packagesRepositoryReader ?? throw new ArgumentNullException(nameof(packagesRepositoryReader));
		}

		[Route("{id}/index.json"), HttpGet]
		public async Task<IHttpActionResult> GetAsync(string id, CancellationToken token)
		{
			try
			{
				var result = await _packagesRepositoryReader.FindAllByFilterAsync(new Filter { Id = id }, token);

				if (result == null || result.Any() != true)
				{
					return NotFound();
				}

				var groups = result.GroupBy(itr => itr.Id);

				return Ok(new
				{
					count = groups.Count(),
					items = groups.Select(itr => new PackageVersionsCollection
					{
						DId = Url.Content($"~/v3/registration3/{itr.Key}/index.json"),
						Count = itr.Count(),
						Items = itr.Select(pv =>
							new PackageVersion
							{
								DId = Url.Content($"~/v3/registration3/{pv.Id}/{pv.Version}.json"),
								CatalogEntry = new CatalogEntry
								{
									DId = Url.Content($"~/v3/catalog0/data/2017.10.05.18.41.33/{pv.Id}.{pv.Version}.json"),
									Id = pv.Id,
									Version = pv.Version
								},

								PackageContent = Url.Content($"~/api/nuget/{pv.Id}/{pv.Version}/{pv.BuildFileName()}"),
								Registration = Url.Content($"~/v3/registration3/{pv.Id}/index.json")

							}).ToArray(),
						Lower = itr.OrderBy(p => p.Version).First().Version,
						Upper = itr.OrderByDescending(p => p.Version).First().Version

					}).ToArray()
				});
			}
			catch (Exception ex)
			{
				_logger.Error(ex.ToString());
				throw;
			}
		}
	}
}
