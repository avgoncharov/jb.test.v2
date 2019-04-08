using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Implementation.Extensions;
using JB.Test.V2.DAL.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	public sealed class PackagesRepositoryReader: IPackagesRepositoryReader
	{			
		private readonly string _rootPath;		
		private readonly ILogger _logger = Log.Logger.ForContext<PackagesRepositoryReader>();


		public PackagesRepositoryReader(string rootPath)
		{
			_rootPath = rootPath;			
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<IPackage>> FindAllByFilterAsync(Filter filter, CancellationToken token)
		{
			using(var innerStore = new NugetStore()) 
			{
				var query = innerStore.Packages.AsQueryable();

				if(filter == null || filter.IsEmpty())
				{
					return (await query
						//.Where(itr => itr.Latest && itr.VersionSuffix == "")
						.ToListAsync(token))
						.Select(itr => itr.MapToPackage(_rootPath));

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

				return (await query.ToListAsync(token)).Select(itr => itr.MapToPackage(_rootPath));
			}
		}


		/// <inheritdoc/>
		public async Task<IEnumerable<IPackage>> FindByIdAsync(string id, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Id can't be null or empty.");
			}

			using(var innerStore = new NugetStore()) 
			{
				return (await innerStore.Packages.Where(itr =>
					itr.Latest && itr.Id == id && itr.VersionSuffix == "").ToListAsync(token))
					.Select(itr => itr.MapToPackage(_rootPath));
			}
		}


		/// <inheritdoc/>
		public async Task<IPackage> GetPackageAsync(string id, string version, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Id can't be null or empty.");
			}

			if(string.IsNullOrWhiteSpace(version))
			{
				throw new ArgumentException("Version can't be null or empty.");
			}

			using(var innerStore = new NugetStore()) 
			{

				var buf = await innerStore.Packages.Where(itr =>
					itr.Id == id && itr.Version == version).FirstOrDefaultAsync(token);


				return buf?.MapToPackage(_rootPath);
			}
		}
	}
}
