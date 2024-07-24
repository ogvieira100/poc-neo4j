namespace Api.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }
        public double Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HIstory { get; set; }

        public Customer()
        {
            Id = Guid.NewGuid();
        }
    }
}
