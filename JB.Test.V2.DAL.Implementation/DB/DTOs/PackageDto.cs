namespace JB.Test.V2.DAL.Implementation.DB.DTOs
{
	/// <summary>
	/// A package dto class.
	/// </summary>
	internal sealed class PackageDto
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public string Metadata { get; set; }
		public string Version { get; set; }
		public bool Latest { get; set; }

		public int Major { get; set; }
		public int Minor { get; set; }
		public int Patch { get; set; }
		public string VersionSuffix { get; set; }
	}
}
