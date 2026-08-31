using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MediaSite_backend.Models.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
