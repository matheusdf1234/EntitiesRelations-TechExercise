using EntitiesRelations.API.DTOs;
using EntitiesRelations.API.Models;
using EntitiesRelations.API.Providers;

namespace EntitiesRelations.API.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonProvider _personProvider;
        private readonly ICompanyProvider _companyProvider;


        public PersonService(IPersonProvider personProvider, ICompanyProvider companyProvider)
        {
            ArgumentNullException.ThrowIfNull(companyProvider);
            ArgumentNullException.ThrowIfNull(personProvider);
            _personProvider = personProvider;
            _companyProvider = companyProvider;
        }
        public async Task<bool> AddPerson(PersonRequestDTO person)
        {
            //Ps.: probably not the best practice to have the Person Service be made aware of the CompanyProvider...
            if (await GetByIdAsync(person.Id) == null && await _companyProvider.GetCompanyById(person.Id) == null)
            {

                var personModel = new PersonModel(person.Id, person.Name, new List<long>());//We worry about the relations on their own endpoint, not at creation level
                await _personProvider.AddPerson(personModel);
                return true;
            }
            return false;

        }

        public async Task<IEnumerable<PersonModel>> GetAllPersons()
        {
            return await _personProvider.GetAllPersons();
        }

        public async Task<PersonModel> GetByIdAsync(long id)
        {
            return await _personProvider.GetPersonById(id);
        }

        public async Task<PersonModel> UpdatePerson(PersonModel person)
        {
            return await _personProvider.UpdatePerson(person);
        }

        public async Task DeletePerson(long id)
        {
            await _personProvider.DeletePerson(id);
            return;
        }

        public async Task<bool> EstablishRelationships(PersonModel person1, PersonModel person2)
        {
            //only add it if the relationship doesnt already exist
            if (!person2.MyRelations.Contains(person1.Id))
            {
                person1.MyRelations.Add(person2.Id);
                person2.MyRelations.Add(person1.Id);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteRelationships(PersonModel person1, PersonModel person2)
        {
            //only remove a relationship if it exists
            if (person2.MyRelations.Contains(person1.Id))
            {
                person1.MyRelations.Remove(person2.Id);
                person2.MyRelations.Remove(person1.Id);
                return true;
            }
            return false;
        }
    }
}
