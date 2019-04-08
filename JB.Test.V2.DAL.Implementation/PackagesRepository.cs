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
using System;
using Serilog;
using JB.Test.V2.DAL.Implementation.Extensions;

namespace JB.Test.V2.DAL.Implementation
{
	/// <inheritdoc/>
	public sealed class PackagesRepository : IPackagesRepository
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
		private readonly NugetStore _store = new NugetStore();
		private readonly string _rootPath;
		private readonly string _inputRoot;
		private readonly ILogger _logger = Log.Logger.ForContext<PackagesRepository>();


		public PackagesRepository(string rootPath, string inputRoot)
		{
			_rootPath = rootPath;
			_inputRoot = inputRoot;
		}


		/// <inheritdoc/>
		public async Task AddPackageAsync(IPackage package, CancellationToken token)
		{
			if(package == null)
			{
				throw new ArgumentNullException(nameof(package));
			}

			await _semaphore.WaitAsync(token);
			try
			{
				var version = SemVersionParser.Parse(package.Version);
				if(await _store.Packages.AnyAsync(itr =>
				       itr.Id == package.Id
				       && itr.Major == version.Major
				       && itr.Minor == version.Minor
				       && itr.Patch == version.Patch
				       && itr.VersionSuffix == version.VersionSuffix,
					token))
				{
					var msg = $"Package '{package.BuildFileName()}' is already exist in system.";
					_logger.Warning(msg);
					throw new PackageIsAlreadyExistException(msg);
				}

				using(var transaction = _store.Database.BeginTransaction())
				{
					var latestPackage = await FindLatestAsync(package.Id, version, token);

					var newPackage = new PackageDto
					{
						Id = package.Id,
						Version = package.Version,

						Major = version.Major,
						Minor = version.Minor,
						Patch = version.Patch,
						VersionSuffix = version.VersionSuffix,
						Description = package.Description,
						Metadata = package.Metadata
					};

					newPackage.Latest = newPackage.CompareToByVersion(latestPackage) > 0;

					if(latestPackage != null && newPackage.Latest)
					{
						latestPackage.Latest = false;
					}

					_store.Packages.Add(newPackage);

					await _store.SaveChangesAsync(token);
					await SaveToFileSystemAsync(package, token);
					transaction.Commit();
				}
			}
			catch(Exception ex)
			{
				_logger.Warning($"Unexpected exception. {ex}");
				throw;
			}
			finally
			{
				_semaphore.Release();
			}
		}


		/// <inheritdoc/>
		public async Task<IEnumerable<IPackage>> FindAllByFilterAsync(Filter filter, CancellationToken token)
		{
			var query = _store.Packages.AsQueryable();

			if(filter == null || filter.IsEmpty())
			{
				return (await query
					.Where(itr => itr.Latest && itr.VersionSuffix == "")
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


		/// <inheritdoc/>
		public async Task<IEnumerable<IPackage>> FindByIdAsync(string id, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Id can't be null or empty.");
			}

			return (await _store.Packages.Where(itr =>
				itr.Latest && itr.Id == id && itr.VersionSuffix == "").ToListAsync(token))
				.Select(itr => itr.MapToPackage(_rootPath));
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

			var buf = await _store.Packages.Where(itr =>
					itr.Id == id && itr.Version == version).FirstOrDefaultAsync(token);
				

			return buf?.MapToPackage(_rootPath);
		}


		public async Task SyncRepositoryAsync(CancellationToken token)
		{
			await SyncWithFileSystemAsync(token);
			await SyncWithInputAsync(token);
		}


		private async Task SyncWithFileSystemAsync(CancellationToken token)
		{
			var toDelete = new List<PackageDto>();
			foreach(var itr in _store.Packages)
			{
				if(File.Exists(Path.Combine(_rootPath, itr.BuildFileName())) != true)
				{
					toDelete.Add(itr);
				}
			}

			if(toDelete.Any())
			{
				await _semaphore.WaitAsync(token);
				try
				{
					_store.Packages.RemoveRange(toDelete);
					await _store.SaveChangesAsync(token);
				}
				catch(Exception ex)
				{
					_logger.Error($"Synchronization with file system failed. Reason: {ex}");
					throw;
				}
				finally
				{
					_semaphore.Release();
				}
			}
		}


		private async Task SyncWithInputAsync(CancellationToken token)
		{
			if(Directory.Exists(_inputRoot) != true)
			{
				return;
			}

			var factory = new PackagesFactory();
			foreach(var itr in Directory.EnumerateFiles(_inputRoot, "*.nupkg"))
			{
				try
				{
					var package = await factory.CreateFromFileAsync(itr, token);
					await AddPackageAsync(package, token);
					File.Delete(itr);
				}
				catch(Exception ex)
				{
					_logger.Error($"Can't add package from file '{itr}'. Resone: {ex}");
				}
			}
		}


		private async Task<PackageDto> FindLatestAsync(string id, SemVersion version, CancellationToken token)
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
			if(Directory.Exists(_rootPath) != true)
			{
				Directory.CreateDirectory(_rootPath);
			}

			var path = Path.Combine(_rootPath, package.BuildFileName());
			const int bufLenght = 1024;
			byte[] buf = new byte[bufLenght];
			int currLen = 0;

			using(var stream = package.Open())
			using(var outStream = File.OpenWrite(path))
			{
				do
				{
					currLen = await stream.ReadAsync(buf, 0, bufLenght, token);
					if(currLen > 0)
					{
						await outStream.WriteAsync(buf, 0, currLen, token);
					}

				} while(currLen == bufLenght);
			}
		}
	}
}
