using Gemstone.Timeseries.Adapters;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;
using System.Net;

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
        )
        }));
    }

    [HttpGet, Route("Options")]
    public IActionResult GetBasicProtocolInfo()
    {
        if (!GetAuthCheck())
            return Unauthorized();

        var result = AdapterCache.AdapterProtocols.Values
            .SelectMany(protocol => protocol.Attributes)
            .Select(attr => new
            {
                Label = attr.Name,
                Value = attr.Acronym,
                attr.SupportsConnectionTest,
            });

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the JavaScript module containing UI resources for the specified protocol.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the JavaScript module required by the adapter's UI.</returns>
    [HttpGet, Route("Components/{protocolAcronym}/{resourceID}")]
    public IActionResult GetAdapterResource(string protocolAcronym, string resourceID)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        protocolAcronym = WebUtility.UrlDecode(protocolAcronym);

        IEnumerable<UIResourceAttribute> allAttributes = AdapterCache<IAdapter>.UIResources.Values
            .SelectMany(uiInfo => uiInfo.Attributes);

        UIAdapterProtocolAttribute? protocolUIAttribute = allAttributes
            .OfType<UIAdapterProtocolAttribute>()
            .FirstOrDefault(attr => attr.Acronym.Equals(protocolAcronym, StringComparison.OrdinalIgnoreCase) && attr.ResourceID.Equals(resourceID, StringComparison.OrdinalIgnoreCase));

        if (protocolUIAttribute is null)
            return NotFound();

        // Get the JavaScript resource stream using the shared UI resource logic.
        Stream? resourceStream = protocolUIAttribute.GetResourceStream();

        if (resourceStream is null)
            return NotFound();

        // Return the stream as a JavaScript file.
        return File(resourceStream, "text/javascript");
    }
}
