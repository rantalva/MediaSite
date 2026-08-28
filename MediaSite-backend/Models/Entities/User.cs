using System.ComponentModel.DataAnnotations;

namespace MediaSite_backend.Models.Entities
{
    public enum UserRole
    {
        Author,
        Editor,
        Admin
    }
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public required EmailAddressAttribute EmailAddress { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
    }
}
