using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;
using EntitiesRelations.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EntitiesRelations.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IOwnershipService _ownershipService;

    public PersonController(IPersonService personService, IOwnershipService ownershipService)
    {
        ArgumentNullException.ThrowIfNull(personService);
        ArgumentNullException.ThrowIfNull(ownershipService);
        _personService = personService;
        _ownershipService = ownershipService;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] PersonRequestDTO person)
    {

        if (await _personService.AddPerson(person))
        {
            return Ok(new PersonResponseDTO { Id = person.Id, Name = person.Name, MyRelations = new List<long>() });
        }

        return BadRequest($"The person id {person.Id} already exists");

    }

    [HttpPost("buyCompany")]
    public async Task<IActionResult> BuyCompany([FromQuery] long personId, [FromQuery] long companyId, [FromQuery] int percentage)
    {
        if (await _personService.GetByIdAsync(personId) != null)
        {
            var companyBought = await _ownershipService.BuyCompany(personId, companyId, percentage);

            if (companyBought != null)
            {
                return Ok(companyBought);

            }
            else
                return BadRequest();

        }
        return NotFound($"person with id {personId} does not exist");
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var result = await _personService.GetAllPersons();
        var personsDTO = new List<PersonResponseDTO>();
        foreach (var person in result)
        {
            personsDTO.Add(new PersonResponseDTO { Id = person.Id, Name = person.Name, MyRelations = person.MyRelations });
        }
        return Ok(personsDTO);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAsync(long id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person != null)
        {
            var personDTO = new PersonResponseDTO { Id = person.Id, Name = person.Name, MyRelations = person.MyRelations };
            return Ok(personDTO);
        }
        return NotFound();
    }

    [HttpPut]
    public async Task<ActionResult<PersonResponseDTO>> Put([FromBody] PersonRequestDTO person)
    {
        var personToBeUpdated = await _personService.GetByIdAsync(person.Id);
        if (personToBeUpdated != null)
        {
            var result = _personService.UpdatePerson(personToBeUpdated);
            return Ok(new PersonResponseDTO { Id = result.Result.Id, Name = result.Result.Name, MyRelations = result.Result.MyRelations });
        }
        return NotFound($"Could not find person with id {person.Id}");
    }

    [HttpDelete]
    public async Task<ActionResult> Delete([FromQuery] long id)
    {
        var personToBeDeleted = await _personService.GetByIdAsync(id);
        if (personToBeDeleted != null)
        {
            _personService.DeletePerson(id);

            return Ok($"person with id {id} was successfully deleted.");
        }
        return NotFound($"Could not find person with id {id}");
    }

    [HttpPost("{person1Id, person2Id}")]
    [Route("CreateRelations")]
    public async Task<ActionResult> AssignRelations(long person1Id, long person2Id)
    {
        var person1 = await _personService.GetByIdAsync(person1Id);
        var person2 = await _personService.GetByIdAsync(person2Id);
        if (person1 == null)
        {
            return NotFound($"Could not find person with id {person1Id}");
        }
        if (person2 == null)
        {
            return NotFound($"Could not find person with id {person2Id}");
        }

        if (await _personService.EstablishRelationships(person1, person2))
        {
            return Ok(new List<PersonResponseDTO>()
            {
                new PersonResponseDTO { Id = person1.Id, Name = person1.Name, MyRelations = person1.MyRelations },
                new PersonResponseDTO { Id = person2.Id, Name = person2.Name, MyRelations = person2.MyRelations }
             });
        }
        return Ok($"Relationship between {person1Id} and {person2Id} was already set");
    }

    [HttpPost("{person1Id, person2Id}")]
    [Route("DeleteRelations")]
    public async Task<ActionResult> DeleteRelations(long person1Id, long person2Id)
    {
        var person1 = await _personService.GetByIdAsync(person1Id);
        var person2 = await _personService.GetByIdAsync(person2Id);
        if (person1 == null)
        {
            return NotFound($"Could not find person with id {person1Id}");
        }
        if (person2 == null)
        {
            return NotFound($"Could not find person with id {person2Id}");
        }
        if (await _personService.DeleteRelationships(person1, person2))
        {
            return Ok(new List<PersonModel>() { person1, person2 });
        }
        return Ok($"Relationship between {person1Id} and {person2Id} already non existent");
    }


}
