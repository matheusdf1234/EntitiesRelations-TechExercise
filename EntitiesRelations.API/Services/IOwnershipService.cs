using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Services
{
    public interface IOwnershipService
    {
        Task<CompanyResponseDTO> BuyCompany(long entityId, long companyId, int percentage);
        void EstablishOwnership(CompanyModel company, int percentage);
    }
}