using Gemstone.Web.APIController;
using GrafanaAdapters.Model.Database;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlarmStateController : ModelController<AlarmState>
{
}