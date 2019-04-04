using JB.Test.V2.DAL.Implementation.DB.DTOs;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace JB.Test.V2.DAL.Implementation.DB.Configurations
{
	internal sealed class NugetUsersConfiguration: EntityTypeConfiguration<NugetUserDto>
	{
		internal NugetUsersConfiguration()
		{
			ToTable("NugetUsers");

			Property(itr => itr.ApiKey)
				.HasMaxLength(512)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			HasKey(itr => itr.ApiKey);

			Property(itr => itr.Name)
				.HasMaxLength(512)
				.IsRequired();
		}
	}
}
