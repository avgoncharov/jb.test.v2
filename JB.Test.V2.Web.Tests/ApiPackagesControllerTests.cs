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
	public sealed class ApiPackagesControllerTests
	{
		private readonly IUnityContainer _container;

		public ApiPackagesControllerTests()
		{
			_container = new UnityContainer();
			
			var pkgsRepo = new PackagesRepositoryFake();
			_container.RegisterInstance<IPackagesRepositoryReader>(pkgsRepo);
			_container.RegisterInstance<IPackagesRepositoryWriter>(pkgsRepo);			
		}


		[Fact]
		public async Task GetByFilterAsyncNotFoundTest()
		{
			var controller = _container.Resolve<ApiPackagesController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Post, $"~/api/packages");
			var result = await controller.GetByFilterAsync(null, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.NotFound, r.StatusCode);


			controller = _container.Resolve<ApiPackagesController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Post, $"~/api/packages");
			result = await controller.GetByFilterAsync(new Filter(), CancellationToken.None);
			r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.NotFound, r.StatusCode);
		}


		[Theory, AutoData]
		public async Task GetVersionAsyncNotFoundTest(string id, string version)
		{
			var controller = _container.Resolve<ApiPackagesController>();
			controller.Request = new HttpRequestMessage(HttpMethod.Post, $"~/api/packages/{id}/{version}");
			var result = await controller.GetVersionAsync(id, version, CancellationToken.None);
			var r = await result.ExecuteAsync(CancellationToken.None);
			Assert.Equal(System.Net.HttpStatusCode.NotFound, r.StatusCode);			
		}
	}
}
