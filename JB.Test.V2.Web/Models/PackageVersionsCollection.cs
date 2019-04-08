using Newtonsoft.Json;

namespace JB.Test.V2.Web.Models
{
	public class PackageVersionsCollection
	{
		[JsonProperty(PropertyName = "@id")]
		public string DId { get; set; }


		[JsonProperty(PropertyName = "count")]
		public int Count { get; set; }


		[JsonProperty(PropertyName = "items")]
		public PackageVersion[] Items { get; set; }


		[JsonProperty(PropertyName = "lower")]
		public string Lower { get; set; }


		[JsonProperty(PropertyName = "upper")]
		public string Upper { get; set; }
	}
}