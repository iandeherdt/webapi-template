﻿using System.Collections.Generic;
using WebApi.Entities;
using WebApi.ResourceAccess.QueryObjects;

namespace WebApi.ResourceAccess.Repositories
{
    public interface IRepository
    {
        T Get<T>(int id) where T : Entity;
        IEnumerable<T> GetAll<T>() where T : Entity;
        int Count<T>() where T : Entity;
        T Add<T>(T entity) where T : Entity;
        T Update<T>(T entity) where T : Entity;
        void Remove<T>(T entity) where T : Entity;

        IQueryResult<T> Query<T>(IQuery<T> query) where T : Entity;
    }
}
