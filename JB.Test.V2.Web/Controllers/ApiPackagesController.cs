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
		private readonly IPackagesRepositoryReader _packagesRepositoryReader;		
		private readonly ILogger _logger = Log.Logger.ForContext<ApiPackagesController>();


		public ApiPackagesController(IPackagesRepositoryReader packagesRepositoryReader)
		{				
			_packagesRepositoryReader = packagesRepositoryReader ?? throw new ArgumentNullException(nameof(packagesRepositoryReader));			
		}


		[Route(""), HttpPost]
		public async Task<IHttpActionResult> GetByFilterAsync([FromBody] Filter filter, CancellationToken token)
		{
			try
			{
				var result = await _packagesRepositoryReader.FindAllByFilterAsync(filter, token);

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
		public async Task<IHttpActionResult> GetVersionAsync(string id, string version, CancellationToken token)
		{
			var result = await _packagesRepositoryReader.FindAllByFilterAsync(new Filter { Id = id, Version = version }, token);
			if(result == null || result.Any() != true)
			{
				return NotFound();
			}

			return Ok(result.First());
		} 
	}
}
