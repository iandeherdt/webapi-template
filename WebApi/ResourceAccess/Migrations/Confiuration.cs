using System.Data.Entity.Migrations;
using WebApi.ResourceAccess.Context;

namespace WebApi.ResourceAccess.Migrations
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
