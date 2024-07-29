using EntitiesRelations.API.Models;
using System.Text.Json.Serialization;

namespace EntitiesRelations.API.DTOs
{
    public class PersonResponseDTO : EntityDTO
    {
        [JsonPropertyName("myRelations")]
        public ICollection<long> MyRelations { get; set; }
    }
}
