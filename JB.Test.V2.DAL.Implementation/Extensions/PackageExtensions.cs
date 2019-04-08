using JB.Test.V2.DAL.Implementation.DB.DTOs;
using JB.Test.V2.DAL.Interfaces;
using System.IO;

namespace JB.Test.V2.DAL.Implementation.Extensions
{
	public static class PackageExtensions
	{
		internal static string BuildFileName(this PackageDto dto) => BuildFileName(dto.Id, dto.Version);
		public static string BuildFileName(this IPackage package) => BuildFileName(package.Id, package.Version);

		internal static IPackage MapToPackage(this PackageDto packageDto, string rootPath)
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


		internal static int CompareToByVersion(this PackageDto lhs, PackageDto rhs)
		{
			if(lhs == null && rhs == null)
			{
				return 0;
			}

			if(lhs == null)
			{
				return -1;
			}

			if(rhs == null)
			{
				return 1;
			}

			if(object.ReferenceEquals(lhs, rhs))
			{
				return 0;
			}

			if(lhs.Major < rhs.Major)
			{
				return -1;
			}
			if(lhs.Major > rhs.Major)
			{
				return 1;
			}


			if(lhs.Minor < rhs.Minor)
			{
				return -1;
			}
			if(lhs.Minor > rhs.Minor)
			{
				return 1;
			}


			if(lhs.Patch < rhs.Patch)
			{
				return -1;
			}
			if(lhs.Patch > rhs.Patch)
			{
				return 1;
			}

			return 0;
		}


		private static string BuildFileName(string id, string version) => $"{id}.{version}{Constants.NugetPackageExtension}";
	}
}
