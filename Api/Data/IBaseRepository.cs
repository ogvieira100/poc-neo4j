using Neo4j.Driver;

namespace Api.Data
{
    public interface IBaseRepository<TEntity> : IDisposable where TEntity : EntityDataBase
    {
         Task AddAsync(TEntity entidade);
         Task UpdateAsync(TEntity customer, IEnumerable<string> fieldsUpdate);
         Task RemoveAsync(TEntity customer);
         IRepositoryConsult<TEntity> RepositoryConsult { get; }
         Task ExecuteQueryAsync(string query,
                            object? parameters = null,
                            Func<IResultCursor, Task>? action = null);
        Task AddColectionAsync(List<TEntity> entities);
    }
}
