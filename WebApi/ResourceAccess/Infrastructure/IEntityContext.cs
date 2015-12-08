namespace WebApi.ResourceAccess.Infrastructure
{
    public interface IEntityContext
    {

    }

    public interface IEntityContext<out TContext> : IEntityContext
    {
        TContext Context { get; }
    }
}