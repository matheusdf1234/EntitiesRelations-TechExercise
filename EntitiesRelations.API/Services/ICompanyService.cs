using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Services
{
    public interface ICompanyService
    {
        Task<bool> AddCompany(CompanyRequestDTO company);
        Task<IEnumerable<CompanyModel>> GetAllCompanies();
        Task<CompanyModel> GetByIdAsync(long id);
        Task<CompanyModel> UpdateCompany(CompanyModel company);
        Task DeleteCompany(long id);
    }
}
