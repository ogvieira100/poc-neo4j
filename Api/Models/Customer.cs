using Api.Data;
using Newtonsoft.Json;

namespace Api.Models
{
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
