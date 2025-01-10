using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Timeseries.Adapters;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;
using System.Net;

namespace openHistorian.WebUI.Controllers;

public class InputAdaptersController : AdaptersControllerBase<IInputAdapter, CustomInputAdapter>;

public class FilterAdaptersController : AdaptersControllerBase<IFilterAdapter, CustomFilterAdapter>;

public class ActionAdaptersController : AdaptersControllerBase<IActionAdapter, CustomActionAdapter>;

public class OutputAdaptersController : AdaptersControllerBase<IOutputAdapter, CustomOutputAdapter>;

[Route("api/[controller]")]
[ApiController]
public class AdapterAdminController : Controller
{
    /// <summary>
    /// Reloads the adapter types.
    /// </summary>
    [HttpGet, Route("ReloadAdapterTypes")]
    public IActionResult ReloadAdapterTypes()
    {
        // TODO: Force check for admin role only
        AdapterCache.ReloadAdapterTypes();
        return Ok();
    }
}

[Route("api/[controller]")]
[ApiController]
public abstract class AdaptersControllerBase<TIAdapter, TAdapterModel> : 
    ModelController<TAdapterModel> 
    where TIAdapter : IAdapter
    where TAdapterModel : CustomAdapterBase, new()
{
    /// <summary>
    /// Gets a collection of all adapters and their associated information.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="IEnumerable{AdapterInfo}"/> of all adapters.</returns>
    [HttpGet, Route("All")]
    public IActionResult GetAllAdaptersDetail()
    {
        if (!GetAuthCheck())
            return Unauthorized();

        return Ok(AdapterCache<TIAdapter>.GetAdapters());
    }

    /// <summary>
    /// Gets all assemblies holding adapters.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="IEnumerable{ValueLabel}"/> of the assemblies.</returns>
    [HttpGet, Route("AssemblyNames")]
    public IActionResult GetAssemblies()
    {
        if (!GetAuthCheck())
            return Unauthorized();

        return Ok(AdapterCache<TIAdapter>
            .GetAdapters().Select(adapter => new ValueLabel
            {
                Value = adapter.AssemblyName,
                Label = adapter.AssemblyName
            })
            .DistinctBy(label => label.Value)
            .OrderBy(label => label.Label));
    }

    /// <summary>
    /// Gets all adapters from a specified Assembly.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="IEnumerable{ValueLabel}"/> of the Types.</returns>
    [HttpGet, Route("Types/{assemblyName}")]
    public IActionResult GetTypes(string assemblyName)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        assemblyName = WebUtility.UrlDecode(assemblyName);

        return Ok(AdapterCache<TIAdapter>.GetAdapters()
            .Where(adapter => string.Equals(adapter.AssemblyName, assemblyName, StringComparison.OrdinalIgnoreCase))
            .Select(adapter => new ValueLabel
            {
                Value = adapter.TypeName,
                Label = adapter.AdapterName
            })
            .DistinctBy(label => label.Value)
            .OrderBy(label => label.Label));
    }

    /// <summary>
    /// Gets all <see cref="ConnectionParameter"/> from a specified Type and Assembly.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="ConnectionParameter"/> for the specified Adapter.</returns>
    [HttpGet, Route("{typeName}/{assemblyName}/ConnectionParameters/{id:int?}")]
    public async Task<IActionResult> GetConnectionStringParameters(string assemblyName, string typeName, CancellationToken cancellationToken, int? id = null)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        string connectionString = string.Empty;

        if (id is not null)
        {
            await using AdoDataConnection connection = CreateConnection();
            TableOperations<TAdapterModel> tableOperations = new(connection);
            TAdapterModel? record = await tableOperations.QueryRecordWhereAsync($"{PrimaryKeyField} = {{0}}", cancellationToken, id);

            if (record is not null)
                connectionString = record.ConnectionString;
        }

        assemblyName = WebUtility.UrlDecode(assemblyName);
        typeName = WebUtility.UrlDecode(typeName);

        if (!AdapterCache<TIAdapter>.AssemblyTypes.TryGetValue((assemblyName, typeName), out Type? adapterType))
            return NotFound();

        if (!AdapterCache<TIAdapter>.AllAdapters.TryGetValue(adapterType, out AdapterInfo? adapter))
            return NotFound();

        ConnectionParameter[] parameters = adapter.Parameters;

        if (!string.IsNullOrWhiteSpace(connectionString))
            parameters.ApplyConnectionString(connectionString);

        return Ok(parameters);
    }

    /// <summary>
    /// Sets the ConnectionString of a <see cref="TAdapterModel"/> based on a <see cref="IEnumerable{ConnectionParameter}"/>
    /// </summary>
    [HttpPost, Route("{id:int}/SetConnectionString")]
    public async Task<IActionResult> SetConnectionString(int id, [FromBody] IEnumerable<ConnectionParameter> parameters, CancellationToken cancellationToken)
    {
        if (!PatchAuthCheck())
            return Unauthorized();

        await using AdoDataConnection connection = CreateConnection();
        TableOperations<TAdapterModel> tableOperations = new(connection);
        TAdapterModel? record = await tableOperations.QueryRecordWhereAsync($"{PrimaryKeyField} = {{0}}", cancellationToken, id);

        if (record is null)
            return NotFound();

        record.ConnectionString = parameters.ToConnectionString();

        await tableOperations.UpdateRecordAsync(record, cancellationToken);

        return Ok(record);
    }

    /// <summary>
    /// Retrieves the JavaScript module containing UI resources for the specified adapter.
    /// </summary>
    /// <returns> An <see cref="IActionResult"/> containing the JavaScript module required by the adapter's UI. </returns>
    [HttpGet, Route("Components/{typeName}/{assemblyName}/{resourceID}")]
    public IActionResult GetAdapterResource(string assemblyName, string typeName, string resourceID)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        assemblyName = WebUtility.UrlDecode(assemblyName);
        typeName = WebUtility.UrlDecode(typeName);
        resourceID = WebUtility.UrlDecode(resourceID);

        if (!AdapterCache<TIAdapter>.AssemblyTypes.TryGetValue((assemblyName, typeName), out Type? adapterType))
            return NotFound();

        if (!AdapterCache<TIAdapter>.UIResources.TryGetValue(adapterType, out UIResourceInfo? resourceInfo))
            return NotFound();

        if (!resourceInfo.AttributeMap.TryGetValue(resourceID, out UIResourceAttribute? resourceAttribute))
            return NotFound();

        using Stream? stream = resourceAttribute.GetResourceStream();

        if (stream is null)
            return NotFound();

        return File(stream, "text/javascript");
    }
}
