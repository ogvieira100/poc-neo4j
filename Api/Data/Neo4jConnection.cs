using Neo4j.Driver;
using System;

namespace Api.Data
{
    public class Neo4jConnection: IUnitOfWork, IDisposable
    {
        private readonly IDriver _driver;
        IAsyncSession session;
        readonly IConfiguration _configuration;  

        public Neo4jConnection(IConfiguration  configuration)
        {
           
            _configuration = configuration;
            _driver = GraphDatabase.Driver(uri: _configuration.GetSection("Neo4j:Uri").Value, AuthTokens.Basic(_configuration.GetSection("Neo4j:User").Value, _configuration.GetSection("Neo4j:Password").Value));
        }

        public async Task<IResultCursor> ExecuteQueryAsync(string query, object parameters = null)
        {
            session = _driver.AsyncSession();
            return await session.RunAsync(query, parameters);
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
