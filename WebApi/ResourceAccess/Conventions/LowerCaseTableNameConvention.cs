using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebApi.ResourceAccess.Conventions
{
    public class LowerCaseTableNameConvention : IStoreModelConvention<EntitySet>
    {
        public void Apply(EntitySet entitySet, DbModel model)
        {
            entitySet.Table = entitySet.Name.ToLower();
        }
    }
}
