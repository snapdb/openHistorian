using openHistorian.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OutputAdaptersController : ModelController<CustomOutputAdapter>
{
}
