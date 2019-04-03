using JB.Test.V2.DAL.Interfaces;
using System.IO;

namespace JB.Test.V2.DAL.Implementation
{
	/// <inheritdoc />
	internal sealed class Package : IPackage
	{
		private readonly string _path;

		internal Package(string path, PackageMetadata metadata)
		{
			_path = path;
			Id = metadata.Id;
			Version = metadata.Version;
			Description = metadata.Description;
			Metadata = metadata.Metadata;			
		}


		/// <inheritdoc />
		public string Id { get; internal set; }


		/// <inheritdoc />
		public string Version { get; internal set; }


		/// <inheritdoc />
		public string Description { get; internal set; }


		/// <inheritdoc />
		public string Metadata { get; internal set; }


		/// <inheritdoc />
		public Stream Open()
		{
			return File.Open(_path, FileMode.Open, FileAccess.Read);
		}
	}
}
