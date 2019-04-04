namespace JB.Test.V2.DAL.Implementation
{
	internal sealed class SemVersion
	{
		public readonly int Major;
		public readonly int Minor;
		public readonly int Patch;
		public readonly string VersionSuffix;

		public SemVersion(int major, int minor, int patch, string suffix)
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			VersionSuffix = suffix;
		}
	}
}
