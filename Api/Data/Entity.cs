using Newtonsoft.Json;

namespace Api.Data
{
    public abstract class Entity
    {
        public string id { get; set; }
    }

    public abstract class EntityDataBase : Entity
    {
        public bool active { get; set; }
        public DateTime dateRegister { get; set; }
        public DateTime? dateUpdate { get; set; }
        public string userInsertedId { get; set; }
        public string? userUpdatedId { get; set; }
        public DateTime? deleteDate { get; set; }
        public string? userDeletedId { get; set; }
    }
}
