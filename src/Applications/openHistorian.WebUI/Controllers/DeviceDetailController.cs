using openHistorian.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/DeviceView")]
[ApiController]
public class DeviceDetailController : ReadOnlyModelController<DeviceDetail>
{
}
