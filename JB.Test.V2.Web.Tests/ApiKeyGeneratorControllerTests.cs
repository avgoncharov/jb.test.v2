using AutoFixture.Xunit2;
using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.Web.Controllers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Xunit;

namespace JB.Test.V2.Web.Tests
{
	public sealed class ApiKeyGeneratorControllerTests
	{
		private readonly IUnityContainer _container;
		public ApiKeyGeneratorControllerTests()
		{
			_container = new UnityContainer();
			var repo = new NugetUserRepositoryFake();
			_container.RegisterInstance<INugetUserRepositoryWriter>(repo);
			_container.RegisterInstance<INugetUserRepositoryReader>(repo);
		}


		[Theory, AutoData]
		public async Task GetByNameAsyncNotFoundTest(string str)
		{
			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-name/{str}");
			var result = await controller.GetByNameAsync(str, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.NotFound, r.StatusCode);
		}


		[Theory, AutoData]
		public async Task GetByNameAsyncTest(string apiKey, string name)
		{
			var writer = _container.Resolve<INugetUserRepositoryWriter>();
			await writer.CreateUserAsync(apiKey, name, CancellationToken.None);

			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-name/{name}");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			var result = await controller.GetByNameAsync(name, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.OK, r.StatusCode);
		}


		[Fact]
		public async Task GetByNameBadInputAsyncTest()
		{
			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-name/");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			var result = await controller.GetByNameAsync(null, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.BadRequest, r.StatusCode);

			controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-name/");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			result = await controller.GetByNameAsync(string.Empty, CancellationToken.None);
			r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.BadRequest, r.StatusCode);
		}


		[Theory, AutoData]
		public async Task GetBytApiKeyAsyncNotFoundTest(string str)
		{
			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-api-key/{str}");

			var result = await controller.GetBytApiKeyAsync(str, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.NotFound, r.StatusCode);
		}


		[Theory, AutoData]
		public async Task GetBytApiKeyAsyncTest(string apiKey, string name)
		{
			var writer = _container.Resolve<INugetUserRepositoryWriter>();
			await writer.CreateUserAsync(apiKey, name, CancellationToken.None);

			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-api-key/{apiKey}");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			var result = await controller.GetBytApiKeyAsync(apiKey, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.OK, r.StatusCode);
		}


		[Fact]
		public async Task GetByApiKeyBadInputAsyncTest()
		{
			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-api-key/");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			var result = await controller.GetBytApiKeyAsync(null, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.BadRequest, r.StatusCode);

			controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/find-by-api-key/");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			result = await controller.GetBytApiKeyAsync(string.Empty, CancellationToken.None);
			r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.BadRequest, r.StatusCode);
		}


		[Theory, AutoData]
		public async Task CreateUserAsyncTest(string name)
		{
			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/generate-new-for/{name}");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			var result = await controller.PostAsync(name, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.OK, r.StatusCode);

			var reader = _container.Resolve<INugetUserRepositoryReader>();
			var user = await reader.FindUserByNameAsync(name, CancellationToken.None);
			Assert.NotNull(user);
		}


		[Theory, AutoData]
		public async Task CreateUserAsyncTwiceTest(string name)
		{
			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/generate-new-for/{name}");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			var result = await controller.PostAsync(name, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.OK, r.StatusCode);

			Assert.True(r.TryGetContentValue<INugetUser>(out var user));
			var apiKey = user.ApiKey;


			controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/generate-new-for/{name}");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			result = await controller.PostAsync(name, CancellationToken.None);
			r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.OK, r.StatusCode);

			Assert.True(r.TryGetContentValue<INugetUser>(out var user2));
			var apiKey2 = user2.ApiKey;

			Assert.Equal(apiKey, apiKey2);

			var reader = _container.Resolve<INugetUserRepositoryReader>();
			var userInRepo = await reader.FindUserByNameAsync(name, CancellationToken.None);
			Assert.NotNull(userInRepo);
			Assert.Equal(user.Name, userInRepo.Name);
			Assert.Equal(user.ApiKey, userInRepo.ApiKey);
		}


		[Fact]
		public async Task CreateUserAsyncBadRequestTest()
		{
			var controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/generate-new-for/");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			var result = await controller.PostAsync(null, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.BadRequest, r.StatusCode);

			controller = _container.Resolve<ApiKeyGeneratorController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Get, $"~/api/api-key-generator/generate-new-for/");
			controller.Configuration = new System.Web.Http.HttpConfiguration();
			result = await controller.PostAsync(string.Empty, CancellationToken.None);
			r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.BadRequest, r.StatusCode);
		}
	}
}
