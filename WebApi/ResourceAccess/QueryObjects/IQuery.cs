using WebApiTemplate.ResourceAccess.Context;

namespace WebApiTemplate.ResourceAccess.QueryObjects
{
    public interface IQuery<T>
    {
        IQueryResult<T> Execute(EntityContext session);
    }
}