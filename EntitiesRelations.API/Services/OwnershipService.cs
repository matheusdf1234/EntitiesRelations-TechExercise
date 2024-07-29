using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Services
{
    public class OwnershipService : IOwnershipService
    {
        private readonly IPersonService _personService;
        private readonly ICompanyService _companyService;

        public OwnershipService(IPersonService personService, ICompanyService companyService)
        {
            _personService = personService;
            _companyService = companyService;
        }

        public async Task<CompanyResponseDTO> BuyCompany(long entityId, long companyId, int percentage)
        {

            var companyToBeBought = await _companyService.GetByIdAsync(companyId);
            if (companyToBeBought != null && companyToBeBought.AvailableShares >= percentage) //TODO: extract this to a "canCompanyBeBought" method
            {
                //in a real world scenario, this would need to call the CompanyProvider class to write this data in the DB
                if (companyToBeBought.WhoOwnsMeList.ContainsKey(entityId))
                {
                    companyToBeBought.WhoOwnsMeList[entityId] = companyToBeBought.WhoOwnsMeList[entityId] + percentage;
                }
                else
                {
                    companyToBeBought.WhoOwnsMeList.Add(entityId, percentage);
                }
                companyToBeBought.AvailableShares = companyToBeBought.AvailableShares - percentage;

                EstablishOwnership(companyToBeBought, companyToBeBought.WhoOwnsMeList[entityId]);

                var companyDTO = new CompanyResponseDTO { AvailableShares = companyToBeBought.AvailableShares, Id = entityId, Name = companyToBeBought.Name, WhoOwnsMe = companyToBeBought.WhoOwnsMeList };
                return companyDTO;
            }
            return null;

        }

        public void EstablishOwnership(CompanyModel company, int percentage)
        {
            if (percentage >= 60)
            {
                company.IsControlled = true;
            }
        }
    }
}
