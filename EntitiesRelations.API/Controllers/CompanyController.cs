using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EntitiesRelations.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IOwnershipService _ownershipService;
        public CompanyController(ICompanyService companyService, IOwnershipService ownershipService)
        {
            ArgumentNullException.ThrowIfNull(companyService);
            ArgumentNullException.ThrowIfNull(ownershipService);
            _companyService = companyService;
            _ownershipService = ownershipService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CompanyRequestDTO company)
        {

            //TODO: I think that the service returning false when the id is being used is potentially "hiding" other errors.
            //If the provider (DB) is unavailable for example, instead of the user receiving a 500, he will get a BadRequest

            if (await _companyService.AddCompany(company))
            {
                return Ok(new CompanyResponseDTO { Id = company.Id, Name = company.Name, AvailableShares = 100, WhoOwnsMe = new Dictionary<long, int>() });
            }

            return BadRequest($"The id {company.Id} already exists");

        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _companyService.GetAllCompanies();

            var companies = new List<CompanyResponseDTO>();
            foreach (var company in result)
            {
                companies.Add(new CompanyResponseDTO { Id = company.Id, Name = company.Name, AvailableShares = company.AvailableShares, WhoOwnsMe = company.WhoOwnsMeList });
            }

            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyResponseDTO>> GetAsync(long id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            var companyDTO = new CompanyResponseDTO { Id = company.Id, Name = company.Name, AvailableShares = company.AvailableShares, WhoOwnsMe = company.WhoOwnsMeList };
            return Ok(companyDTO);
        }

        [HttpPut]
        public async Task<ActionResult<CompanyResponseDTO>> Put([FromBody] CompanyRequestDTO company)
        {
            var companyToBeUpdated = await _companyService.GetByIdAsync(company.Id);
            if (companyToBeUpdated == null)
            {
                return NotFound($"Could not find company with id {company.Id}");
            }
            var result = await _companyService.UpdateCompany(companyToBeUpdated);

            return Ok(new CompanyResponseDTO { Id = result.Id, Name = result.Name, AvailableShares = result.AvailableShares, WhoOwnsMe = result.WhoOwnsMeList });
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] long id)
        {
            if (await _companyService.GetByIdAsync(id) == null)
            {
                return NotFound($"Could not find company with id {id}");
            }
            await _companyService.DeleteCompany(id);

            return Ok($"Company {id} deleted");
        }

        [HttpPost("buyCompany")]
        public async Task<IActionResult> BuyCompany([FromQuery] long entityId, [FromQuery] long companyId, [FromQuery] int percentage) 
        {
            if (await _companyService.GetByIdAsync(entityId) != null)
            {
                var companyBought = await _ownershipService.BuyCompany(entityId, companyId, percentage);

                if (companyBought != null)
                {
                    return Ok(companyBought);
                }
                else
                    return BadRequest();

            }
            return NotFound($"company with id {entityId} does not exist");
        }

    }
}
