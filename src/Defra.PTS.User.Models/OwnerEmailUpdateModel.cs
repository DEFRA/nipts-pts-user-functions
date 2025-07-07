using System.Text.Json.Serialization;

namespace Defra.PTS.User.Models
{
    public class OwnerEmailUpdateModel
    {
        [JsonPropertyName("oldEmail")]
        public string OldEmail { get; set; } = string.Empty;

        [JsonPropertyName("newEmail")]
        public string NewEmail { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
    }
}