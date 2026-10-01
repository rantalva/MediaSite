using MediaSite_backend.Data;
using MediaSite_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaSite_backend.Repositories.ApplicationUserRepository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ApplicationUserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<ApplicationUser> GetByIdAsync(Guid id) 
        {
            var user = await _applicationDbContext.Users.FindAsync(id);

            if (user == null) 
            {
                return null;
            }

            return user;
        }
        public async Task<List<ApplicationUser>> GetAllApplicationUsersAsync()
        {
            return await _applicationDbContext.Users.ToListAsync();
        }
        public async Task<int> GetUsersCount()
        {
            return await _applicationDbContext.Users.CountAsync();
        }
    }
}
