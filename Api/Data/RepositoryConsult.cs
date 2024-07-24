using Api.Util;
using Neo4j.Driver;
using System.Linq.Expressions;

namespace Api.Data
{
    public class RepositoryConsult<TEntity> : IRepositoryConsult<TEntity> where TEntity : class
    {
        readonly IContextDb _contextDb;
        public RepositoryConsult(IContextDb contextDb)
        {
            _contextDb = contextDb;
        }

        public void Dispose() => GC.SuppressFinalize(this);


        public async Task ExecuteQueryAsync(string query,
                                    object? parameters = null,
                                    Action<IResultCursor>? action = null)
        {
            var session = _contextDb.Driver.AsyncSession();
            try
            {
                var cursor = await session.RunAsync(query, parameters);
                action?.Invoke(cursor);
            }
            catch (Exception e)
            {

                throw;
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var listReturn = new List<TEntity>();
            var query = $" MATCH (n:{nameof(TEntity).ToLower()}) RETURN n ";
            var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
            var type = instance?.GetType();
            var props = type?.GetProperties();

            await ExecuteQueryAsync(query, null, async (cursor) =>
            {
                await foreach (var record in cursor)
                {
                    var node = record["n"].As<INode>();
                    if (props != null && props.Any())
                    {
                        var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
                        foreach (var prop in props)
                        {
                            node.TryGet<object>(prop.Name.ToLower(), out var res);
                            if (res != null)
                                prop.SetValue(instance, res);
                        }
                        listReturn.Add(instance);
                    }
                }
            });
            return listReturn;
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
            var type = instance?.GetType();
            var props = type?.GetProperties();
            var query = " MATCH (n:" + nameof(TEntity).ToLower() + " {id: $id}) RETURN n ";
            var par = new { id = id.ToString() };
            await ExecuteQueryAsync(query, par, async (cursor) =>
            {
                while (await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                    if (props != null && props.Any())
                    {
                        foreach (var prop in props)
                        {
                            node.TryGet<object>(prop.Name.ToLower(), out var res);
                            if (res != null)
                                prop.SetValue(instance, res);
                        }
                    }
                }
            });
            return instance;
        }

        public async Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate)
        {

            var conditions = UtilClass.ExtractConditions(predicate);
           
            var query = "";

            throw new NotImplementedException();
        }
    }
}
