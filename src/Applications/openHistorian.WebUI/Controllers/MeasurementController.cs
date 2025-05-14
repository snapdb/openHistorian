using Gemstone.Data.Model;
using Gemstone.Data;
using Gemstone.Timeseries.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeasurementController: ModelController<Measurement>
{

    /// <summary>
    /// Gets a new record.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A <see cref="T"/> object or <see cref="Exception"/>.</returns>
    [HttpGet, Route("New")]
    public override async Task<IActionResult> New(CancellationToken cancellationToken)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        await using AdoDataConnection connection = CreateConnection();
        TableOperations<Measurement> tableOperations = new(connection);

        Measurement? result = tableOperations.NewRecord();
        result.Manual = true;
        return Ok(result);
    }
}
