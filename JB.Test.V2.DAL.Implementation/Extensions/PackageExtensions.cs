using JB.Test.V2.DAL.Implementation.DB.DTOs;
using JB.Test.V2.DAL.Interfaces;
using System.IO;

namespace JB.Test.V2.DAL.Implementation.Extensions
{
	internal static class PackageExtensions
	{
		internal static string BuildFileName(this PackageDto dto) => BuildFileName(dto.Id, dto.Version);
		internal static string BuildFileName(this IPackage package) => BuildFileName(package.Id, package.Version);

		internal static  IPackage MapToPackage(this PackageDto packageDto, string rootPath)
		{
			return new Package(
				Path.Combine(rootPath, packageDto.BuildFileName()),
				new PackageMetadata
				{
					Id = packageDto.Id,
					Version = packageDto.Version,
					Description = packageDto.Description,
					Metadata = packageDto.Metadata
				});
		}

		private static string BuildFileName(string id, string version)=> $"{id}.{version}{Constants.NugetPackageExtension}";
	}
}
