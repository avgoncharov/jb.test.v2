using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	internal static class SemVersionParser
	{
		private static readonly Regex _regex = new Regex("(?<major>[0-9]*)\\.(?<minor>[0-9]*)\\.(?<patch>[0-9]*)");

		public static SemVersion Parse(string version)
		{
			if (string.IsNullOrWhiteSpace(version)) throw new ArgumentException($"Version can't be empty.");

			var parts = version.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries);

			var versionPart = parts[0];

			var suffix = string.Empty;

			if (parts.Length > 1)
			{
				suffix = string.Join("-", parts.Skip(1));
			}

			var versionParts = _regex.Match(versionPart);

			var major = 0;
			var minor = 0;
			var patch = 0;

			if (versionParts.Success != true)
			{
				throw new InvalidDataException($"Bad version format. '{version}'");
			}

			if (versionParts.Groups["major"].Success != true)
			{
				throw new InvalidDataException($"Bad version format. '{version}'");
			}

			major = Convert.ToInt32(versionParts.Groups["major"].Value);

			if (versionParts.Groups["minor"].Success != true)
			{
				throw new InvalidDataException($"Bad version format. '{version}'");
			}

			minor = Convert.ToInt32(versionParts.Groups["minor"].Value);

			if (versionParts.Groups["patch"].Success != true)
			{
				throw new InvalidDataException($"Bad version format. '{version}'");
			}

			patch = Convert.ToInt32(versionParts.Groups["patch"].Value);

			return new SemVersion(major, minor, patch, suffix);
		}
	}
}
