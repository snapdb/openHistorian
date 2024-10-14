using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]/[action]")]
public class CurrentUser : Controller
{

    // TODO: De

    [HttpGet, Route("/api/[controller]/settings")]
    public IActionResult GetSettings()
    {
        return View();
    }

    [HttpPatch, Route("/api/[controller]/settings")]
    public IActionResult UpdateSettings()
    {
        return View();
    }

}
