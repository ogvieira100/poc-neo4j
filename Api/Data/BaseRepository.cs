
using Neo4j.Driver;

namespace Api.Data
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityDataBase
    {
        readonly IContextDb _contextDb;
        public BaseRepository(IContextDb contextDb)
        {
            _contextDb = contextDb; 
        }
        IRepositoryConsult<TEntity> IBaseRepository<TEntity>.RepositoryConsult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task AddAsync(TEntity entidade)
        {
            var query = $"CREATE (n:{nameof(entidade)}";

           //await _contextDb.

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task ExecuteQueryAsync(string query, 
                                           object? parameters = null,
                                           Action<IResultCursor>? action = null)
        {
           var  session = _contextDb.Driver.AsyncSession();
            try
            {
               var cursor =  await session.RunAsync(query, parameters);
                action?.Invoke(cursor);     
            }
            catch (Exception e){

                throw;
            }
            finally {
                await session.CloseAsync();
            }
        }

        public Task RemoveAsync(TEntity customer)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity customer)
        {
            throw new NotImplementedException();
        }
    }
}
