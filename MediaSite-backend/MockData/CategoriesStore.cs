using MediaSite_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace MediaSite_backend.MockData
{
    public class CategoriesStore
    {


        public static List<Category> CategoriesList = new List<Category>
        {
            new Category {Name="Style"},
            new Category {Name="Shopping"},
            new Category {Name="Culture"},
            new Category {Name="Sports"},
        };
        public static List<ApplicationUser> ApplicationUsersList = new List<ApplicationUser>
        {

            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "Alvari",
                LastName = "Rantapelkonen",
                Email = "alvari.rantapelkonen@gmail.com"
            }
        };
    }
}
