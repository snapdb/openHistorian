using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Timeseries.Model;
using Gemstone.Data.Model;
using Gemstone.Data;
using Gemstone.Numeric.EE;

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
    [HttpPost, Route("ConnectionParameters/{id:int?}")]
    public IActionResult GetConnectionParameters(Device detail)
    {
        return Ok(new List<object>());
    }

    /// <summary>
    /// Override for the update (PATCH) functionality.
    /// </summary>
    /// <param name="record">Device record to be updated.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>An IActionResult containing the updated device record.</returns>
    [HttpPatch, Route("")]
    public override async Task<IActionResult> Patch([FromBody] Device record, CancellationToken cancellationToken)
    {
        await using AdoDataConnection connection = CreateConnection();
        TableOperations<Measurement> measurementTableOperations = new(connection);
        TableOperations<Gemstone.Timeseries.Model.SignalType> signalTypeOperations = new(connection);
        TableOperations<Device> deviceTableOperations = new(connection);
        RecordRestriction deviceRestriction = deviceTableOperations.GetNonPrimaryFieldRecordRestriction(record);

        //this is only place to have cancellation token..
        Device? storedRecord = await deviceTableOperations.QueryRecordWhereAsync("ID = {0}", cancellationToken, record.ID).ConfigureAwait(false);
        if (cancellationToken.IsCancellationRequested)
            return Ok();

        IActionResult patchResponse = await Patch(record, new CancellationToken());

        //return patchresponse immediately if it wasnt successfull
        if (!(patchResponse is OkResult) && !(patchResponse is OkObjectResult))
            return patchResponse;

        if (record.Acronym == storedRecord.Acronym)
            return Ok();

        List<Measurement?> measurementList = measurementTableOperations.QueryRecordsWhere("DeviceID = {0} AND Manual = 0", record.ID).ToList();

        foreach (Measurement measurement in measurementList)
        {
            if(measurement == null) continue;
            Gemstone.Timeseries.Model.SignalType? signalType = signalTypeOperations.QueryRecordWhere("ID = {0}", measurement.SignalTypeID);
            int? phasorSourceIndex = measurement.PhasorSourceIndex;

            if (signalType == null || phasorSourceIndex == null) continue;

            SignalKind signalKind = signalType.Suffix.ParseSignalKind();

            string newSignalReference = SignalReference.ToString(record.Acronym, signalKind, (int)phasorSourceIndex);
            measurement.SignalReference = newSignalReference;
            measurementTableOperations.UpdateRecord(measurement);
        }

        return Ok(record);
    }
}
