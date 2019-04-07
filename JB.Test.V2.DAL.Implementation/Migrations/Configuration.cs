namespace JB.Test.V2.DAL.Implementation.Migrations
{
	using System.Data.Entity.Migrations;
	
	internal sealed class Configuration : DbMigrationsConfiguration<JB.Test.V2.DAL.Implementation.DB.NugetStore>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(JB.Test.V2.DAL.Implementation.DB.NugetStore context)
		{
			
		}
	}
}
