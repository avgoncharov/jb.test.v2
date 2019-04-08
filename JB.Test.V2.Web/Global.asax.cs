using JB.Test.V2.Web.App_Start;
using System;
using System.IO;
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

			//TODO: Sync repo on start.
			//TODO: from right index.json.
		}
	}
}
