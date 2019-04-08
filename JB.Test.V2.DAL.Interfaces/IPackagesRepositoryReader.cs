using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// The interface of packages repository only for reading.
	/// </summary>
	public interface IPackagesRepositoryReader
	{
		/// <summary>
		/// Lookings for all versions of the package by id.
		/// </summary>
		/// <param name="id">Id of the package.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>Set of all package versions.</returns>
		Task<IEnumerable<IPackage>> FindByIdAsync(string id, CancellationToken token);


		/// <summary>
		/// Gets package by id and version.
		/// </summary>
		/// <param name="id">Id of the package.</param>
		/// <param name="version">Version of the package.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>Data stream.</returns>
		Task<IPackage> GetPackageAsync(string id, string version, CancellationToken token);


		/// <summary>
		/// Lookings for packages by filter.
		/// </summary>
		/// <param name="filter">Filter.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>All packages which are satisfy filter.</returns>
		Task<IEnumerable<IPackage>> FindAllByFilterAsync(Filter filter, CancellationToken token);
	}
}
