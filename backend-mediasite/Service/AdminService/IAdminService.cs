using MediaSite_backend.Models.Dtos.AdminDashboardDto;

namespace MediaSite_backend.Service.AdminService;

public interface IAdminService 
{
    Task<AdminDashboardDto> GetAdminDashboard();
}
