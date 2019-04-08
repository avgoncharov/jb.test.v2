using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Interfaces;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using Unity;
using Unity.Injection;
using Xunit;
using JB.Test.V2.DAL.Implementation.Extensions;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public sealed class PackagesRepositoryTests : RepositoryTestsBase
	{		  		
		private const string OutPath = "TestData\\out";
		private const string InputPath = "TestData\\input";
		private const string TestInputPath = "TestData";

		//GetPackageAsync Ok
		//GetPackageAsync not found
		//FindAllByFilterAsync Null | Empty Fltr 
		//FindAllByFilterAsync Fltr By id,  by ver, by desc, and id/ver/des
		

		[Fact]
		public async Task AddPackageTest()
		{
			var factory = Container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync(Path.Combine(TestInputPath, "Moq.4.8.2.nupkg"), CancellationToken.None);

			var repo = Container.Resolve<IPackagesRepository>();
			await repo.AddPackageAsync(package, CancellationToken.None);

			var path = Path.Combine(OutPath, $"{package.Id}.{package.Version}{Constants.NugetPackageExtension}");

			Assert.True(File.Exists(path));
			var newPackage = await factory.CreateFromFileAsync(path, CancellationToken.None);

			Assert.Equal(package.Id, newPackage.Id);
			Assert.Equal(package.Version, newPackage.Version);
			Assert.Equal(package.Description, newPackage.Description);
			Assert.Equal(package.Metadata, newPackage.Metadata);

			var store = Container.Resolve<NugetStore>();

			var version = SemVersionParser.Parse(package.Version);

			var storePackage = await store.Packages.Where(p =>
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
		public async Task FileSaveFailedTest()
		{
			var factory = Container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync(Path.Combine(TestInputPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var repo = Container.Resolve<IPackagesRepository>();

			var packageMock = new Moq.Mock<IPackage>();
			packageMock.SetupGet(m => m.Id).Returns(() => package.Id);
			packageMock.SetupGet(m => m.Version).Returns(() => package.Version);
			packageMock.SetupGet(m => m.Description).Returns(() => package.Description);
			packageMock.SetupGet(m => m.Metadata).Returns(() => package.Metadata);

			var msg = "314";
			packageMock.Setup(m => m.Open()).Throws(new InvalidOperationException(msg));

			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async()=> await repo.AddPackageAsync(packageMock.Object, CancellationToken.None));
			Assert.Equal(msg, exception.Message);

			var store = Container.Resolve<NugetStore>();
			var failedPackageDto = await store
				.Packages
				.FirstOrDefaultAsync(itr => itr.Id == package.Id && itr.Version == package.Version);

			Assert.Null(failedPackageDto);
			Assert.False(File.Exists(Path.Combine(OutPath, package.BuildFileName())));
		}


		[Fact]
		public async Task AddPackageTwiceTest()
		{
			var factory = Container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync(Path.Combine(TestInputPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var repo = Container.Resolve<IPackagesRepository>();
			await repo.AddPackageAsync(package, CancellationToken.None);

			await Assert.ThrowsAsync<PackageIsAlreadyExistException>(async () => await repo.AddPackageAsync(package, CancellationToken.None));
		}

		[Fact]
		public async Task AddPackageWithDifferentVersionsTest()
		{
			var factory = Container.Resolve<IPackagesFactory>();
			var package482 = await factory.CreateFromFileAsync(
				Path.Combine(TestInputPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var package483 = await factory.CreateFromFileAsync(
				Path.Combine(TestInputPath, "Moq.4.8.3.nupkg"),
				CancellationToken.None);

			var repo = Container.Resolve<IPackagesRepository>();
			await repo.AddPackageAsync(package482, CancellationToken.None);
			await repo.AddPackageAsync(package483, CancellationToken.None);

			var store = Container.Resolve<NugetStore>();

			var storePackages = await store.Packages.Where(p => p.Id == package482.Id).ToArrayAsync();
			Assert.Equal(2, storePackages.Length);

			var latestCount = storePackages.Count(itr => itr.Latest && itr.VersionSuffix == "");
			Assert.Equal(1, latestCount);

			var latest = storePackages.FirstOrDefault(itr => itr.Latest && itr.VersionSuffix == "");
			Assert.NotNull(latest);

			Assert.Equal(package483.Version, latest.Version);
		}


		[Fact]
		public async Task SyncRepositoryTest()
		{
			var factory = Container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync(
				Path.Combine(TestInputPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var inputPackage = await factory.CreateFromFileAsync(
				Path.Combine(InputPath, "AutoFixture.4.8.0.nupkg"),
				CancellationToken.None);

			var repo = Container.Resolve<IPackagesRepository>();

			//Add package to system.
			await repo.AddPackageAsync(package, CancellationToken.None);

			//Check that it in system (as file)
			var path = Path.Combine(OutPath, package.BuildFileName());
			Assert.True(File.Exists(path));

			var store = Container.Resolve<NugetStore>();

			//Check that it in system (as item in db)
			var packageDto = await store
				.Packages
				.FirstOrDefaultAsync(itr => itr.Id == package.Id && itr.Version == package.Version);
			Assert.NotNull(packageDto);


			//Check that there are files in input dir.
			Assert.NotEmpty(Directory.EnumerateFiles(InputPath));

			//Delete added package from system (as file).
			File.Delete(path);
			Assert.False(File.Exists(path));

			//Sync repo with file system and input dir.
			await repo.SyncRepositoryAsync(CancellationToken.None);

			//Check that deleted from file system package is deleted from db.
			var deletedPackageDto = await store
				.Packages
				.FirstOrDefaultAsync(itr => itr.Id == package.Id && itr.Version == package.Version);

			Assert.Null(deletedPackageDto);					 

			//Check that input dir is empty.
			Assert.Empty(Directory.EnumerateFiles(InputPath));

			//Check that package from input dir is added to system.
			var addedPackageDto = await store
				.Packages
				.FirstOrDefaultAsync(itr => itr.Id == inputPackage.Id && itr.Version == inputPackage.Version);

			Assert.NotNull(addedPackageDto);
			var addedPath = Path.Combine(OutPath, inputPackage.BuildFileName());
			Assert.True(File.Exists(addedPath));
		}


		protected override void Dispose(bool disposing)
		{	
			var store = new NugetStore();
			store.Database.ExecuteSqlCommand("truncate table dbo.Packages");

			if(Directory.Exists(OutPath))
				Directory.Delete(OutPath, true);

			base.Dispose(disposing);
		}


		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();
			Container.RegisterType<IPackagesFactory, PackagesFactory>();
			Container.RegisterType<IPackagesRepository, PackagesRepository>(new InjectionConstructor(OutPath, InputPath));
		}
	}
}
