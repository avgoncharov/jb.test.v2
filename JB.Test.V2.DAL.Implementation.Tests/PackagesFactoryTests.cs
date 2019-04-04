using AutoFixture.Xunit2;
using JB.Test.V2.DAL.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Xunit;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	/// <summary>
	/// Test suite for a PackagesFactory.
	/// </summary>
	public sealed class PackagesFactoryTests
	{
		private readonly IUnityContainer _container;

		
		/// <summary>
		/// Creates a new instance of the test suite.
		/// </summary>
		public PackagesFactoryTests()
		{
			_container = new UnityContainer();
			_container.RegisterType<IPackagesFactory, PackagesFactory>();
		}


		/// <summary>
		/// Tests a normal execution.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task CreateFromFileAsyncOkTest()
		{
			var factory = _container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync("TestData\\Moq.4.8.2.nupkg", CancellationToken.None);

			Assert.NotNull(package);
			Assert.Equal("Moq", package.Id);
			Assert.Equal("4.8.2", package.Version);
			Assert.StartsWith("Moq is the most popular and friendly mocki", package.Description);
		}


		/// <summary>
		/// Tests an execution with null and epty path.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task CreateFromFileAsyncNullOrEmptyPathTest()
		{
			var factory = _container.Resolve<IPackagesFactory>();

			Exception ex = await Assert.ThrowsAsync<ArgumentException>(
				async ()=> await factory.CreateFromFileAsync(null, CancellationToken.None));

			Assert.Equal("Path can't be null or empty.", ex.Message);

			ex = await Assert.ThrowsAsync<ArgumentException>(
				async () => await factory.CreateFromFileAsync(string.Empty, CancellationToken.None));

			Assert.Equal("Path can't be null or empty.", ex.Message);
		}


		/// <summary>
		/// Tests an execution with bad path.
		/// </summary>
		/// <returns></returns>
		[Theory, AutoData]
		public async Task CreateFromFileAsyncBadPathTest(string path)
		{
			var factory = _container.Resolve<IPackagesFactory>();

			Exception ex = await Assert.ThrowsAsync<FileNotFoundException>(
				async () => await factory.CreateFromFileAsync(path, CancellationToken.None));

			Assert.EndsWith("not found.", ex.Message);
		}


		/// <summary>
		/// Tests an execution with bad data.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task CreateFromFileAsyncBadDataTest()
		{
			var factory = _container.Resolve<IPackagesFactory>();

			await Assert.ThrowsAsync<InvalidDataException>(
				async () => await factory.CreateFromFileAsync("TestData\\bad_data_test_file", CancellationToken.None));			
		}
	}
}
