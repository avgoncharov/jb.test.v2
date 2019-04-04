using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JB.Test.V2.DAL.Implementation.DB.DTOs;

namespace JB.Test.V2.DAL.Implementation.DB.Configurations
{
	internal sealed class PackagesConfiguration : EntityTypeConfiguration<PackageDto>
	{
		internal PackagesConfiguration()
		{
			ToTable("Packages");
			Property(itr => itr.Id)
				.HasMaxLength(800)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(itr => itr.Major)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(itr => itr.Minor)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(itr => itr.Patch)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(itr => itr.VersionSuffix)
				.HasMaxLength(20)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(itr => itr.Version)
				.HasMaxLength(800)
				.IsRequired();

			Property(itr => itr.Latest)
				.IsRequired();

			Property(itr => itr.Metadata).IsRequired();

			HasKey(itr => new {itr.Id, itr.Major, itr.Minor, itr.Patch, itr.VersionSuffix});


		}
	}
}
