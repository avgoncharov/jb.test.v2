using JB.Test.V2.DAL.Implementation.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JB.Test.V2.DAL.Implementation.DB.DTOs;
using Xunit;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public class PackagesStoreTests
	{
		[Fact]
		public void Test()
		{
			var version = SemVersionParser.Parse("1.2.3-abc-bcd");

			//var store = new PackagesStore();
			//var count = store.Packages.Count();

			////Assert.Equal(0, count);

			//store.Packages.Add(new PackageDto
			//{
			//	Id = "xunit.a",
			//	Major = 1,
			//	Minor = 25,
			//	Patch = 0,
			//	VersionSuffix = "-rc",
			//	Metadata = "abc",
			//	Description = "xyz"
			//});

			//store.SaveChanges();

			//var result = store.Packages.Where(itr => itr.Id == "xunit.a")
			//	.OrderByDescending(itr => itr.Major)
			//	.ThenByDescending(itr => itr.Minor)
			//	.ThenByDescending(itr => itr.Patch);

			//var release = result.FirstOrDefault(itr => string.IsNullOrEmpty(itr.VersionSuffix));
			//var rc = result.FirstOrDefault(itr => string.IsNullOrEmpty(itr.VersionSuffix) != true);

			//count = store.Packages.Count();
			//Assert.Equal(1, count);

		}
	}
}
