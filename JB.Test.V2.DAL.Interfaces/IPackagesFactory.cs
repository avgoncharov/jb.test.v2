using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// The interface of factory of packages.
	/// </summary>
	public interface IPackagesFactory
	{
		/// <summary>
		/// Creates a package from file.
		/// </summary>
		/// <param name="path">File's path.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>A package from file.</returns>
		Task<IPackage> CreateFromFileAsync(string path, CancellationToken token);
	}
}
