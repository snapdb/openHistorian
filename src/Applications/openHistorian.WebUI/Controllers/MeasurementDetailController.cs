using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;

namespace openHistorian.WebUI.Controllers;

[Route("api/MeasurementView")]
[ApiController]
public class MeasurementDetailController : ReadOnlyModelController<MeasurementDetail>
{
}
