using JB.Test.V2.DAL.Interfaces;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("api/packages")]
	public class ApiPackagesController : ApiController
	{
		private readonly IPackagesRepository _packagesRepository;
		private readonly IPackagesFactory _pacakgeFactory;
		private readonly ILogger _logger = Log.Logger.ForContext<ApiPackagesController>();


		public ApiPackagesController(
			IPackagesRepository packagesRepository,
			IPackagesFactory pacakgeFactory)
		{				
			_packagesRepository = packagesRepository ?? throw new ArgumentNullException(nameof(packagesRepository));
			_pacakgeFactory = pacakgeFactory ?? throw new ArgumentNullException(nameof(pacakgeFactory));
		}


		[Route(""), HttpPost]
		public async Task<IHttpActionResult> GetByFilter([FromBody] Filter filter, CancellationToken token)
		{
			try
			{
				var result = await _packagesRepository.FindAllByFilterAsync(filter, token);

				if (result == null || result.Any() != true)
				{
					return NotFound();
				}

				return Ok(result);
			}
			catch (Exception ex) {
				_logger.Error(ex.ToString());
				throw;
			}
		}


		[Route("{id}/{version}"), HttpGet]
		public async Task<IHttpActionResult> GetVersion(string id, string version, CancellationToken token)
		{
			var result = await _packagesRepository.FindAllByFilterAsync(new Filter { Id = id, Version = version }, token);
			if(result == null || result.Any() != true)
			{
				return NotFound();
			}

			return Ok(result.First());
		} 
	}
}
