using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.User.Models
{
    [ExcludeFromCodeCoverageAttribute]
    public class UserEmail
    {
        public string? Email { get; set; }
        public string? Type { get; set; }
        public Guid? AddressId { get; set; }
    }
}