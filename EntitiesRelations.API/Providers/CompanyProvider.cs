using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Providers
{
    public class CompanyProvider : ICompanyProvider
    {
        public static ICollection<CompanyModel> CompanyTable = new List<CompanyModel>();

        public Task AddCompany(CompanyModel company)
        {
            CompanyTable.Add(company);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<CompanyModel>> GetAllCompanies()
        {
            return CompanyTable;
        }

        public async Task<CompanyModel> UpdateCompany(CompanyModel company)
        {
            var companyToBeUpdated = CompanyTable.First(x => x.Id == company.Id);
            
            //the only property eligible to update is the Name, all the other are either immutable (like the ID),
            //or are handled through the Ownership service (whoOwnsMeList, AvailableShares, etc)
            companyToBeUpdated.Name = company.Name;

            return companyToBeUpdated;
        }

        public async Task DeleteCompany(long id)
        {
            var companyToBeDeleted = CompanyTable.First(x => x.Id == id);
            CompanyTable.Remove(companyToBeDeleted);
        }

        public async Task<CompanyModel> GetCompanyById(long id)
        {
            var company = CompanyTable.FirstOrDefault(x => x.Id == id);
            return company;
        }

        public async Task<int> GetCompanyAvailableShares(long id)
        {
            return CompanyTable.Single(x => x.Id == id).AvailableShares;
        }
    }
}
