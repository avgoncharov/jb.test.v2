using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("api/api-key-generator")]
	public class ApiKeyGeneratorController : ApiController
	{
		private readonly INugetUserRepositoryWriter _nugetUserRepositoryWriter;
		private readonly INugetUserRepositoryReader _nugetUserRepositoryReader;
		private readonly ILogger _logger = Log.Logger.ForContext<ApiKeyGeneratorController>();


		public ApiKeyGeneratorController(
			INugetUserRepositoryWriter nugetUserRepository,
			INugetUserRepositoryReader nugetUserRepositoryReader)
		{
			_nugetUserRepositoryWriter = nugetUserRepository ?? throw new ArgumentNullException(nameof(nugetUserRepository));
			_nugetUserRepositoryReader = nugetUserRepositoryReader ?? throw new ArgumentNullException(nameof(nugetUserRepositoryReader));
		}


		[Route("find-by-name/{userName}"), HttpGet]
		public async Task<IHttpActionResult> GetByNameAsync(string userName, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(userName))
			{
				return BadRequest("User name can't be empty.");
			}

			var user = await _nugetUserRepositoryReader.FindUserByNameAsync(userName, token);
			       
			return user != null ? Ok(user) : NotFound() as IHttpActionResult;
		}


		[Route("find-by-api-key/{apiKey}"), HttpGet]
		public async Task<IHttpActionResult> GetBytApiKeyAsync(string apiKey, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				return BadRequest("ApiKey can't be empty.");
			}

			var user = await _nugetUserRepositoryReader.FindUserByApiKeyAsync(apiKey, token);

			return user != null ? Ok(user) : NotFound() as IHttpActionResult;
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
				var user = await _nugetUserRepositoryReader.FindUserByNameAsync(userName, token);
				if (user != null)
				{
					return Ok(user);
				}

				var guid = Guid.NewGuid().ToString();
				await _nugetUserRepositoryWriter.CreateUserAsync(guid, userName, token);

				var newUser = await _nugetUserRepositoryReader.FindUserByApiKeyAsync(guid, token);

				return newUser != null ? Ok(newUser) : BadRequest() as IHttpActionResult;
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
