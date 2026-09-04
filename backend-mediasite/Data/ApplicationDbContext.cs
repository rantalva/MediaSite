using MediaSite_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MediaSite_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fixed IDs are important for HasData()
            var styleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var shoppingId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var cultureId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var sportsId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            var seedDate = DateTime.SpecifyKind(new DateTime(2026, 8, 24), DateTimeKind.Utc);

            var userId = Guid.Parse("df3369fe-d3fa-41fb-b76a-05b4f64d042d");

            var mockUser = new ApplicationUser
            {
                Id = userId,

                UserName = "alvari.rantapelkonen@gmail.com",
                NormalizedUserName = "ALVARI.RANTAPELKONEN@GMAIL.COM",

                Email = "alvari.rantapelkonen@gmail.com",
                NormalizedEmail = "ALVARI.RANTAPELKONEN@GMAIL.COM",

                EmailConfirmed = true,

                FirstName = "Alvari",
                LastName = "Rantapelkonen",

                SecurityStamp = "11111111-1111-1111-1111-111111111111",
                ConcurrencyStamp = "22222222-2222-2222-2222-222222222222",

                PasswordHash = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw="
            };

            modelBuilder.Entity<ApplicationUser>().HasData(mockUser);

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = styleId,
                    Name = "Style"
                },
                new Category
                {
                    Id = shoppingId,
                    Name = "Shopping"
                },
                new Category
                {
                    Id = cultureId,
                    Name = "Culture"
                },
                new Category
                {
                    Id = sportsId,
                    Name = "Sports"
                }
            );

            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Title = "Vaatekaapin kulmakivien opas 2026",
                    Content = "Lorem ipsum dolor sit amet...",
                    Slug = "vaatekaapin-kulmakivien-opas-2026",
                    HeroImage = "../Uploads/Menswear+closet.jpg",
                    CreatedDate = seedDate,
                    CategoryId = styleId,
                    AuthorId = userId
                },
                new Article
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Title = "Black Fridayn parhaimmat ostosvinkit miehille",
                    Content = "Lorem ipsum dolor sit amet...",
                    Slug = "black-fridayn-parhaimmat-ostosvinkit-miehille",
                    HeroImage = "../Uploads/Menswear+closet.jpg",
                    CreatedDate = seedDate,
                    CategoryId = shoppingId,
                    AuthorId = userId
                },
                new Article
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Title = "Hellride 2026: Madness valokuvissa",
                    Content = "Lorem ipsum dolor sit amet...",
                    Slug = "hellride-2026-madness-valokuvissa",
                    HeroImage = "../Uploads/Menswear+closet.jpg",
                    CreatedDate = seedDate,
                    CategoryId = cultureId,
                    AuthorId = userId
                },
                new Article
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    Title = "Jokerit palaa liigaan: Pääkaupungin derbyt vuonna 2026",
                    Content = "Lorem ipsum dolor sit amet...",
                    Slug = "jokerit-palaa-liigaan-paakaupungin-derbyt-vuonna-2026",
                    HeroImage = "../Uploads/Menswear+closet.jpg",
                    CreatedDate = seedDate,
                    CategoryId = sportsId,
                    AuthorId = userId
                }
            );
        }
    }
}