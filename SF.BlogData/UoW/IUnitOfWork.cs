﻿using SF.BlogData.Repository;


namespace SF.BlogData;

public interface IUnitOfWork : IDisposable
{
    int SaveChanges(bool ensureAutoHistory = false);
    IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
}
