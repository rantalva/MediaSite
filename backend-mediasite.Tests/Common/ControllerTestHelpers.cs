using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MediaSite_backend.Tests.Common;

/// <summary>
/// ControllerBase.CreatedAtRoute / CreatedAtAction both resolve the Location header through
/// IUrlHelper, which is null when the controller is constructed directly instead of by MVC.
/// Without this, every 201 assertion throws NullReferenceException before reaching its assert.
/// </summary>
internal static class ControllerTestHelpers
{
    public static T WithStubbedUrlHelper<T>(this T controller) where T : ControllerBase
    {
        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.RouteUrl(Arg.Any<UrlRouteContext>()).Returns((string?)null);
        urlHelper.Action(Arg.Any<UrlActionContext>()).Returns((string?)null);

        var urlHelperFactory = Substitute.For<IUrlHelperFactory>();
        urlHelperFactory.GetUrlHelper(Arg.Any<ActionContext>()).Returns(urlHelper);

        var services = new ServiceCollection()
            .AddSingleton(urlHelperFactory)
            .AddLogging()
            .BuildServiceProvider();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { RequestServices = services }
        };

        return controller;
    }
}
