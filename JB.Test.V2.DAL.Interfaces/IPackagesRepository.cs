using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// Interface of a packages repository.
	/// </summary>
	public interface IPackagesRepository
	{
		/// <summary>
		/// Adds a package to the repository.
		/// </summary>
		/// <param name="package">Package.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task AddPackageAsync(IPackage package, CancellationToken token);


		/// <summary>
		/// Lookings for all versions of the package by id.
		/// </summary>
		/// <param name="id">Id of the package.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>Set of all package versions.</returns>
		Task<IEnumerable<IPackage>> FindByIdAsync(string id, CancellationToken token);


		/// <summary>
		/// Gets stream of a package's data.
		/// </summary>
		/// <param name="id">Id of the package.</param>
		/// <param name="version">Version of the package.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>Data stream.</returns>
		Task<Stream> GetStreamForAsync(string id, string version, CancellationToken token);


		/// <summary>
		/// Lookings for packages by filter.
		/// </summary>
		/// <param name="filter">Filter.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>All packages which are satisfy filter.</returns>
		Task<IEnumerable<IPackage>> FindAllByFilter(Filter filter, CancellationToken token);


		/// <summary>
		/// Synchronizes repository with file system.
		/// </summary>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task SyncRepositoryAsync(CancellationToken token);
	}
}
