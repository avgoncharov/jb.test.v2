using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("api/api-key-generator")]
	public class ApiKeyGeneratorController : ApiController
	{
		private readonly INugetUserRepository _nugetUserRepository;
		private readonly ILogger _logger = Log.Logger.ForContext<ApiKeyGeneratorController>();


		public ApiKeyGeneratorController(INugetUserRepository nugetUserRepository)
		{
			_nugetUserRepository = nugetUserRepository ?? throw new ArgumentNullException(nameof(nugetUserRepository));			
		}


		[Route("generate-new-for/{userName}"), HttpPost]
		public async Task<IHttpActionResult> PostAsync(string userName, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(userName))
			{
				return BadRequest("User name can't be empty.");
			}

			try
			{
				var guid = Guid.NewGuid().ToString();
				await _nugetUserRepository.CreateUserAsync(guid, userName, token);
				return Ok(guid);
			}
			catch (ArgumentException ex){
				_logger.Warning($"Create new user failed. Resone: {ex.Message}");
				return BadRequest();
			}
			catch(UserIsAlreadyExistException ex)
			{
				_logger.Warning($"Create new user failed. Resone: {ex.Message}");
				return BadRequest(ex.Message);
			}
			catch(Exception ex) {
				_logger.Error($"Unexpected error: {ex}");
				return InternalServerError();
			}

		}
	}
}
