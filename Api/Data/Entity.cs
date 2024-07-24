namespace Api.Data
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }

    public abstract class EntityDataBase : Entity
    {
        public bool Active { get; set; }
        public DateTime DateRegister { get; set; }
        public DateTime? DateUpdate { get; set; }
        public Guid UserInsertedId { get; set; }
        public Guid? UserUpdatedId { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? UserDeletedId { get; set; }
    }
}
