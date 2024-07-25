using Neo4j.Driver;
using System.Linq.Expressions;

namespace Api.Data
{
    public interface IRepositoryConsult<TEntity> : IDisposable where TEntity : class
    {
        Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task ExecuteQueryAsync(string query,
                               object? parameters = null,
                               Func<IResultCursor, Task>? action = null);
    }
}
