using Microsoft.AspNetCore.Mvc;

namespace Egw.PubManagement.Controllers;

/// <summary>
/// Index page controller
/// </summary>
[Route("/")]
public class IndexPageController : ControllerBase
{
    /// <summary>
    /// Index page
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return new RedirectResult("/graphql");
    }
}