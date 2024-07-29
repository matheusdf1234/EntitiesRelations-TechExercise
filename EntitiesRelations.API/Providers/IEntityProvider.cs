
using EntitiesRelations.API.Models;

namespace EntitiesRelations.API.Providers
{
    public interface IEntityProvider
    {
        Task<Entity> GetEntityById(long id);
    }
}