using MediaSite_backend.Models.Entities;

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
    }
}
