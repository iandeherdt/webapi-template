using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using WebApiTemplate.Entities;
using WebApiTemplate.ResourceAccess.Attributes;
using WebApiTemplate.ResourceAccess.Infrastructure;

namespace WebApiTemplate.ResourceAccess.Context{
    public class EntityContext : DbContext, IEntityContext<DbContext>
    {
        protected DbContext _context = null;

        public EntityContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // PostgreSQL gebruikt schema 'public' - niet dbo.
            modelBuilder.HasDefaultSchema("main");

            // Lowercase
            modelBuilder.Types().Configure(c =>
            {
                c.ToTable(c.ClrType.Name.ToLower());
            });

            modelBuilder.Properties().Configure(c =>
            {
                c.HasColumnName(c.ClrPropertyInfo.Name.ToLower());
            });

            // Cascading deletes afzetten voor de veiligheid
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // Pluralizing table names afzetten
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Verwerk de HasPrecision attributes
            ProcessHasPrecisionAttributes(modelBuilder);
        }

        /// <summary>
        /// To add AuditInformation
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<Entity>().Where(p => p.State == EntityState.Added || p.State == EntityState.Modified))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.Now;
                        entry.Entity.CreatedBy = CurrentUser();
                        entry.Entity.ModifiedAt = DateTime.Now;
                        entry.Entity.ModifiedBy = CurrentUser();
                        break;
                    case EntityState.Modified:
                        entry.Entity.CreatedAt = entry.Property(p => p.CreatedAt).OriginalValue;
                        entry.Entity.CreatedBy = entry.Property(p => p.CreatedBy).OriginalValue;
                        entry.Entity.ModifiedAt = DateTime.Now;
                        entry.Entity.ModifiedBy = CurrentUser();
                        break;
                }               
            }
            return base.SaveChanges();
        }

        private void ProcessHasPrecisionAttributes(DbModelBuilder modelBuilder)
        {
            var classTypes = (from t in Assembly.GetAssembly(typeof (HasPrecisionAttribute)).GetTypes()
                where t.IsClass && t.Namespace == "Salga.Entities"
                select t).ToList();



            foreach (var classType in classTypes)
            {
                var propAttributes = classType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.GetCustomAttribute<HasPrecisionAttribute>() != null)
                    .Select(p => new {prop = p, attr = p.GetCustomAttribute<HasPrecisionAttribute>(true)});

                foreach (var propAttr in propAttributes)
                {
                    var entityConfig =
                        modelBuilder.GetType()
                            .GetMethod("Entity")
                            .MakeGenericMethod(classType)
                            .Invoke(modelBuilder, null);
                    ParameterExpression param = ParameterExpression.Parameter(classType, "c");
                    Expression property = Expression.Property(param, propAttr.prop.Name);
                    LambdaExpression lambdaExpression = Expression.Lambda(property, true,
                        new ParameterExpression[] {param});
                    DecimalPropertyConfiguration decimalConfig;
                    if (propAttr.prop.PropertyType.IsGenericType &&
                        propAttr.prop.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                    {
                        MethodInfo methodInfo =
                            entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[7];
                        decimalConfig =
                            methodInfo.Invoke(entityConfig, new[] {lambdaExpression}) as DecimalPropertyConfiguration;
                    }
                    else
                    {
                        MethodInfo methodInfo =
                            entityConfig.GetType().GetMethods().Where(p => p.Name == "Property").ToList()[6];
                        decimalConfig =
                            methodInfo.Invoke(entityConfig, new[] {lambdaExpression}) as DecimalPropertyConfiguration;
                    }

                    decimalConfig.HasPrecision(propAttr.attr.Precision, propAttr.attr.Scale);
                }
            }
        }

        public DbContext Context
        {
            get { return _context; }
        }

        private string CurrentUser()
        {
            return Thread.CurrentPrincipal.Identity.Name;
        }
    }
}