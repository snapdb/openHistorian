using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Numeric.EE;
using Gemstone.Timeseries.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Adapters;
using ServiceInterface;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeviceController : ReloadController<Device>
{
    private readonly IServiceCommands m_serviceCommands = WebServer.ServiceCommands;

    /// <summary>
    /// Creates new records in device table.
    /// </summary>
    /// <param name="record">The records to be created.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing the new <see cref="T"/> or <see cref="Exception"/>.</returns>
    [HttpPost, Route("BulkAdd")]
    public async Task<IActionResult> BulkAdd([FromBody] Device[] records, CancellationToken cancellationToken)
    {
        if (!PostAuthCheck())
            return Unauthorized();

        await using AdoDataConnection connection = CreateConnection();
        TableOperations<Device> tableOperations = new(connection);

        foreach(Device record in records)
        {
            await tableOperations.AddNewRecordAsync(record, cancellationToken);
        }

        m_serviceCommands.ReloadConfig();

        return Ok(1);
    }

    /// <summary>
    /// Deletes a record from associated table.
    /// </summary>
    /// <param name="record">The record to be deleted.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing 1 or <see cref="Exception"/>.</returns>
    [HttpDelete, Route("")]
    public override async Task<IActionResult> Delete([FromBody] Device record, CancellationToken cancellationToken)
    {
        if (!DeleteAuthCheck())
            return Unauthorized();

        await using AdoDataConnection connection = CreateConnection();
        TableOperations<Device> tableOperations = new(connection);

        List<Device?> children = tableOperations.QueryRecordsWhere("ParentID = {0}", record.ID).ToList();

        foreach (Device? child in children)
        {
            if (child is not null)
                tableOperations.DeleteRecord(child);
        }

        return await base.Delete(record, cancellationToken).ConfigureAwait(false);
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

        Device? storedRecord = await deviceTableOperations.QueryRecordWhereAsync("ID = {0}", cancellationToken, record.ID).ConfigureAwait(false);
        if (cancellationToken.IsCancellationRequested)
            return Ok(record);

        IActionResult patchResponse = await base.Patch(record, new CancellationToken());

        //return patchresponse immediately if it wasnt successfull
        if (!(patchResponse is OkResult) && !(patchResponse is OkObjectResult))
            return patchResponse;

        if (record.Acronym == storedRecord.Acronym)
            return Ok(record);

        List<Measurement?> measurementList = measurementTableOperations.QueryRecordsWhere("DeviceID = {0} AND Manual = 0", record.ID).ToList();

        foreach (Measurement measurement in measurementList)
        {
            if (measurement == null) continue;
            Gemstone.Timeseries.Model.SignalType? signalType = signalTypeOperations.QueryRecordWhere("ID = {0}", measurement.SignalTypeID);
            int? phasorSourceIndex = measurement.PhasorSourceIndex;

            if (signalType == null || phasorSourceIndex == null) continue;

            SignalKind signalKind = signalType.Suffix.ParseSignalKind();

            string newSignalReference = SignalReference.ToString(record.Acronym, signalKind, (int)phasorSourceIndex);
            measurement.SignalReference = newSignalReference;
            measurementTableOperations.UpdateRecord(measurement);
        }

        m_serviceCommands.ReloadConfig();

        return Ok(record);
    }
}
