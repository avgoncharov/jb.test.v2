using JB.Test.V2.DAL.Implementation.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public abstract class RepositoryTestsBase : IDisposable
	{
		public RepositoryTestsBase()
		{
			Container = new UnityContainer();
			
			ConfigureContainer();

			AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB"));
		}
			

		public void Dispose()
		{
			Dispose(true);
		}


		protected IUnityContainer Container { get; }


		protected virtual void ConfigureContainer()
		{
			Container.RegisterType<NugetStore>();
		}


		protected virtual void Dispose(bool disposing)
		{
			Container?.Dispose();
		}
	}
}
