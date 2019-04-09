using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.Web.App_Start;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace JB.Test.V2.Web
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AppDomain.CurrentDomain.SetData(
				"DataDirectory", 
				Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data"), "DB"));			

			LogConfig.Configure();
			GlobalConfiguration.Configure(WebApiConfig.Register);

			SyncWithFileSystem();

			//TODO: Sync repo on start.
			//TODO: from right index.json.
		}

		private void SyncWithFileSystem()
		{
			var repo =  GlobalConfiguration
				.Configuration
				.DependencyResolver
				.GetService(typeof(IPackagesRepositoryWriter)) as IPackagesRepositoryWriter;

			if(repo != null)
			{
				var awaiter = repo.SyncRepositoryAsync(CancellationToken.None)
					.ConfigureAwait(false)
					.GetAwaiter();

				awaiter.GetResult();
			}
		}
	}
}
