using System.Data.Entity;

namespace WebApi.ResourceAccess.Context
{
    public class NpgsqlDbConfiguration : DbConfiguration
    {
        public NpgsqlDbConfiguration()
        {
            SetDefaultConnectionFactory(new Npgsql.NpgsqlConnectionFactory());
            SetProviderFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
            SetProviderServices("Npgsql", Npgsql.NpgsqlServices.Instance);
        }
    }
}
