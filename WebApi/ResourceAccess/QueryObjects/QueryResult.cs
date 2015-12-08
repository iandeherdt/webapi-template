using System.Collections.Generic;
using System.Linq;

namespace WebApi.ResourceAccess.QueryObjects
{
    public interface IQueryResult<T>
    {
        T UniqueResult();
        IEnumerable<T> List();
        int Count();
    }
    public class LinqResult<T> : IQueryResult<T>
    {
        private readonly IQueryable<T> _query;

        public LinqResult(IQueryable<T> query)
        {
            _query = query;
        }

        protected virtual IQueryable<T> ExpandQuery(IQueryable<T> query)
        {
            return query;
        }

        public T UniqueResult()
        {
            return ExpandQuery(_query).SingleOrDefault();
        }
        public IEnumerable<T> List()
        {
            return ExpandQuery(_query).ToList();
        }

        public int Count()
        {
            return ExpandQuery(_query).Count();
        }
    }
}