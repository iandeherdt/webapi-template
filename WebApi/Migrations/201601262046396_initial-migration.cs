namespace WebApiTemplate.ResourceAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialmigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "main.product",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 512),
                        createdat = c.DateTime(nullable: false),
                        createdby = c.String(),
                        modifiedat = c.DateTime(nullable: false),
                        modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("main.product");
        }
    }
}
