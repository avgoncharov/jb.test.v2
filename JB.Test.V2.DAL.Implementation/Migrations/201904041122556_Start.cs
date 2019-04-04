namespace JB.Test.V2.DAL.Implementation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Packages",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 800),
                        Major = c.Int(nullable: false),
                        Minor = c.Int(nullable: false),
                        Patch = c.Int(nullable: false),
                        VersionSuffix = c.String(nullable: false, maxLength: 20),
                        Description = c.String(),
                        Metadata = c.String(nullable: false),
                        Version = c.String(nullable: false, maxLength: 800),
                        Latest = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.Major, t.Minor, t.Patch, t.VersionSuffix });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Packages");
        }
    }
}
