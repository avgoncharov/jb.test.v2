using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Interfaces;
using System.IO;
using Unity;
using Unity.Injection;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public sealed class PackagesRepositoryReaderTests : RepositoryTestsBase
	{	
		//GetPackageAsync Ok
		//GetPackageAsync not found
		//FindAllByFilterAsync Null | Empty Fltr 
		//FindAllByFilterAsync Fltr By id,  by ver, by desc, and id/ver/des

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
