using Gemstone.Web.APIController;
using GrafanaAdapters.Model.Database;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlarmStateController : ModelController<AlarmState>
{
}