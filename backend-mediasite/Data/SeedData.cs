using MediaSite_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace MediaSite_backend.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roles =
            {
                ApplicationUserRoles.Admin,
                ApplicationUserRoles.Editor,
                ApplicationUserRoles.Author,
                ApplicationUserRoles.Member
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var userManager =
                services.GetRequiredService<UserManager<ApplicationUser>>();

            var existingUser =
                await userManager.FindByEmailAsync("test@gmail.com");

            if (existingUser == null)
            {
                var testUser = new ApplicationUser
                {
                    Email = "test@gmail.com",
                    UserName = "test@gmail.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    testUser,
                    "TestPassword123!"
                );

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        testUser,
                        ApplicationUserRoles.Admin
                    );
                }
            }

        }

    }
}
