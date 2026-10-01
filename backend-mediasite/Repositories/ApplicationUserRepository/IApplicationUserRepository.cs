namespace MediaSite_backend.Repositories.ApplicationUserRepository
{
    public interface IApplicationUserRepository
    {
        Task<int> GetUsersCount();
    }
}
