using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly Providers.ICompanyProvider _companyProvider;
        private readonly Providers.IPersonProvider _personprovider;

        public CompanyService(Providers.ICompanyProvider companyProvider, Providers.IPersonProvider personprovider)
        {
            ArgumentNullException.ThrowIfNull(companyProvider);
            ArgumentNullException.ThrowIfNull(personprovider);
            _companyProvider = companyProvider;
            _personprovider = personprovider;
        }
        public async Task<bool> AddCompany(CompanyRequestDTO companyDTO)
        {

            //Ps.: probably not the best practice to have the Company Service be made aware of the PersonProvider...
            if (await GetByIdAsync(companyDTO.Id) == null && await _personprovider.GetPersonById(companyDTO.Id) == null)
            {
                var companyModel = new CompanyModel(companyDTO.Id, companyDTO.Name);

                await _companyProvider.AddCompany(companyModel);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<CompanyModel>> GetAllCompanies()
        {
            return await _companyProvider.GetAllCompanies();
        }

        public async Task<CompanyModel> GetByIdAsync(long id)
        {
            return await _companyProvider.GetCompanyById(id);
        }

        public async Task<CompanyModel> UpdateCompany(CompanyModel company)
        {
            var companyModel = new CompanyModel(company.Id, company.Name);
            return await _companyProvider.UpdateCompany(companyModel);

        }

        public async Task DeleteCompany(long id)
        {
            await _companyProvider.DeleteCompany(id);
            return;
        }
    }
}
