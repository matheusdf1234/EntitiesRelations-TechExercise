using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Providers
{
    public interface IPersonProvider
    {
        Task AddPerson(PersonModel person);
        Task<IEnumerable<PersonModel>> GetAllPersons();
        Task<PersonModel> GetPersonById(long id);
        Task<PersonModel> UpdatePerson(PersonModel person);
        Task DeletePerson(long id);
    }
}