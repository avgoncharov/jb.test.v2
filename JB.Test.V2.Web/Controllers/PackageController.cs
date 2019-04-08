using JB.Test.V2.DAL.Implementation.Extensions;
using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("api/v2/package")]
	public class PackageController : ApiController
	{
		private const string ApiKeyHeader = "X-NUGET-APIKEY";
		private readonly INugetUserRepository _nugetUserRepository;
		private readonly IPackagesRepository _packagesRepository;
		private readonly IPackagesFactory _pacakgeFactory;
		private readonly ILogger _logger = Log.Logger.ForContext<PackageController>();


		public PackageController(
			INugetUserRepository nugetUserRepository,
			IPackagesRepository packagesRepository,
			IPackagesFactory pacakgeFactory)
		{
			_nugetUserRepository = nugetUserRepository ?? throw new ArgumentNullException(nameof(nugetUserRepository));
			_packagesRepository = packagesRepository ?? throw new ArgumentNullException(nameof(packagesRepository));
			_pacakgeFactory = pacakgeFactory ?? throw new ArgumentNullException(nameof(pacakgeFactory));
		}


		[Route("~/api/nuget/{id}/{version}/{suffix}")]
		public async Task<IHttpActionResult> GetAsync(
			string id, 
			string version, 
			string suffix, 
			CancellationToken token)
		{
			try
			{
				var result = await _packagesRepository.GetPackageAsync(id, version, token);
				if (result == null)
				{
					return NotFound();
				}

				var stream = result.Open();

				var resonse = new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StreamContent(stream)
				};

				resonse.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
				{
					FileName = Path.GetFileName(result.BuildFileName())
				};

				resonse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				resonse.Content.Headers.ContentLength = stream.Length;
				return ResponseMessage(resonse);
			}
			catch (ArgumentException ex)
			{
				_logger.Warning($"Get faield. Reason: {ex}");
				return BadRequest();
			}
			catch (Exception ex)
			{
				_logger.Error($"Get failed. Unexpected error: {ex}");
				return InternalServerError();
			}
		}


		[Route(""), HttpPut]
		public async Task<IHttpActionResult> Put(CancellationToken token)
		{
			var apiKye = GetApiKey();
			if(string.IsNullOrWhiteSpace(apiKye))
			{
				return Unauthorized();
			}

			try
			{

				if(await _nugetUserRepository.FindUserByApiKeyAsync(apiKye, token) == null)
				{
					_logger.Warning($"User by api key '{apiKye}' wasn't found in system.");
					return Unauthorized();
				}

				var temporaryFile = Path.GetTempFileName();
				using(var temporaryFileStream = File.Open(temporaryFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
				{
					if(Request.Content.IsMimeMultipartContent())
					{
						var multipartContents = await Request.Content.ReadAsMultipartAsync();
						await multipartContents.Contents.First().CopyToAsync(temporaryFileStream);
					}
					else
					{
						await Request.Content.CopyToAsync(temporaryFileStream);
					}
				}


				var package = await _pacakgeFactory.CreateFromFileAsync(temporaryFile, token);
				await _packagesRepository.AddPackageAsync(package, token);

				return Ok();
			}
			catch(ArgumentException ex)
			{
				_logger.Warning($"Put faield. Reason: {ex.Message}");
				return BadRequest();
			}
			catch(PackageIsAlreadyExistException ex)
			{
				_logger.Warning($"Put faield. Reason: {ex.Message}");
				return BadRequest();
			}
			catch(Exception ex)
			{
				_logger.Error($"Put failed. Unexpected error: {ex}");
				return InternalServerError();
			}
		}   


		private string GetApiKey()
		{
			if(Request.Headers.TryGetValues(ApiKeyHeader, out var values))
			{
				return values.FirstOrDefault();
			}

			return null;
		}
	}
}
