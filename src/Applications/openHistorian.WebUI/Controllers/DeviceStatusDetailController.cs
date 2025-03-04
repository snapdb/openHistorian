using Gemstone.Web.APIController;
using GrafanaAdapters.Model.Database;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

//rename models and routes to drop alarm

[Route("api/DeviceStatusView")]
[ApiController]
public class DeviceStatusDetailController : ReadOnlyModelController<DeviceStatusView>
{
}