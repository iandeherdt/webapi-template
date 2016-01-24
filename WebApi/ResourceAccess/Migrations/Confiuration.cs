using System.Data.Entity.Migrations;
using WebApiTemplate.ResourceAccess.Context;

namespace WebApiTemplate.ResourceAccess.Migrations
{
    public class Configuration : DbMigrationsConfiguration<EntityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(EntityContext context)
        {
        }
    }
}
