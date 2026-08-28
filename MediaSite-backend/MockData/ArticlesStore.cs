using MediaSite_backend.Models.Entities;

namespace MediaSite_backend.MockData
{
    public class ArticlesStore
    {
        public static List<Article> ArticlesList = new List<Article>
        {
            new Article 
            {
                Title = "Suomen tyylikkäimmät miehet 2026", 
                Content = "Alvari Rantapelkonen on suomen tyylikkäin mies 2026.", 
                Slug = "suomen-tyylikkäimmät-miehet-2026", 
                HeroImage = "/uploads/test", 
                CreatedDate = DateTime.UtcNow,
                Category = CategoriesStore.CategoriesList[0]
            },
            new Article 
            {
                Title = "Hajuvesi suositukset syksylle 2026", 
                Content = "Ralph Lauren", 
                Slug = "hajuvesi-suositukset-syksylle-2026", 
                HeroImage = "/uploads/test2", 
                CreatedDate = DateTime.UtcNow, 
                Category = CategoriesStore.CategoriesList[1]
            }
        };

    }
}
