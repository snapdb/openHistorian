using openHistorian.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeviceController : ModelController<Device>
{
    /// <summary>
    /// Gets the default connection parameters for a device.
    /// </summary>
    /// <param name="detail">Device to get the connection parameters for.</param>
    /// <returns>Default Connection Parameters for a device.</returns>
    [HttpPost, Route("ConnectionParameters")]
    public IActionResult GetConnectionParameters(Device detail)
    {
        return Ok(new List<object>());
    }
}
