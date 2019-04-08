using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.Web.Tests
{
	internal sealed class PackagesRepositoryFake : IPackagesRepositoryReader, IPackagesRepositoryWriter
	{
		private sealed class Package : IPackage
		{
			public string Id { get; set; }

			public string Version { get; set; }

			public string Description { get; set; }

			public string Metadata { get; set; }

			public Stream Open()
			{
				throw new NotImplementedException();
			}
		}

		private readonly object _lockObj = new object();
		private readonly List<IPackage> _packages = new List<IPackage>();

		internal static readonly string Id = "Id";
		internal static readonly string Version = "3.1.4";
		internal static readonly string Description = "Description 3.1.4";
		internal static readonly string Metadata = "Metadata 3.1.4";

		Task IPackagesRepositoryWriter.AddPackageAsync(IPackage package, CancellationToken token)
		{
			if(package == null)
			{
				throw new ArgumentNullException(nameof(package));
			}

			lock(_lockObj)
			{
				if(_packages.Any(itr => itr.Id == package.Id && itr.Version == package.Version))
				{
					throw new PackageIsAlreadyExistException();
				}

				_packages.Add(
					new Package
					{
						Id = package.Id,
						Version = package.Version,
						Description = package.Description,
						Metadata = package.Metadata
					});
			}

			return Task.CompletedTask;

		}

		Task<IEnumerable<IPackage>> IPackagesRepositoryReader.FindAllByFilterAsync(Filter filter, CancellationToken token)
		{
			lock(_lockObj)
			{
				var query = _packages.AsEnumerable();
				if(filter == null || filter.IsEmpty())
				{
					return Task.FromResult(query.ToList().AsReadOnly().AsEnumerable());
				}

				if(string.IsNullOrWhiteSpace(filter.Id) != true)
				{
					query = query.Where(itr => itr.Id.Contains(filter.Id));
				}
				if(string.IsNullOrWhiteSpace(filter.Version) != true)
				{
					query = query.Where(itr => itr.Version.Contains(filter.Version));
				}
				if(string.IsNullOrWhiteSpace(filter.Description) != true)
				{
					query = query.Where(itr => itr.Description.Contains(filter.Description));
				}

				return Task.FromResult(query.ToList().AsReadOnly().AsEnumerable());
			}
		}

		Task<IEnumerable<IPackage>> IPackagesRepositoryReader.FindByIdAsync(string id, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		Task<IPackage> IPackagesRepositoryReader.GetPackageAsync(string id, string version, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Id can't be null or empty.");
			}

			if(string.IsNullOrWhiteSpace(version))
			{
				throw new ArgumentException("Version can't be null or empty.");
			}

			lock(_lockObj)
			{
				return Task.FromResult(_packages.FirstOrDefault(itr => itr.Id == id && itr.Version == version));
			}

		}

		Task IPackagesRepositoryWriter.SyncRepositoryAsync(CancellationToken token)
		{
			throw new NotImplementedException();
		}
	}
}
