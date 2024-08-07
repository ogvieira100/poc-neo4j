using Api.Util;
using Neo4j.Driver;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Reflection;

namespace Api.Data
{
    public class RepositoryConsult<TEntity> : IRepositoryConsult<TEntity> where TEntity : class
    {
        readonly IContextDb _contextDb;
        public RepositoryConsult(IContextDb contextDb)
        {
            _contextDb = contextDb;
        }

        IDictionary<string, object?> Parametros<TValue>(TValue value) where TValue : class
        {
            var dictionary = new Dictionary<string, object?>();
            var typeValue = value.GetType();
            var propertyInfos = typeValue.GetProperties().ToList();
            // parameters.Add($"name_{i}", person.Name);
            /*padrão posição da propriedade com _0, _1*/
            foreach (var propertyInfo in propertyInfos)
            {
                var indexOf = propertyInfos.IndexOf(propertyInfo);
                dictionary.Add($"{GetPropJsonAttr(propertyInfo)}_{indexOf}", propertyInfo.GetValue(value));
            }
            return dictionary;
        }

        string GetPropJsonAttr(PropertyInfo propertyInfo)
        {
            var jsonPropertyAttribute = propertyInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                                                      .FirstOrDefault() as JsonPropertyAttribute;
            if (jsonPropertyAttribute == null)
                return propertyInfo.Name;

            return jsonPropertyAttribute.PropertyName!;
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

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var listReturn = new List<TEntity>();
            var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
            var type = instance?.GetType();
            var query = $" MATCH (n:{type.Name}) RETURN n ";


            var props = type?.GetProperties();

            await ExecuteQueryAsync(query, null, async (cursor) =>
            {
                var listRecord = await cursor.ToListAsync();
                if (listRecord != null)
                {
                    foreach (var record in listRecord)
                    {
                        var node = record["n"].As<INode>();
                        if (props != null && props.Any())
                        {
                            var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
                            foreach (var prop in props)
                            {
                                TratarClasseNodes(node, instance, prop);

                            }
                            listReturn.Add(instance);
                        }
                    }
                }
            });
            return listReturn;
        }

        void TratarClasseNodes(INode node, TEntity? instance, PropertyInfo prop)
        {
            node.TryGet<object>(GetPropJsonAttr(prop), out var res);
            if (res != null)
            {
                if (prop.PropertyType == typeof(Int32))
                {
                    if (Int32.TryParse(res.ToString(), out var resConv))
                    {
                        prop.SetValue(instance, resConv);
                    }
                }
                else if (prop.PropertyType == typeof(Int32?))
                {
                    if (Int32.TryParse(res.ToString(), out var resConv))
                    {
                        prop.SetValue(instance, resConv);
                    }
                }
                else if (prop.PropertyType == typeof(decimal))
                {
                    if (Decimal.TryParse(res.ToString(), out var resConv))
                    {
                        prop.SetValue(instance, resConv);
                    }
                }
                else if (prop.PropertyType == typeof(decimal?))
                {
                    if (Decimal.TryParse(res.ToString(), out var resConv))
                    {
                        prop.SetValue(instance, resConv);
                    }
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    var dateNeo = res as Neo4j.Driver.ZonedDateTime;
                    if (dateNeo != null)
                    {
                        prop.SetValue(instance, UtilClass.ConvertZonedDateTimeToDateTime(dateNeo));
                    }
                }
                else if (prop.PropertyType == typeof(DateTime?))
                {
                    var dateNeo = res as Neo4j.Driver.ZonedDateTime;
                    if (dateNeo != null)
                    {
                        prop.SetValue(instance, UtilClass.ConvertZonedDateTimeToDateTime(dateNeo));
                    }
                }
                else
                {
                    prop.SetValue(instance, res);
                }

            }
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
            var type = instance?.GetType();
            var props = type?.GetProperties();
            var query = " MATCH (n:" + type.Name + " {id: $id}) RETURN n ";
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
                            TratarClasseNodes(node, instance, prop);
                        }
                    }
                }
            });
            return instance;
        }

        public async Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var listSearch = new List<TEntity>();
            var conditions = UtilClass.ExtractConditions(predicate);
            if (conditions != null && conditions.Any())
            {
                var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
                var type = instance?.GetType();
                var props = type?.GetProperties();
                /* unica expressão tratada == */
                var condicionsRead = conditions.Where(x => x.Expressao == ExpressionType.Equal);
                var queryConditional = "{";
                foreach (var conditional in condicionsRead)
                {
                    queryConditional += conditional.Campo+": $"+ conditional.Campo+",";
                    var prop =  props.FirstOrDefault(x => x.Name == conditional.Campo);
                    prop.SetValue(instance, conditional.Valor);
                }
                queryConditional = queryConditional.Substring(0, queryConditional.Length - 1) + "}";
                var query = " MATCH (n:" + type.Name + queryConditional+ ") RETURN n ";

                await ExecuteQueryAsync(query, instance, async (cursor) =>
                {
                    var listRecord = await cursor.ToListAsync();
                    if (listRecord != null)
                    {
                        foreach (var record in listRecord)
                        {
                            var node = record["n"].As<INode>();
                            if (props != null && props.Any())
                            {
                                var instance = Activator.CreateInstance(typeof(TEntity)) as TEntity;
                                foreach (var prop in props)
                                {
                                    TratarClasseNodes(node, instance, prop);

                                }
                                listSearch.Add(instance);
                            }
                        }
                    }
                });
            }
            return listSearch;
        }
    }
}
