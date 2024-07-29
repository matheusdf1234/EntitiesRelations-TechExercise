using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Providers
{
    public interface ICompanyProvider
    {
        Task AddCompany(CompanyModel company);
        Task<IEnumerable<CompanyModel>> GetAllCompanies();
        Task<CompanyModel> UpdateCompany(CompanyModel company);
        Task DeleteCompany(long id);
        Task<CompanyModel> GetCompanyById(long id);
        Task<int> GetCompanyAvailableShares(long id);
    }
}
