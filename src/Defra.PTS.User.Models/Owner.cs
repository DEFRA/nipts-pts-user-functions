using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.User.Models
{
    [ExcludeFromCodeCoverage]
    public class Owner
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Address? Address { get; set; }
    }
}
