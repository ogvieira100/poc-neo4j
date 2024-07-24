﻿namespace Api.Data
{
    public interface IBaseRepository<TEntity> : IDisposable where TEntity : EntityDataBase
    {
        public IRepositoryConsult<TEntity> RepositoryConsult { get; protected set; }
        Task AddAsync(TEntity entidade);
        Task UpdateAsync(TEntity customer);
        Task RemoveAsync(TEntity customer);
    }
}
