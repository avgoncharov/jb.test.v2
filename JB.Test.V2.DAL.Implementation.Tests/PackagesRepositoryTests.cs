using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using Unity;
using Unity.Injection;
using Xunit;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public sealed class PackagesRepositoryTests : IDisposable
	{
		private readonly IUnityContainer _container;
		private const string OutPath = "TestData\\out";
		private const string InPath = "TestData";


		/// <summary>
		/// Creates a new instance of the test suite.
		/// </summary>
		public PackagesRepositoryTests()
		{
			_container = new UnityContainer();
			_container.RegisterType<IPackagesFactory, PackagesFactory>();
			_container.RegisterType<IPackagesRepository, PackagesRepository>(new InjectionConstructor(OutPath));
			_container.RegisterType<PackagesStore>();
		}	


		[Fact]
		public async Task AddPackageTest()
		{
			var factory = _container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync(Path.Combine(InPath, "Moq.4.8.2.nupkg"), CancellationToken.None);
									     
			var repo = _container.Resolve<IPackagesRepository>();
			await repo.AddPackageAsync(package, CancellationToken.None);

			var path = Path.Combine(OutPath, $"{package.Id}.{package.Version}{Constants.NugetPackageExtension}");

			Assert.True(File.Exists(path));
			var newPackage = await factory.CreateFromFileAsync(path, CancellationToken.None);

			Assert.Equal(package.Id, newPackage.Id);
			Assert.Equal(package.Version, newPackage.Version);
			Assert.Equal(package.Description, newPackage.Description);
			Assert.Equal(package.Metadata, newPackage.Metadata);

			var store = _container.Resolve<PackagesStore>();

			var version = SemVersionParser.Parse(package.Version);

			var storePackage =  await store.Packages.Where(p =>
				p.Id == package.Id
				&& p.Major == version.Major
				&& p.Minor == version.Minor
				&& p.Patch == version.Patch
				&& p.VersionSuffix == version.VersionSuffix).FirstOrDefaultAsync(CancellationToken.None);

			Assert.NotNull(storePackage);
			Assert.Equal(package.Description, storePackage.Description);
			Assert.Equal(package.Metadata, storePackage.Metadata);
			Assert.True(storePackage.Latest);
		}


		[Fact]
		public async Task AddPackageTwiceTest()
		{
			var factory = _container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync(Path.Combine(InPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var repo = _container.Resolve<IPackagesRepository>();
			await repo.AddPackageAsync(package, CancellationToken.None);

			await Assert.ThrowsAsync<PackageIsAlreadyExistException>(async ()=> await repo.AddPackageAsync(package, CancellationToken.None));
		}

		[Fact]
		public async Task AddPackageWithDifferentVersionsTest()
		{
			var factory = _container.Resolve<IPackagesFactory>();
			var package482 = await factory.CreateFromFileAsync(
				Path.Combine(InPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var package483 = await factory.CreateFromFileAsync(
				Path.Combine(InPath, "Moq.4.8.3.nupkg"),
				CancellationToken.None);

			var repo = _container.Resolve<IPackagesRepository>();
			await repo.AddPackageAsync(package482, CancellationToken.None);
			await repo.AddPackageAsync(package483, CancellationToken.None);

			var store = _container.Resolve<PackagesStore>();
			
			var storePackages = await store.Packages.Where(p => p.Id == package482.Id).ToArrayAsync();
			Assert.Equal(2, storePackages.Length);

			var latestCount = storePackages.Count(itr => itr.Latest && itr.VersionSuffix == "");
			Assert.Equal(1, latestCount);

			var latest = storePackages.FirstOrDefault(itr => itr.Latest && itr.VersionSuffix == "");
			Assert.NotNull(latest);

			Assert.Equal(package483.Version, latest.Version);  
		}

		public void Dispose()
		{
			var store = new PackagesStore();
			store.Database.ExecuteSqlCommand("truncate table dbo.Packages");

			if(Directory.Exists(OutPath))
				Directory.Delete(OutPath, true);

			_container?.Dispose();
		}
	}
}
