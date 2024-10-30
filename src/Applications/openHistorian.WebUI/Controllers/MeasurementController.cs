using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeasurementController: ModelController<Measurement>
{
}
