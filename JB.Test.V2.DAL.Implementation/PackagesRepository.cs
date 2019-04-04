using JB.Test.V2.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using JB.Test.V2.DAL.Implementation.DB.DTOs;

namespace JB.Test.V2.DAL.Implementation
{
	/// <inheritdoc/>
	public sealed class PackagesRepository : IPackagesRepository
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
		private readonly PackagesStore _store = new PackagesStore();
		private readonly string _rootPath;


		public PackagesRepository(string rootPath)
		{
			_rootPath = rootPath;
		}


		/// <inheritdoc/>
		public async Task AddPackageAsync(IPackage package, CancellationToken token)
		{
			await _semaphore.WaitAsync(token);
			try
			{
				var version = SemVersionParser.Parse(package.Version);
				if (await _store.Packages.AnyAsync(itr =>
					itr.Id == package.Id
					&& itr.Major == version.Major
					&& itr.Minor == version.Minor
					&& itr.Patch == version.Patch
					&& itr.VersionSuffix == version.VersionSuffix,
					token))
				{
					throw new PackageIsAlreadyExistException($"{package.Id}.{package.Version}");
				}

				using (var transaction = _store.Database.BeginTransaction())
				{
					var latestPackage = await FindLatest(package.Id, version, token);

					var newPackage = new PackageDto
					{
						Id = package.Id,
						Version = package.Version,

						Major = version.Major,
						Minor = version.Minor,
						Patch = version.Patch,
						VersionSuffix = version.VersionSuffix,

						Latest = IsLatest(latestPackage, version),

						Description = package.Description,
						Metadata = package.Metadata
					};

					if (latestPackage != null && newPackage.Latest)
						latestPackage.Latest = false; 

					_store.Packages.Add(newPackage);

					await _store.SaveChangesAsync(token);
					await SaveToFileSystemAsync(package, token);
					transaction.Commit();
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}


		/// <inheritdoc/>
		public async Task<IEnumerable<IPackage>> FindAllByFilter(Filter filter, CancellationToken token)
		{
			var query = _store.Packages.AsQueryable();

			if (filter == null || filter.IsEmpty())
			{
				return await query.Where(itr=>itr.Latest && itr.VersionSuffix == "").Select(itr => Map(itr)).ToListAsync(token);
			}

			if (string.IsNullOrWhiteSpace(filter.Id) != true)
			{
				query = query.Where(itr => itr.Id.Contains(filter.Id));
			}

			if (string.IsNullOrWhiteSpace(filter.Version) != true)
			{
				query = query.Where(itr => itr.Version.Contains(filter.Version));
			}

			if (string.IsNullOrWhiteSpace(filter.Description) != true)
			{
				query = query.Where(itr => itr.Description.Contains(filter.Description));
			}

			return await query.Select(itr => Map(itr)).ToListAsync(token);
		}


		/// <inheritdoc/>
		public async Task<IEnumerable<IPackage>> FindByIdAsync(string id, CancellationToken token)
		{
			return await _store.Packages.Where(itr =>
				itr.Latest && itr.Id == id && itr.VersionSuffix == "")
				.Select(itr=>Map(itr))
				.ToListAsync(token);
		}


		/// <inheritdoc/>
		public async Task<Stream> GetStreamForAsync(string id, string version, CancellationToken token)
		{		
			var buf = await _store.Packages.Where(itr =>
					itr.Id == id && itr.Version == version)
				.Select(itr =>
					new Package(
						Path.Combine(_rootPath, $"{itr.Id}.{itr.Version}{Constants.NugetPackageExtension}"),
						new PackageMetadata
						{
							Id = itr.Id,
							Version = itr.Version,
							Description = itr.Description,
							Metadata = itr.Metadata
						}))
				.FirstOrDefaultAsync(token);

			return buf?.Open();
		}


		private IPackage Map(PackageDto packageDto)
		{
			return new Package(
				Path.Combine(_rootPath,
					$"{packageDto.Id}.{packageDto.Version}{Constants.NugetPackageExtension}"),
				new PackageMetadata
				{
					Id = packageDto.Id,
					Version = packageDto.Version,
					Description = packageDto.Description,
					Metadata = packageDto.Metadata
				});
		}


		private static bool IsLatest(PackageDto latestPackage, SemVersion version)
		{
			if (latestPackage == null) return true;

			if (version.Major < latestPackage.Major) return false;
			if (version.Major > latestPackage.Major) return true;

			if (version.Minor < latestPackage.Minor) return false;
			if (version.Minor > latestPackage.Minor) return true;

			if (version.Patch < latestPackage.Patch) return false;
			if (version.Patch > latestPackage.Patch) return true;

			return false;
		}


		private async Task<PackageDto> FindLatest(string id, SemVersion version, CancellationToken token)
		{
			var release = string.IsNullOrWhiteSpace(version.VersionSuffix);
			var query = _store.Packages.Where(itr => itr.Id == id && itr.Latest);
			query = string.IsNullOrWhiteSpace(version.VersionSuffix) 
				? query.Where(itr => itr.VersionSuffix == "")
				: query.Where(itr => itr.VersionSuffix != "");

			return await query.FirstOrDefaultAsync(token);
		}

		private async Task SaveToFileSystemAsync(IPackage package, CancellationToken token)
		{
			if (Directory.Exists(_rootPath) != true)
			{
				Directory.CreateDirectory(_rootPath);
			}

			var path = Path.Combine(_rootPath, $"{ package.Id}.{package.Version}{Constants.NugetPackageExtension}");
			const int bufLenght = 1024;
			byte[] buf = new byte[bufLenght];
			int currLen = 0;

			using (var stream = package.Open())
			using (var outStream = File.OpenWrite(path))
			{
				do
				{
					currLen = await stream.ReadAsync(buf, 0, bufLenght, token);
					if (currLen > 0)
					{
						await outStream.WriteAsync(buf, 0, currLen, token);
					}

				} while (currLen == bufLenght);
			}
		}
	}
}
