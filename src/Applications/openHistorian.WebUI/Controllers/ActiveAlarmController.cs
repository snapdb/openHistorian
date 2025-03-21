using DataQualityMonitoring;
using Gemstone.Timeseries;
using Microsoft.AspNetCore.Mvc;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActiveAlarmController : ControllerBase
{
    /// <summary>
    /// Gets all records from associated table sorted by the provided field.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing <see cref="T:T[]"/> or <see cref="Exception"/>.</returns>
    [HttpPost, Route("")]
    public IActionResult GetAlarms([FromBody] AlarmSeverity[] severities)
    {
        return AlarmAdapter.GetRaisedAlarmsStatic(this, severities);
    }
}