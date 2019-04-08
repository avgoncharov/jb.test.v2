using System;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("v3/registration3")]
	public class RegistrationController : ApiController
	{
		[Route("{id}/index.json"), HttpGet]
		public IHttpActionResult Get(string id)
		{
			throw new NotImplementedException();
		}
	}
}
