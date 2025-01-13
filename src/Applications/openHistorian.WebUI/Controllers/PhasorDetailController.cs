using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;

namespace openHistorian.WebUI.Controllers;

[Route("api/PhasorView")]
[ApiController]
public class PhasorDetailController : ReadOnlyModelController<PhasorDetail>
{
}
