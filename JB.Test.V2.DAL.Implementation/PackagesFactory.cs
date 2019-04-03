using JB.Test.V2.DAL.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	/// <summary>
	/// The class of a factory of packages.
	/// </summary>
	public sealed class PackagesFactory : IPackagesFactory
	{			
		public async Task<IPackage> CreateFromFileAsync(string path, CancellationToken token)
		{
			if(string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Path can't be null or empty.");
			}

			if(File.Exists(path) != true)
			{
				throw new FileNotFoundException($"File '{path}' not found.");
			}
							   
			var metadata = await PackageDataExtractor.ExtractAync(path, token);

			return new Package(path, metadata);
		}
	}
}
