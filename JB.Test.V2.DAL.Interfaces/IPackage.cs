using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// Interface of a package.
	/// </summary>
	public interface IPackage
	{
		/// <summary>
		/// Gets Id of the package.
		/// </summary>
		string Id { get; }


		/// <summary>
		/// Gets version of the package.
		/// </summary>
		string Version { get; }


		/// <summary>
		/// Gets description of the package.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets metadata of the package.
		/// </summary>
		string Metadata { get; }

		/// <summary>
		/// Gets stream of the package data.
		/// </summary>
		Task<Stream> OpenAsync(CancellationToken token);
	}
}
