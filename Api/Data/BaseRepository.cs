
using Api.Models;
using Neo4j.Driver;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Api.Data
{
    public class BaseRepository<TEntity> 
        : IBaseRepository<TEntity> where TEntity : EntityDataBase
    {
        readonly IContextDb _contextDb;
        public BaseRepository(IContextDb contextDb, IRepositoryConsult<TEntity> _repositoryConsult)
        {
            _contextDb = contextDb;
            RepositoryConsult = _repositoryConsult; 
        }

        public IRepositoryConsult<TEntity> RepositoryConsult { get; }

        string GetPropJsonAttr(PropertyInfo propertyInfo)
        {
          var jsonPropertyAttribute =   propertyInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                                                    .FirstOrDefault() as JsonPropertyAttribute;
          if (jsonPropertyAttribute == null)
                return propertyInfo.Name;

            return jsonPropertyAttribute.PropertyName!;
        }

        public async Task AddColectionAsync(List<TEntity> entities )
        {
            var queryBuilder = new StringBuilder();
            var parameters = new Dictionary<string, object?>();
            queryBuilder.AppendLine(" CREATE ");
            var countEntities = 0; 
            for (int i = 0; i < entities.Count(); i++)
            {
                countEntities++;
                var entity = entities[i];
                var propertyInfo = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                // Criar o nó Person
                queryBuilder.AppendLine($" (e{i}:{entity.GetType().Name}" + " {");
                var countProperts = 0;
                foreach (var property in propertyInfo)
                {
                    countProperts++;
                    parameters.Add($"{GetPropJsonAttr(property)}_{i}", property.GetValue(entity));
                    if (countProperts == propertyInfo.Count())
                        queryBuilder.AppendLine(GetPropJsonAttr(property)   + ":" + "$" + GetPropJsonAttr(property)+"_"+i );
                    else
                        queryBuilder.AppendLine(GetPropJsonAttr(property) + ": " + "$" + GetPropJsonAttr(property) + "_" + i + ","); 
                }
                if (countEntities == entities.Count())
                    queryBuilder.AppendLine("})");
                else
                    queryBuilder.AppendLine("}),");
            }
            queryBuilder.AppendLine("RETURN *");
            string query = queryBuilder.ToString();
            await ExecuteQueryAsync(query, parameters, async (cursor) => {
                while (await cursor.FetchAsync())
                {
                    await foreach (var record in cursor)
                    {
                        // Processar o resultado conforme necessário
                        Console.WriteLine(record);
                    }
                }
            });
        }
        public async Task AddAsync(TEntity entidade)
        {
            var type = entidade.GetType();
            var query = $" CREATE (n:{type.Name}"+"{";
            var props = type.GetProperties();
            foreach (var prop in props)
            {
            
               query += GetPropJsonAttr(prop) + ":" + "$" + GetPropJsonAttr(prop) + ",";
            }
            query = query.Substring(0, query.Length-1) + "}) RETURN n";
            await ExecuteQueryAsync(query, entidade, async  (cursor) => {
                while (await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                   // Console.WriteLine($"Created node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                }
            });
        }

        public void Dispose() => GC.SuppressFinalize(this);

        public async Task ExecuteQueryAsync(string query,
                                           object? parameters = null,
                                          Func<IResultCursor, Task>? action = null)
        {
            var session = _contextDb.Driver.AsyncSession();
            try
            {
                var cursor = await session.RunAsync(query, parameters);
                await action?.Invoke(cursor);
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
            var type = entidade.GetType();
            var query = "MATCH (n:"+ type.Name+" {id: $id}) DELETE n";
            await ExecuteQueryAsync(query, entidade, async (cursor) => {
                while ( await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                    // Console.WriteLine($"Created node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                }
            });
        }

        public async Task UpdateAsync(TEntity entidade, IEnumerable<string> fieldsUpdate)
        {
            var type = entidade.GetType();
            var query = "MATCH (n:"+ type.Name +" {id: $id}) SET ";
           
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (fieldsUpdate != null && fieldsUpdate.Any())
                {
                    if (!fieldsUpdate.Contains(GetPropJsonAttr(prop)))
                        continue;
                }
                if (GetPropJsonAttr(prop) == "id")
                    continue;
                query += "n." + GetPropJsonAttr(prop) + " = " + "$" + GetPropJsonAttr(prop) + ",";
            }
            query = query.Substring(0, query.Length - 1) + " RETURN n";
            await ExecuteQueryAsync(query, entidade, async  (cursor) => {
                while  ( await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                    // Console.WriteLine($"Created node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                }
            });
        }
    }
}
