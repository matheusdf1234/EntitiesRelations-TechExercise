using System.Text.Json.Serialization;

namespace EntitiesRelations.API.DTOs
{
    public abstract class EntityDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
