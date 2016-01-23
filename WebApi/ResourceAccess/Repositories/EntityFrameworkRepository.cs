using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using WebApiTemplate.Entities;
using WebApiTemplate.ResourceAccess.Context;
using WebApiTemplate.ResourceAccess.Exceptions;
using WebApiTemplate.ResourceAccess.QueryObjects;
using WebApiTemplate.ResourceAccess.Uow;

namespace WebApiTemplate.ResourceAccess.Repositories
{
    public class EntityFrameworkRepository : IRepository
    {
        private string _errorMessage = string.Empty;

        protected virtual EntityContext Context
        {
            get
            {
                if (UnitOfWork.Current == null)
                    throw new Exception("Repository should be used within a unit of work");
                return UnitOfWork.Current.Context;
            }
        }

        public T Get<T>(int id) where T : Entity
        {
            return Context.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll<T>() where T : Entity
        {
            return Context.Set<T>();
        }

        public int Count<T>() where T : Entity
        {
            return Context.Set<T>().Count();
        }

        public T Add<T>(T entity) where T : Entity
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                return Context.Set<T>().Add(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += string.Format("Property: {0} Error: {1}",
                            validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
        }

        public T Update<T>(T entity) where T : Entity
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                var current = Context.Set<T>().Find(entity.Id);
                if (current == null)
                {
                    throw new EntityNotFoundException(typeof(T).Name, entity.Id);
                }
                Context.Entry(current).CurrentValues.SetValues(entity);
                return current;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += Environment.NewLine + string.Format("Property: {0} Error: {1}",
                            validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                throw new Exception(_errorMessage, dbEx);
            }
        }

        public void Remove<T>(T entity) where T : Entity
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                Context.Set<T>().Remove(entity);
                Context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _errorMessage += Environment.NewLine + string.Format("Property: {0} Error: {1}",
                            validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw new Exception(_errorMessage, dbEx);
            }
        }

        public IQueryResult<T> Query<T>(IQuery<T> query) where T : Entity
        {
            return query.Execute(Context);
        }
    }
}