
namespace EntitiesRelations.API.Models
{
    public class PersonModel : Entity
    {
        public ICollection<long> MyRelations { get; set; } 
        public PersonModel(long id, string name, ICollection<long> myRelations) : base(id, name)
        {
            MyRelations = myRelations;
        }
    }
}
