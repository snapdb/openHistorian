using Gemstone.Timeseries.Adapters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;

namespace openHistorian.WebUI.Controllers;

public class InputAdapterCommandController : AdapterCommandControllerBase<IInputAdapter>;

public class FilterAdapterCommandController : AdapterCommandControllerBase<IFilterAdapter>;

public class ActionAdapterCommandController : AdapterCommandControllerBase<IActionAdapter>;

public class OutputAdapterCommandController : AdapterCommandControllerBase<IOutputAdapter>;

[Route("api/[controller]")]
[ApiController]
public class AdapterCommandControllerBase<TIAdapter> :
    Controller
    where TIAdapter : IAdapter
{
    /// <summary>
    /// Gets a collection of all protocols and their associated information.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing an <see cref="IEnumerable{AdapterInfo}"/> of all adapters.</returns>
    [HttpGet, Route("All")]
    public IActionResult GetAllAdaptersDetail()
    {
        return Ok(AdapterCache.AdapterCommands.Values.Select(command => new
        {
            command.Info,
            Attributes = command.MethodAttributes.Select(item => new
            {
                item.method.Name,
                item.attribute.Description,
                ReturnType = item.method.ReturnType.Name,
                Parameters = string.Join(", ", item.method.GetParameters().Select(param => $"{param.Name}: {param.ParameterType.Name}")),
                item.attribute.AllowedRoles,
            }
        )}));
    }

    /// <summary>
    /// Executes a command on an adapter.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpGet, Route("Execute/{typeName}/{assemblyName}/{command}")]
    public IActionResult Execute(string assemblyName, string typeName, string command)
    {
        IActionResult result = TryGetCommandMethod(assemblyName, typeName, command, out MethodInfo method);

        if (result is not OkResult)
            return result;

        return (IActionResult)method.Invoke(null, null)!;
    }

    /// <summary>
    /// Executes a command on an adapter with parameters.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpGet, Route("Execute/{typeName}/{assemblyName}/{command}/{*parameters}")]
    public IActionResult Execute(string assemblyName, string typeName, string command, string parameters)
    {
        IActionResult result = TryGetCommandMethod(assemblyName, typeName, command, out MethodInfo method);

        if (result is not OkResult)
            return result;

        // Parse URL parameters into method arguments
        string[] parameterValues = parameters.Split('/').Select(WebUtility.UrlDecode).ToArray()!;
        ParameterInfo[] methodParameters = method.GetParameters();

        if (parameterValues.Length != methodParameters.Length)
            return BadRequest("Invalid number of parameters");

        object?[] methodArguments = new object[methodParameters.Length];

        for (int i = 0; i < methodParameters.Length; i++)
        {
            ParameterInfo parameter = methodParameters[i];
            methodArguments[i] = Convert.ChangeType(parameterValues[i], parameter.ParameterType);
        }

        return (IActionResult)method.Invoke(null, methodArguments)!;
    }

    private IActionResult TryGetCommandMethod(string assemblyName, string typeName, string command, out MethodInfo method)
    {
        assemblyName = WebUtility.UrlDecode(assemblyName);
        typeName = WebUtility.UrlDecode(typeName);
        command = WebUtility.UrlDecode(command);
        method = default!;

        if (!AdapterCache<TIAdapter>.AssemblyTypes.TryGetValue((assemblyName, typeName), out Type? adapterType))
            return NotFound();
        
        if (!AdapterCache<TIAdapter>.AdapterCommands.TryGetValue(adapterType, out AdapterCommandInfo? commandInfo))
            return NotFound();

        if (!commandInfo.MethodAttributeMap.TryGetValue(command, out (MethodInfo, AdapterCommandAttribute) methodAttribute))
            return NotFound();

        (method, AdapterCommandAttribute attribute) = methodAttribute;

        // Verify user is in allowed roles for command
        if (!attribute.AllowedRoles.Any(User.IsInRole))
            return Unauthorized();

        // Verify method return type is IActionResult
        if (method.ReturnType != typeof(IActionResult))
            return BadRequest("Command method must return IActionResult");

        return Ok();
    }
}
