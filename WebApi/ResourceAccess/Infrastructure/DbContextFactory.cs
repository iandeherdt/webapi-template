﻿using WebApiTemplate.ResourceAccess.Context;

namespace WebApiTemplate.ResourceAccess.Infrastructure
{
    public interface IDbContextFactory
    {
        IEntityContext GetContext();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly EntityContext _context;

        public DbContextFactory()
        {
            _context = new EntityContext();
        }

        public IEntityContext GetContext()
        {
            return _context;
        }
    }
}