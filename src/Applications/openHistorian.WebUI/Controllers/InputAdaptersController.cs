using openHistorian.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Gemstone.Data.Model;
using Gemstone.Timeseries.Adapters;
using System.Net;

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
                Value = adapter.Type,
                Label = adapter.Header
            }).DistinctBy((v) => v.Value).OrderBy(adapter => adapter.Label)));
    }


} 
