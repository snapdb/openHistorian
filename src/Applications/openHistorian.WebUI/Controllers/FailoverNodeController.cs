using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Utility;
using openHistorian.WebUI.Controllers.JsonModels;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FailoverNodeController : ReadOnlyModelController<JsonModels.FailoverNodeView>
{
    [HttpGet, Route("current")]
    public IActionResult GetCurrentNodeName()
    {
        if (!GetAuthCheck()) return Unauthorized();

        return Ok(new FailoverNodeView() { 
            SystemName = FailoverModule.SystemName, 
            Priority = FailoverModule.SystemPriority,
            LastLog = DateTime.UtcNow
        });
    }
}
