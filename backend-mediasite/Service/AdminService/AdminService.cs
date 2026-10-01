using MediaSite_backend.Models.Dtos.AdminDashboardDto;
using MediaSite_backend.Repositories.ApplicationUserRepository;
using MediaSite_backend.Repositories.ArticleRepository;
using MediaSite_backend.Repositories.CategoryRepository;
using MediaSite_backend.Repositories.NewsletterSubscriberRepository;

namespace MediaSite_backend.Service.AdminService;

public class AdminService : IAdminService
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationUserRepository _applicationuserRepository;
    private readonly INewsLetterSubscriberRepository _newsLetterSubscriberRepository;
    public AdminService(IArticleRepository articleRepository, ICategoryRepository categoryRepository, IApplicationUserRepository applicationUserRepository, INewsLetterSubscriberRepository newsLetterSubscriberRepository)
    {
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
        _applicationuserRepository = applicationUserRepository;
        _newsLetterSubscriberRepository = newsLetterSubscriberRepository;
    }
    public async Task<AdminDashboardDto> GetAdminDashboard()
    {
        return new AdminDashboardDto
        {
            ArticleCount = await _articleRepository.GetArticlesCountAsync(),
            CategoryCount = await _categoryRepository.GetCategoriesCountAsync(),
            UserCount = await _applicationuserRepository.GetUsersCount(),
            NewslettersubscriberCount = await _newsLetterSubscriberRepository.GetNewsLettersubscribersCountAsync()
        };
    }
}
