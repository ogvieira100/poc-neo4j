using Neo4j.Driver;

namespace Api.Data
{
    public interface IContextDb
    {
        public IDriver Driver { get;  }
    }
}
