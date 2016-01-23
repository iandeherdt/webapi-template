using System;
using System.Data;
using System.Runtime.Serialization;
using WebApiTemplate.ResourceAccess.Context;

namespace WebApiTemplate.ResourceAccess.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        EntityContext Context { get; }

        void SaveChanges();
    }

    public class UnitOfWork : IUnitOfWork
    {
        public EntityContext Context { get; private set; }

        public static IUnitOfWork Current
        {
            get { return Container.Resolve<IRequestState>().Get<IUnitOfWork>(); }
            private set { Container.Resolve<IRequestState>().Store(value); }
        }

        private UnitOfWork(bool transactional)
        {
            Context = new EntityContext();
            if (transactional) Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public static IUnitOfWork Start()
        {
            return Start(false);
        }

        private static IUnitOfWork Start(bool transactional)
        {
            if (Current != null)
                throw new UnitOfWorkException("you cannot start more that one unit of work per request");
            Current = new UnitOfWork(transactional);
            return Current;
        }

        public static IUnitOfWork StartWithoutTransaction()
        {
            return Start(false);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
            Current = null;
        }
    }

    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException()
        {
        }

        public UnitOfWorkException(string message)
            : base(message)
        {
        }

        public UnitOfWorkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnitOfWorkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}