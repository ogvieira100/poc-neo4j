using Neo4j.Driver;

namespace Api.Data
{
    public class ContextDb : IContextDb, IDisposable
    {
        public   IDriver Driver { get; }
        readonly IConfiguration _configuration;

        public ContextDb(IConfiguration configuration)
        {
            _configuration = configuration;
            Driver = GraphDatabase.Driver(uri: _configuration.GetSection("Neo4j:Uri").Value, AuthTokens.Basic(_configuration.GetSection("Neo4j:User").Value, _configuration.GetSection("Neo4j:Password").Value));
        }

        public void Dispose()
        {
            Driver?.Dispose();
        }
    }
}
