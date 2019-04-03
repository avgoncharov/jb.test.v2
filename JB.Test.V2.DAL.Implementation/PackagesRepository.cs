using JB.Test.V2.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	public sealed class PackagesRepository : IPackagesRepository
	{
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
		private readonly string _rootPath;


		public PackagesRepository(string rootPath)
		{
			_rootPath = rootPath;
		}


		public async Task AddPackageAsync(IPackage package, CancellationToken token)
		{
			await _semaphore.WaitAsync(token);
			try
			{
				await SaveToFileSistemAsync(package, token);
					throw new NotImplementedException();
			}
			finally
			{
				_semaphore.Release();
			}
		}
			

		public Task<IEnumerable<IPackage>> FindAllByFilter(Filter filter, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<IPackage>> FindByIdAsync(string id, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public Task<Stream> GetStreamForAsync(string id, string version, CancellationToken token)
		{
			throw new NotImplementedException();
		}


		private async Task SaveToFileSistemAsync(IPackage package, CancellationToken token)
		{
			if(Directory.Exists(_rootPath) != true)
			{
				Directory.CreateDirectory(_rootPath);
			}

			var path = Path.Combine(_rootPath, $"{ package.Id}.{package.Version}{Constants.NugetPackageExtension}");
			const int bufLenght = 1024;
			byte[] buf = new byte[bufLenght];
			int currLen = 0;

			using(var stream = package.Open())
			using(var outStream = File.OpenWrite(path))
			{
				currLen = await stream.ReadAsync(buf, 0, bufLenght, token);
				await outStream.WriteAsync(buf, 0, currLen, token);
			}
		}
	}
}
