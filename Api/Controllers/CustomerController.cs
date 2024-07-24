using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

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


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Customer customer)
        {


            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            await _baseRepositoryCustomer.AddAsync(customer);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Customer customer)
        {


            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Put([FromRoute] Guid id)
        {


            return Ok();
        }

    }
}
