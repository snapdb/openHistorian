using Gemstone.Timeseries.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeasurementController: ModelController<Measurement>
{
}
