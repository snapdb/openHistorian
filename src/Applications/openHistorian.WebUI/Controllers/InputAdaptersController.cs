using openHistorian.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Data.Model;
using Gemstone.Timeseries.Adapters;
using System.Net;
using Gemstone.Data;
using Gemstone.StringExtensions;
using ServiceInterface;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InputAdaptersController : ModelController<CustomInputAdapter>
{
    /// <summary>
    /// Gets all Assemblies holding input adapters.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="IEnumerable{ValueLabel}"/> of the assemblies.</returns>
    [HttpGet, Route("AssemblyNames")]
    public async Task<IActionResult> GetAssemblies(CancellationToken cancellationToken)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        return await Task.Run(() =>
            Ok(AdapterCollectionHelper<IInputAdapter>.GetAdapters().Select(adapter => new ValueLabel()
            {
                Value = adapter.Assembly,
                Label = adapter.Assembly
            }).DistinctBy((v) => v.Value).OrderBy(adapter => adapter.Label)));    
    }

    /// <summary>
    /// Gets all input Adapters from a specified Assembly.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="IEnumerable{ValueLabel}"/> of the Types.</returns>
    [HttpGet, Route("Types/{assemblyName}")]
    public async Task<IActionResult> GetTypes(string assemblyName, CancellationToken cancellationToken)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        return await Task.Run(() =>
            Ok(AdapterCollectionHelper<IInputAdapter>.GetAdapters().Where((a) => String.Compare(a.Assembly, WebUtility.UrlDecode(assemblyName),true) == 0)
            .Select(adapter => new ValueLabel()
            {
                Value = adapter.TypeName,
                Label = adapter.Header
            }).DistinctBy((v) => v.Value).OrderBy(adapter => adapter.Label)));
    }

    /// <summary>
    /// Gets all <see cref="ConnectionParameter"> from a specified Type and Assembly.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="ConnectionParameter"/> for the specified Adapter.</returns>
    [HttpGet, Route("{encodedTypeName}/{encodedAssemblyName}/ConnectionParameters/{id:int?}")]
    public async Task<IActionResult> GetConnectionStringParameters(string encodedAssemblyName, string encodedTypeName, CancellationToken cancellationToken, int? id = null)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        string connectionString = "";

        if (id is not null)
        {
            await using AdoDataConnection connection = CreateConnection();
            TableOperations<CustomInputAdapter> tableOperations = new(connection);
            CustomInputAdapter? result = await tableOperations.QueryRecordAsync(new RecordRestriction($"{PrimaryKeyField} = {{0}}", id), cancellationToken);
            if (result is not null)
                connectionString = result.ConnectionString;
        }
        string assemblyName = WebUtility.UrlDecode(encodedAssemblyName);
        string typeName = WebUtility.UrlDecode(encodedTypeName);

        IEnumerable<ConnectionParameter> connectionParameters = AdapterCollectionHelper<IInputAdapter>.GetConnectionParameters(assemblyName, typeName, connectionString);
        return Ok(connectionParameters);
    }

    /// <summary>
    /// Sets the Connectinstring of a <see cref="CustomInputAdapter"/> based on a <see cref="IEnumerable{ConnectionParameter}"/>
    /// </summary>
    [HttpPost, Route("{id:int}/SetConnectionString")]
    public async Task<IActionResult> SetConnectionString(int id, [FromBody] IEnumerable<ConnectionParameter> parameters, CancellationToken cancellationToken)
    {
        if (!PatchAuthCheck())
            return Unauthorized();

        await using AdoDataConnection connection = CreateConnection();
        TableOperations<CustomInputAdapter> tableOperations = new(connection);
        CustomInputAdapter? result = await tableOperations.QueryRecordAsync(new RecordRestriction($"{PrimaryKeyField} = {{0}}", id), cancellationToken);

        if (result is null)
            return NotFound();

        Dictionary<string, string> settings = parameters.ToDictionary(p => p.Name, p => p.Value);

        string connectionString = settings.JoinKeyValuePairs();

        result.ConnectionString = result.ConnectionString;

        await tableOperations.UpdateRecordAsync(result, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the JavaScript module containing UI resources for the specified input adapter.
    /// </summary>
    /// <returns> An <see cref="IActionResult"/> containing the JavaScript module required by the adapter's UI. </returns>
    [HttpGet, Route("Components/{encodedTypeName}/{encodedAssemblyName}/{encodedResourceName}")]
    public async Task<IActionResult> ProxyToAdapter(string encodedAssemblyName, string encodedTypeName, string encodedResourceName, CancellationToken cancellationToken)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        string assemblyName = WebUtility.UrlDecode(encodedAssemblyName);
        string typeName = WebUtility.UrlDecode(encodedTypeName);
        string resourceName = WebUtility.UrlDecode(encodedResourceName);

        return await Task.Run(() =>
        {
            Dictionary<string, Func<ControllerBase, IActionResult>> adapterDictionary = AdapterCollectionHelper<IInputAdapter>.GetUIResources(typeName, assemblyName);
            Func<ControllerBase,IActionResult>? adapterMethod = adapterDictionary.GetValueOrDefault(resourceName);
            if (adapterMethod is null)
                return NotFound();

            return adapterMethod.Invoke(this);
        });
    }
} 
