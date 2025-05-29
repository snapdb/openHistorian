using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Timeseries;
using Gemstone.Data.Model;
using Gemstone.Data;
using Gemstone.EnumExtensions;
using System.Text;
using System.Text.RegularExpressions;
using ServiceInterface;
using System;
namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlarmController : ModelController<Alarm>
{
    private readonly IServiceCommands m_serviceCommands = WebServer.ServiceCommands;

    /// <summary>
    /// Creates a new alarm record in table.
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
        Gemstone.Timeseries.Model.SignalType? alarmSignalType = signalTypeTableOperations.QueryRecordWhere("Acronym = {0}", "ALRM");

        TableOperations<Model.Historian> historianTableOperations = new(connection);
        Model.Historian? statHistorian = historianTableOperations.QueryRecordWhere("Acronym = {0}", "STAT");

        if (cancellationToken.IsCancellationRequested)
            return Ok();

        if (alarmRecord.CreateAssociatedMeasurement)
        {
            string cleanedTag = GetCleanPointTag(alarmRecord);
            TableOperations<Gemstone.Timeseries.Model.Measurement> measurementTableOperations = new(connection);
            newMeasurement = measurementTableOperations.NewRecord()!;
            newMeasurement.PointTag = cleanedTag;
            newMeasurement.SignalTypeID = alarmSignalType.ID;
            newMeasurement.SignalReference = $"{cleanedTag}-{alarmSignalType.Suffix}{0}";
            newMeasurement.HistorianID = statHistorian?.ID;
            measurementTableOperations.AddNewRecord(newMeasurement);
        }

        if (newMeasurement is not null)
        {
            alarmRecord.SignalID = newMeasurement.SignalID;
        }

        TableOperations<Alarm> tableOperations = new(connection);
        tableOperations.AddNewRecord(alarmRecord);
        Alarm? foundRecord = tableOperations.QueryRecord(tableOperations.GetNonPrimaryFieldRecordRestriction(alarmRecord));
        m_serviceCommands.ReloadConfig();
        return Ok(foundRecord ?? alarmRecord);
    }

    /// <summary>
    /// Updates an alarm record
    /// </summary>
    /// <param name="record">The record to be updated.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing the new record <see cref="T"/> or <see cref="Exception"/>.</returns>
    [HttpPatch, Route("")]
    public override async Task<IActionResult> Patch([FromBody] Alarm record, CancellationToken cancellationToken)
    {
        IActionResult result = await base.Patch(record, cancellationToken);
        m_serviceCommands.ReloadConfig();
        return result;
    }

    /// <summary>
    /// Gets a clean point tag.
    /// </summary>
    /// <param name="pointTag">Point tag.</param>
    /// <returns>Clean point tag.</returns>
    private static string GetCleanPointTag(Alarm alarmRecord)
    {
        StringBuilder pointTag = new("AL-");
        switch (alarmRecord.Severity)
        {
            case AlarmSeverity.None:
                pointTag.Append("EXEMPT");
                break;
            case AlarmSeverity.Unreasonable:
                switch (alarmRecord.Operation)
                {
                    case AlarmOperation.GreaterThan:
                    case AlarmOperation.GreaterOrEqual:
                        pointTag.Append("HIGH");
                        break;
                    case AlarmOperation.LessThan:
                    case AlarmOperation.LessOrEqual:
                        pointTag.Append("LOW");
                        break;
                    default:
                        pointTag.Append(alarmRecord.Severity.GetDescription());
                        break;
                }
                break;
            default:
                pointTag.Append(alarmRecord.Severity.GetDescription());
                break;
        }

        pointTag.Append(":");

        // Remove any invalid characters from point tag
        pointTag.Append(Regex.Replace(alarmRecord.TagName.ToUpperInvariant(), @"[^A-Z0-9\-\+!\:_\.@#\$]", "", RegexOptions.IgnoreCase | RegexOptions.Compiled));

        return pointTag.ToString();
    }
}