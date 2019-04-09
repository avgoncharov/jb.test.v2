using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// The interface of a packages repository for writing.
	/// </summary>
	public interface IPackagesRepositoryWriter
	{
		/// <summary>
		/// Adds the package to the repository.
		/// </summary>
		/// <param name="package">Package.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task AddPackageAsync(IPackage package, CancellationToken token);
			       

		/// <summary>
		/// Synchronizes repository with file system.
		/// </summary>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task SyncRepositoryAsync(CancellationToken token);
	}
}
