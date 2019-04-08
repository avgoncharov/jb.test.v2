using AutoFixture.Xunit2;
using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Xunit;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public sealed class PackagesRepositoryReaderTests : RepositoryTestsBase
	{

		[Theory, AutoData]
		public async Task GetPackageAsyncNotFoundTest(string id, string version)
		{
			var repoReader = Container.Resolve<IPackagesRepositoryReader>();
			var notFound = await repoReader.GetPackageAsync(id, version, CancellationToken.None);

			Assert.Null(notFound);
		}


		[Fact]
		public async Task GetPackageAsyncNormalFoundTest()
		{
			var factory = Container.Resolve<IPackagesFactory>();
			var expectedPackage = await factory.CreateFromFileAsync(Path.Combine(TestsConstants.TestInputPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var repo = Container.Resolve<IPackagesRepositoryWriter>();
			//Excpected that PackagesRepositoryWriterTests green.
			await repo.AddPackageAsync(expectedPackage, CancellationToken.None);

			var repoReader = Container.Resolve<IPackagesRepositoryReader>();
			var actual = await repoReader.GetPackageAsync(expectedPackage.Id, expectedPackage.Version, CancellationToken.None);

			Assert.NotNull(actual);
			Assert.Equal(expectedPackage.Id, actual.Id);
			Assert.Equal(expectedPackage.Version, actual.Version);
			Assert.Equal(expectedPackage.Description, actual.Description);
			Assert.Equal(expectedPackage.Metadata, actual.Metadata);
		}


		[Theory, AutoData]
		public async Task GetPackageAsyncWithBadInputTest(string str)
		{
			var repoReader = Container.Resolve<IPackagesRepositoryReader>();
			await Assert.ThrowsAsync<ArgumentException>(async ()=> await repoReader.GetPackageAsync(null, str, CancellationToken.None));
			await Assert.ThrowsAsync<ArgumentException>(async () => await repoReader.GetPackageAsync(string.Empty, str, CancellationToken.None));
			await Assert.ThrowsAsync<ArgumentException>(async () => await repoReader.GetPackageAsync(str, null, CancellationToken.None));
			await Assert.ThrowsAsync<ArgumentException>(async () => await repoReader.GetPackageAsync(str, string.Empty, CancellationToken.None));			
		}


		[Theory, AutoData]
		public async Task FindAllByFilterAsyncNotFoundTest(string id, string version)
		{
			var factory = Container.Resolve<IPackagesFactory>();
			var expectedPackage = await factory.CreateFromFileAsync(Path.Combine(TestsConstants.TestInputPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var repo = Container.Resolve<IPackagesRepositoryWriter>();
			//Excpected that PackagesRepositoryWriterTests green.
			await repo.AddPackageAsync(expectedPackage, CancellationToken.None);

			var repoReader = Container.Resolve<IPackagesRepositoryReader>();
			var actual = await repoReader.FindAllByFilterAsync(new Filter { Id = id}, CancellationToken.None);
			Assert.Empty(actual);

			actual = await repoReader.FindAllByFilterAsync(new Filter { Version = version }, CancellationToken.None);
			Assert.Empty(actual);
		}


		[Fact]
		public async Task FindAllByFilterAsyncNormalTest()
		{
			var factory = Container.Resolve<IPackagesFactory>();

			var expectedPackage = await factory.CreateFromFileAsync(
				Path.Combine(TestsConstants.TestInputPath, "Moq.4.8.2.nupkg"),
				CancellationToken.None);

			var expectedPackage2 = await factory.CreateFromFileAsync(
				Path.Combine(TestsConstants.TestInputPath, "Moq.4.8.3.nupkg"),
				CancellationToken.None);

			var expected = new[] { expectedPackage, expectedPackage2 };

			var repo = Container.Resolve<IPackagesRepositoryWriter>();
			//Excpected that PackagesRepositoryWriterTests green.
			foreach(var itr in expected)
			{
				var inner = itr;
				await repo.AddPackageAsync(inner, CancellationToken.None);
			}

			var repoReader = Container.Resolve<IPackagesRepositoryReader>();
			var actual = (await repoReader.FindAllByFilterAsync(null, CancellationToken.None)).ToList().AsReadOnly();
			Assert.NotEmpty(actual);

			CompareCollections(expected, actual);

			actual = (await repoReader.FindAllByFilterAsync(new Filter(), CancellationToken.None)).ToList().AsReadOnly();
			Assert.NotEmpty(actual);
			CompareCollections(expected, actual);												     

			actual = (await repoReader.FindAllByFilterAsync(new Filter { Version= expectedPackage2.Version }, CancellationToken.None)).ToList().AsReadOnly();
			Assert.Single(actual);
			var actualPkg = actual.First();
			Assert.Equal(expectedPackage2.Id, actualPkg.Id);
			Assert.Equal(expectedPackage2.Version, actualPkg.Version);

			actual = (await repoReader.FindAllByFilterAsync(new Filter { Version = "4.8" }, CancellationToken.None)).ToList().AsReadOnly();
			Assert.NotEmpty(actual);
			CompareCollections(expected, actual);

			actual = (await repoReader.FindAllByFilterAsync(new Filter { Id = expectedPackage.Id }, CancellationToken.None)).ToList().AsReadOnly();
			Assert.NotEmpty(actual);
			CompareCollections(expected, actual);
		}


		private static void CompareCollections(IReadOnlyCollection<IPackage> expected, IReadOnlyCollection<IPackage> actual)
		{
			Assert.Equal(expected.Count, actual.Count);

			foreach(var itr in expected)
			{
				Assert.NotNull(actual.FirstOrDefault(a =>
					a.Id == a.Id
					&& a.Version == itr.Version
					&& a.Description == itr.Description
					&& a.Metadata == itr.Metadata));
			}
		}


		protected override void Dispose(bool disposing)
		{
			var store = new NugetStore();
			store.Database.ExecuteSqlCommand("truncate table dbo.Packages");

			if(Directory.Exists(TestsConstants.OutPath))
				Directory.Delete(TestsConstants.OutPath, true);

			base.Dispose(disposing);
		}


		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();
			Container.RegisterType<IPackagesFactory, PackagesFactory>();
			Container.RegisterType<IPackagesRepositoryWriter, PackagesRepositoryWriter>(
				new InjectionConstructor(TestsConstants.OutPath, TestsConstants.InputPath));
			Container.RegisterType<IPackagesRepositoryReader, PackagesRepositoryReader>(
				new InjectionConstructor(TestsConstants.OutPath));
		}
	}	
}
