using openHistorian.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Data.Model;
using Gemstone.Timeseries.Adapters;
using System.Net;
using Gemstone.Data;

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

        return await Task<IActionResult>.Run(() =>
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

        return await Task<IActionResult>.Run(() =>
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
    [HttpGet, Route("/{typeName}/{assemblyName}/ConnectionParameters/{id:int?}")]
    public async Task<IActionResult> GetConnectionStringParameters(string assemblyName, string typeName,CancellationToken cancellationToken, int? id = null)
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
        return Ok(AdapterCollectionHelper< IInputAdapter>.GetConnectionParameters(assemblyName,typeName,connectionString));
    }

    public async Task<IActionResult> ProxyToAdapter(string assemblyName, string typeName, string resourceName, CancellationToken cancellationToken)
    {
        if (!GetAuthCheck())
            return Unauthorized();

        return await Task<IActionResult>.Run(() =>
        {
            Func<ControllerBase,IActionResult> adapterMethod = AdapterCollectionHelper<IInputAdapter>.GetUIResources(assemblyName, typeName)[resourceName];
            if (adapterMethod is null)
                return NotFound();

            return adapterMethod.Invoke(this);
        });
    }
} 
