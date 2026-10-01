using MediaSite_backend.Models.Dtos.AdminDashboardDto;
using MediaSite_backend.Service.AdminService;
using Microsoft.AspNetCore.Mvc;

namespace MediaSite_backend.Controllers
{
    [Route("api/admindashboard")]
    [ApiController]

    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminDashboardController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpGet]
        public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard()
        {
            return Ok(_adminService.GetAdminDashboard());
        }
    }
}
