using JB.Test.V2.DAL.Implementation;
using JB.Test.V2.DAL.Interfaces;
using System;
using System.Configuration;
using System.IO;
using Unity;
using Unity.Injection;

namespace JB.Test.V2.Web
{
	/// <summary>
	/// Specifies the Unity configuration for the main container.
	/// </summary>
	public static class UnityConfig
	{
		#region Unity Container
		private static Lazy<IUnityContainer> container =
		  new Lazy<IUnityContainer>(() =>
		  {
			  var container = new UnityContainer();
			  RegisterTypes(container);
			  return container;
		  });

		/// <summary>
		/// Configured Unity Container.
		/// </summary>
		public static IUnityContainer Container => container.Value;
		#endregion

		/// <summary>
		/// Registers the type mappings with the Unity container.
		/// </summary>
		/// <param name="container">The unity container to configure.</param>
		/// <remarks>
		/// There is no need to register concrete types such as controllers or
		/// API controllers (unless you want to change the defaults), as Unity
		/// allows resolving a concrete type even if it was not previously
		/// registered.
		/// </remarks>
		public static void RegisterTypes(IUnityContainer container)
		{
			var output = CreatAbsolutePath(ConfigurationManager.AppSettings["OutputDir"]);

			container.RegisterType<IPackagesFactory, PackagesFactory>();

			container.RegisterSingleton<IPackagesRepositoryWriter, PackagesRepositoryWriter>(new InjectionConstructor(
				output,
				CreatAbsolutePath(ConfigurationManager.AppSettings["InputDir"])));

			container.RegisterType<IPackagesRepositoryReader, PackagesRepositoryReader>(
				new InjectionConstructor(output));

			container.RegisterSingleton<INugetUserRepositoryWriter, NugetUserRepositoryWriter>();

			container.RegisterType<INugetUserRepositoryReader, NugetUserRepositoryReader>();
		}


		private static string CreatAbsolutePath(string lastPart)
		{
			return Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Pkgs"), lastPart);
		}
	}
}