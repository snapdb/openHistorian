using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;

namespace openHistorian.WebUI.Controllers;

[Route("api/VendorDeviceView")]
[ApiController]
public class VendorDeviceDetailController : ReadOnlyModelController<VendorDeviceDetail>
{
}
