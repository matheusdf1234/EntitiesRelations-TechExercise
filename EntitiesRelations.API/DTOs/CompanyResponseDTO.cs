using EntitiesRelations.API.Models;
using System.Text.Json.Serialization;

namespace EntitiesRelations.API.DTOs
{
    public class CompanyResponseDTO : EntityDTO
    {

        [JsonPropertyName("availableShares")]
        public int AvailableShares { get; set; }

        [JsonPropertyName("whoOwnsMe")]
        public Dictionary<long, int> WhoOwnsMe { get; set; }
    }
}
