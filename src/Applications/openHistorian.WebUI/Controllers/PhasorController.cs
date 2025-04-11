using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Timeseries.Model;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhasorController : ModelController<Phasor>
{
}
