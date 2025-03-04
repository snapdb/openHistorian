using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Timeseries;
namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlarmController : ModelController<Alarm>
{
}