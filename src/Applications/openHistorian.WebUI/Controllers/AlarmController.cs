using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Timeseries;
using Gemstone.Data.Model;
using Gemstone.Data;
namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlarmController : ModelController<Alarm>
{

    /// <summary>
    /// Creates new record in associated table.
    /// </summary>
    /// <param name="record">The record to be created.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing the new <see cref="T"/> or <see cref="Exception"/>.</returns>
    [HttpPost, Route("")]
    public override async Task<IActionResult> Post([FromBody] Alarm alarmRecord, CancellationToken cancellationToken)
    {
        if (!PostAuthCheck())
            return Unauthorized();

        await using AdoDataConnection connection = CreateConnection();
        Gemstone.Timeseries.Model.Measurement? newMeasurement = null;

        TableOperations<Gemstone.Timeseries.Model.SignalType> signalTypeTableOperations = new(connection);
        Gemstone.Timeseries.Model.SignalType? signalType = signalTypeTableOperations.QueryRecordWhere("Acronym = {0}", "ALRM");

        if (cancellationToken.IsCancellationRequested)
            return Ok();

        if (alarmRecord.CreateAssociatedMeasurement)
        {
            TableOperations<Gemstone.Timeseries.Model.Measurement> measurementTableOperations = new(connection);
            newMeasurement = measurementTableOperations.NewRecord()!;
            newMeasurement.PointTag = alarmRecord.TagName;
            newMeasurement.SignalTypeID = signalType.ID;
            measurementTableOperations.AddNewRecord(newMeasurement);
        }

        if(newMeasurement is not null)
        {
            alarmRecord.SignalID = newMeasurement.SignalID;
        }

        TableOperations<Alarm> tableOperations = new(connection);
        tableOperations.AddNewRecord(alarmRecord);
        Alarm? foundRecord = tableOperations.QueryRecord(tableOperations.GetNonPrimaryFieldRecordRestriction(alarmRecord));

        return Ok(foundRecord ?? alarmRecord);
    }
}