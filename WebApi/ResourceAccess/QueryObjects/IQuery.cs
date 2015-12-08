using WebApi.ResourceAccess.Context;

namespace WebApi.ResourceAccess.QueryObjects
{
    public interface IQuery<T>
    {
        IQueryResult<T> Execute(EntityContext session);
    }
}