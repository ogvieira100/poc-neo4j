using Neo4j.Driver;

namespace Api.Data
{
    public interface IUnitOfWork
    {
        Task<IResultCursor> ExecuteQueryAsync(string query, object parameters = null);
    }
}
