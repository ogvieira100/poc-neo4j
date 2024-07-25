using Api.Data;
using Newtonsoft.Json;

namespace Api.Models
{

    public class CustomerQuerySearch
    {
        public bool? active { get; set; }
        public DateTime? dateRegister { get; set; }
        public DateTime? dateUpdate { get; set; }
        public string? userInsertedId { get; set; }
        public string? userUpdatedId { get; set; }
        public DateTime? deleteDate { get; set; }
        public string? userDeletedId { get; set; }
        public string? name { get; set; }
        public string? surName { get; set; }
        public int? age { get; set; }
        public string? address { get; set; }
        public decimal? salary { get; set; }
        public double? weight { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string? history { get; set; }

    }
    public class CustomerUpdateQuery : EntityDataBase
    {
        public IEnumerable<string> fieldsUpdate { get; set; }
        public string name { get; set; }
        public string surName { get; set; }
        public int age { get; set; }
        public string address { get; set; }
        public decimal salary { get; set; }
        public double weight { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string history { get; set; }
    }
    public class Customer:EntityDataBase
    {
        public string name { get; set; }
        public string surName { get; set; }
        public int age { get; set; }
        public string address { get; set; }
        public decimal salary { get; set; }
        public double weight { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string history { get; set; }
        public Customer()
        {
            id = Guid.NewGuid().ToString();
        }
    }
}
