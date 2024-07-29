using EntitiesRelations.API.Models;
using System;

namespace EntitiesRelations.API.Providers
{
    public class PersonProvider : IPersonProvider
    {
        public static ICollection<PersonModel> PersonTable = new List<PersonModel>();
        public async Task<IEnumerable<PersonModel>> GetAllPersons()
        {
            return PersonTable;
        }

        public async Task AddPerson(PersonModel person)
        {
            PersonTable.Add(person);
            return;
        }

        public async Task<PersonModel> GetPersonById(long id)
        {
            var person = PersonTable.FirstOrDefault(x => x.Id == id);
            return person;
        }

        public async Task<PersonModel> UpdatePerson(PersonModel person)
        {
            var personToBeUpdated = PersonTable.First(x => x.Id == person.Id);

            //the only property eligible to update is the Name, all the other are either immutable (like the ID),
            //relationships are handled in the service directly
            personToBeUpdated.Name = person.Name;  
            return personToBeUpdated;
        }

        public async Task DeletePerson(long id)
        {
            var personToBeDeleted = PersonTable.First(x => x.Id == id);

            foreach (var personId in personToBeDeleted.MyRelations)
            {
                var person = await GetPersonById(personId);
                person.MyRelations.Remove(id);
            }
            PersonTable.Remove(personToBeDeleted);
        }
    }
}
