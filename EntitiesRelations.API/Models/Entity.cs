namespace EntitiesRelations.API.Models;
public abstract class Entity
{
    public Entity(long id, string name)
    {
        Id = id;
        Name = name;
    }
    public long Id { get; set; }

    public string Name { get; set; }

}
