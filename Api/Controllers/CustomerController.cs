using Api.Data;
using Api.Models;
using Api.Util;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System.Linq.Expressions;

namespace Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        readonly IBaseRepository<Customer> _baseRepositoryCustomer;
        public CustomerController(IBaseRepository<Customer> baseRepositoryCustomer)
        {
            _baseRepositoryCustomer = baseRepositoryCustomer;   
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _baseRepositoryCustomer.RepositoryConsult.GetAllAsync();
            return Ok(all);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CustomerQuerySearch customer)
        {
            Expression<Func<Customer, bool>> predicate =  x => true;

            if (!string.IsNullOrEmpty(customer.name))
            {
                string n = customer.name!;
                predicate = UtilClass.AddConditionEqual(predicate, "name",n );
            }

            if (!string.IsNullOrEmpty(customer.surName))
            {
                string n = customer.surName!;
                predicate = UtilClass.AddConditionEqual(predicate, "surName", n);
            }


            var ls = await _baseRepositoryCustomer.RepositoryConsult.SearchAsync(predicate);
            return Ok(ls);
        }


        [HttpGet("ById/{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var customer =  await _baseRepositoryCustomer.RepositoryConsult.GetByIdAsync(id);
            return Ok(customer);
        }

        [HttpPost("CreateAllCustomer")]
        public async Task<IActionResult> CreateAllCustomer([FromBody] IEnumerable<Customer> customers)
        {
            await _baseRepositoryCustomer.AddColectionAsync(customers.ToList());
            return Ok();    
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            await _baseRepositoryCustomer.AddAsync(customer);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CustomerUpdateQuery customer)
        {

            await _baseRepositoryCustomer.UpdateAsync(new Customer { 
                active = customer.active,   
                address = customer.address,
                age = customer.age,
                dateOfBirth = customer.dateOfBirth,
                dateRegister = customer.dateRegister,   
                dateUpdate = customer.dateUpdate,   
                deleteDate = customer.deleteDate,
                history = customer.history, 
                id = customer.id,
                name = customer.name,
                salary = customer.salary,
                surName = customer.surName,
                userDeletedId = customer.userDeletedId,
                userInsertedId = customer.userInsertedId,
                userUpdatedId = customer.userUpdatedId, 
                weight = customer.weight,
                
            }, customer.fieldsUpdate);
            return Ok();
            
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Put([FromRoute] Guid id)
        {
            await _baseRepositoryCustomer.RemoveAsync(new Customer { id = id.ToString() });
            return Ok();
        }

    }
}
