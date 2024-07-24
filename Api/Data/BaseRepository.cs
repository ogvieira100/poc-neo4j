
using Neo4j.Driver;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Api.Data
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityDataBase
    {
        readonly IContextDb _contextDb;
        public BaseRepository(IContextDb contextDb)
        {
            _contextDb = contextDb;
        }

        public IRepositoryConsult<TEntity> RepositoryConsult { get; }

        public async Task AddAsync(TEntity entidade)
        {
            var type = entidade.GetType();
            var query = $" CREATE (n:{type.Name.ToLower()}"+"{";
            var props = type.GetProperties();
            foreach (var prop in props)
            {
               query += prop.Name + ":" + "$" + prop.Name + ",";
            }
            query = query.Substring(0, query.Length-1) + "}) RETURN n";
            var objSerial =  Newtonsoft.Json.JsonConvert.SerializeObject(entidade);
            var objDeSerial =  Newtonsoft.Json.JsonConvert.DeserializeObject<TEntity>(objSerial);      
            await ExecuteQueryAsync(query, objDeSerial, async (cursor) => {
                while (cursor.FetchAsync().Result)
                {
                    var node = cursor.Current["n"].As<INode>();
                   // Console.WriteLine($"Created node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                }
            });
        }

        public void Dispose() => GC.SuppressFinalize(this);

        public async Task ExecuteQueryAsync(string query,
                                           object? parameters = null,
                                          Action<IResultCursor>?  action = null )
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

        public async Task RemoveAsync(TEntity entidade)
        {
            var query = "MATCH (n:"+nameof(TEntity).ToLower()+" {id: $id}) DELETE n";
            var objSerial = Newtonsoft.Json.JsonConvert.SerializeObject(entidade);
            var objDeSerial = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(query);
            await ExecuteQueryAsync(query, objDeSerial, async (cursor) => {
                while (await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                    // Console.WriteLine($"Created node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                }
            });
        }

        public async Task UpdateAsync(TEntity entidade, IEnumerable<string> fieldsUpdate)
        {
            var query = "MATCH (n:"+nameof(TEntity).ToLower() +" {id: $id}) SET ";
            var type = entidade.GetType();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (fieldsUpdate != null && fieldsUpdate.Any())
                {
                    if (!fieldsUpdate.Contains(prop.Name.ToLower()))
                        continue;
                }
                if (prop.Name.ToLower() == "id")
                    continue;
                query += "n." + prop.Name.ToLower() + "=" + "$" + prop.Name.ToLower() + ",";
            }
            query = query.Substring(0, query.Length - 1) + "}) RETURN n";
            var objSerial = Newtonsoft.Json.JsonConvert.SerializeObject(entidade);
            var objDeSerial = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(query);
            await ExecuteQueryAsync(query, objDeSerial, async (cursor) => {
                while (await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                    // Console.WriteLine($"Created node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                }
            });
        }
    }
}
