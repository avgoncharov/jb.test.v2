using System.Data.Entity;
using JB.Test.V2.DAL.Implementation.DB.Configurations;
using JB.Test.V2.DAL.Implementation.DB.DTOs;

namespace JB.Test.V2.DAL.Implementation.DB
{
	/// <summary>
	/// A class of a packages store.
	/// </summary>
	internal sealed class NugetStore: DbContext
	{
		public NugetStore() : base("FeedDb")
		{
		}

		public DbSet<PackageDto> Packages { get; set; }
		public DbSet<NugetUserDto> Users { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new PackagesConfiguration());
			modelBuilder.Configurations.Add(new NugetUsersConfiguration());
		}
	}
}
