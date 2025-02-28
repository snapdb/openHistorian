using Gemstone.Web.APIController;
using GrafanaAdapters.Model.Database;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

//rename models and routes to drop alarm

[Route("api/DeviceStateView")]
[ApiController]
public class DeviceStateDetailController : ReadOnlyModelController<DeviceStateView>
{
}