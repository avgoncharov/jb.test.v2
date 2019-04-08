using Newtonsoft.Json;

namespace JB.Test.V2.Web.Models
{
	public class PackageVersion
	{
		[JsonProperty(PropertyName = "@id")]
		public string DId { get; set; }


		[JsonProperty(PropertyName = "catalogEntry")]
		public CatalogEntry CatalogEntry { get; set; }


		[JsonProperty(PropertyName = "packageContent")]
		public string PackageContent { get; set; }


		[JsonProperty(PropertyName = "registration")]
		public string Registration { get; set; }
	}
}