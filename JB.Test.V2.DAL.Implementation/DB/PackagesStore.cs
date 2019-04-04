using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JB.Test.V2.DAL.Implementation.DB.Configurations;
using JB.Test.V2.DAL.Implementation.DB.DTOs;

namespace JB.Test.V2.DAL.Implementation.DB
{
	/// <summary>
	/// A class of a packages store.
	/// </summary>
	internal sealed class PackagesStore: DbContext
	{
		public PackagesStore() : base("FeedDb")
		{
		}

		public DbSet<PackageDto> Packages { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new PackagesConfiguration());
		}
	}
}
