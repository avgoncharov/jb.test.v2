using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("index.json")]
	public class IndexController : ApiController
	{
		[Route(), HttpGet]
		public IHttpActionResult Get()
		{
			var template = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/bin/IndexTemplate.json"));

			var stream = new MemoryStream(Encoding.UTF8.GetBytes(template.Replace("[host]", Url.Content("~/"))));

			var resonse = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StreamContent(stream)
			};

			resonse.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = Path.GetFileName("index.json")
			};

			resonse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			resonse.Content.Headers.ContentLength = stream.Length;
			return ResponseMessage(resonse);
		}
	}
}
