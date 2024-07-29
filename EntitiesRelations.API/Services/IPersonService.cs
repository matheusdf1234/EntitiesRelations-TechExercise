using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonModel>> GetAllPersons();
        Task<PersonModel> GetByIdAsync(long id);
        Task<bool> AddPerson(PersonRequestDTO person);
        Task<PersonModel> UpdatePerson(PersonModel person);
        Task DeletePerson(long id);
        Task<bool> EstablishRelationships(PersonModel person1, PersonModel person2);
        Task<bool> DeleteRelationships(PersonModel person1, PersonModel person2);
    }
}