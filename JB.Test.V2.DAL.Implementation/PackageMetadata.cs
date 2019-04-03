using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	/// <summary>
	/// Class of a package's metadata.
	/// </summary>
	internal sealed class PackageMetadata
	{
		/// <summary>
		/// Gets / sets Id of the package.
		/// </summary>
		public string Id { get; set; }


		/// <summary>
		/// Gets / sets version of the package.
		/// </summary>
		public string Version { get; set; }


		/// <summary>
		/// Gets / sets description of the package.
		/// </summary>
		public string Description { get; set; }


		/// <summary>
		/// Gets / sets raw metadata of the package.
		/// </summary>
		public string Metadata { get; set; }
	}
}
