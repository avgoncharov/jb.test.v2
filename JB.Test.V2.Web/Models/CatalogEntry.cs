using Newtonsoft.Json;

namespace JB.Test.V2.Web.Models
{
	public class CatalogEntry
	{
		[JsonProperty(PropertyName = "@id")]
		public string DId { get; set; }


		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }


		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }
	}
}