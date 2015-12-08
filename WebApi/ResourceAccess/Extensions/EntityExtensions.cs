using System;
using System.Linq;
using System.Linq.Expressions;


public static class EntityExtensions
{
    public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByValues)
        where TEntity : class
    {
        IQueryable<TEntity> returnValue = null;

        string orderPair = orderByValues.Trim().Split(',')[0];
        string command = orderPair.ToUpper().Contains("DESC") ? "OrderByDescending" : "OrderBy";

        var type = typeof (TEntity);
        var parameter = Expression.Parameter(type, "p");

        string propertyName = (orderPair.Split(' ')[0]).Trim();

        System.Reflection.PropertyInfo property;
        MemberExpression propertyAccess;

        if (propertyName.Contains('.'))
        {
            // support to be sorted on child fields. 
            String[] childProperties = propertyName.Split('.');

            property = SearchProperty(typeof (TEntity), childProperties[0]);
            propertyAccess = Expression.MakeMemberAccess(parameter, property);

            for (int i = 1; i < childProperties.Length; i++)
            {
                Type t = property.PropertyType;
                property = SearchProperty(t, childProperties[i]);

                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
        }
        else
        {
            property = SearchProperty(type, propertyName);

            propertyAccess = Expression.MakeMemberAccess(parameter, property);
        }

        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var resultExpression = Expression.Call(typeof (Queryable), command, new Type[] {type, property.PropertyType},

            source.Expression, Expression.Quote(orderByExpression));

        returnValue = source.Provider.CreateQuery<TEntity>(resultExpression);

        if (orderByValues.Trim().Split(',').Count() > 1)
        {
            // remove first item
            string newSearchForWords = orderByValues.Remove(0, orderByValues.IndexOf(',') + 1);
            return source.OrderBy(newSearchForWords);
        }

        return returnValue;
    }

    private static System.Reflection.PropertyInfo SearchProperty(Type type, string propertyName)
    {
        return type.GetProperties().FirstOrDefault(item => item.Name.ToLower() == propertyName.ToLower());
    }
}
