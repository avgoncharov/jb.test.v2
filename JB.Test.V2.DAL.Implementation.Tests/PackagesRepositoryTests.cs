using JB.Test.V2.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Xunit;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public class PackagesRepositoryTests
	{
		private readonly IUnityContainer _container;


		/// <summary>
		/// Creates a new instance of the test suite.
		/// </summary>
		public PackagesRepositoryTests()
		{
			_container = new UnityContainer();
			_container.RegisterType<IPackagesFactory, PackagesFactory>();
			_container.RegisterType<IPackagesRepository, PackagesRepository>(new InjectionConstructor(new object[] { "TestData\\out" }));
		}


		[Fact]
		public async Task AddPackageTest()
		{
			var factory = _container.Resolve<IPackagesFactory>();
			var package = await factory.CreateFromFileAsync("TestData\\test_file", CancellationToken.None);

			var repo = _container.Resolve<IPackagesRepository>();
			await repo.AddPackageAsync(package, CancellationToken.None);

			//Проверять через загрузку из out.
			//Отчистить out.
		}
	}
}
