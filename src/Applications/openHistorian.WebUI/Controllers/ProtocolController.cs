using openHistorian.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Timeseries.Adapters;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProtocolController : ReadOnlyModelController<Protocol>
{
    /// <summary>
    /// Gets a collection of all protocols and their associated information.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="IEnumerable{AdapterInfo}"/> of all adapters.</returns>
    [HttpGet, Route("All")]
    public IActionResult GetAllAdaptersDetail()
    {
        if (!GetAuthCheck())
            return Unauthorized();

        return Ok(AdapterCache.AdapterProtocols.Values.Select(protocol => new
        {
            protocol.Info,
            Attributes = protocol.Attributes.Select(attr => new
            {
                attr.Acronym,
                attr.Name,
                attr.Type,
                attr.Category,
                attr.SupportsConnectionTest,
                attr.LoadOrder
            }
        )}));
    }
}
