using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("nuget")]
	public class NugetController : ApiController
	{

		//https://docs.microsoft.com/en-us/nuget/api/overview
		[Route("FindPackagesById()"), HttpGet]
		public IHttpActionResult Get([FromUri]string id)
		{
			var path = HostingEnvironment.MapPath("~/test.xml");
			var data = File.ReadAllText(path);

			return Ok(data);
		}

		[Route("v3-flatcontainer/{id}/{version}/str"), HttpGet]
		public IHttpActionResult Get22(string id, string version, string str)
		{
			var path = HostingEnvironment.MapPath("~/test.xml");
			var data = File.ReadAllText(path);

			return Ok(data);
		}

		[Route("Packages(Id={id},Version={version})/Download"), HttpGet, HttpHead]
		public IHttpActionResult Get(string id, string version)
		{
			var path = HostingEnvironment.MapPath("~/test.xml");
			var data = File.ReadAllText(path);

			return Ok(data);
		}


		[Route, HttpPut]
		public IHttpActionResult Put()
		{	
			return Ok();
		}
	}
}
