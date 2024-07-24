using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Neo4j.Driver;
using System;
using System.Net;
using System.Xml.Linq;

namespace Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PersonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetByHistory")]
        public async Task<IActionResult> GetByHistory([FromQuery] string history)
        {
            var listPerson = new List<object>();
            try
            {
                var query = "CALL db.index.fulltext.queryNodes('personHistories', $history) YIELD node RETURN node ";
                var parameters =
                 new
                 {
                     history = history
                 };
                var result = await _unitOfWork.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    var node = record["node"].As<INode>();
                    node.TryGet<string>("id", out var res);
                    listPerson.Add(new
                    {
                        id = res ?? "",
                        name = node["name"],
                        surname = node["surname"],
                        age = node["age"],
                        address = node["address"],
                        salary = node["salary"],
                        weight = node["weight"],
                        historia = node["historia"],
                        dateOfBirth = node["dateOfBirth"],
                    });
                }
                return Ok(listPerson);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Person person)
        {
            try
            {
                var query = "MATCH (n:Person {id: $id}) SET n.name =  $name, n.surname = $surname, n.age= $age, n.address= $address, n.salary= $salary, n.weight= $weight, n.historia= $historia, n.dateOfBirth= $dateOfBirth  RETURN n ";
                var parameters =
                 new
                 {
                     id = person.Id.ToString(),
                     name = person.Name,
                     surname = person.SurName,
                     age = person.Age,
                     address = person.Address,
                     salary = person.Salary,
                     weight = person.Weight,
                     historia = person.Historia,
                     dateOfBirth = person.DateOfBirth
                 };
                var result = await _unitOfWork.ExecuteQueryAsync(query, parameters);
                while (await result.FetchAsync())
                {
                    var node = result.Current["n"].As<INode>();
                    Console.WriteLine($"Updated node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                    return Ok(new { id = node["id"], name = node["name"], surname = node["surname"], age = node["age"], address = node["address"], salary = node["salary"], weight = node["weight"], historia = node["historia"], dateOfBirth = node["dateOfBirth"] });
                }

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            return BadRequest();
        }

        [HttpDelete("DeleteById/{id:guid}")]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            try
            {
                var query = "MATCH (n:Person {id: $id}) DELETE n";
                var parameters = new { id = id.ToString() };
                await _unitOfWork.ExecuteQueryAsync(query, parameters);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("All")]
        public async Task<IActionResult> ReadAllPerson()
        {
            var listPerson = new List<object>();
            try
            {
                var query = "MATCH (n:Person) RETURN n";
                var result = await _unitOfWork.ExecuteQueryAsync(query);
                await foreach (var record in result)
                {
                    var node = record["n"].As<INode>();
                    node.TryGet<string>("id", out var res);
                    listPerson.Add(new
                    {
                        id = res ?? "",
                        name = node["name"],
                        surname = node["surname"],
                        age = node["age"],
                        address = node["address"],
                        salary = node["salary"],
                        weight = node["weight"],
                        historia = node["historia"],
                        dateOfBirth = node["dateOfBirth"],
                    });
                }
                return Ok(listPerson);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            try
            {
                var query = "CREATE (n:Person {id:$id, name: $name, surname: $surname, age: $age, address: $address, salary: $salary, weight: $weight, historia: $historia, dateOfBirth: $dateOfBirth}) RETURN n";
                var parameters =
                    new
                    {
                        id = person.Id.ToString(),
                        name = person.Name,
                        surname = person.SurName,
                        age = person.Age,
                        address = person.Address,
                        salary = person.Salary,
                        weight = person.Weight,
                        historia = person.Historia,
                        dateOfBirth = person.DateOfBirth
                    };
                var result = await _unitOfWork.ExecuteQueryAsync(query, parameters);
                while (await result.FetchAsync())
                {
                    var node = result.Current["n"].As<INode>();
                    Console.WriteLine($"Created node: {node["name"]} {node["surname"]}, Age: {node["age"]}, Address: {node["address"]}, Salary: {node["salary"]}, Weight: {node["weight"]}, History: {node["historia"]}, DateOfBirth: {node["dateOfBirth"]}");
                    return Ok(new { id = node["id"], name = node["name"], surname = node["surname"], age = node["age"], address = node["address"], salary = node["salary"], weight = node["weight"], historia = node["historia"], dateOfBirth = node["dateOfBirth"] });
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
            return BadRequest();
        }

    }
}
