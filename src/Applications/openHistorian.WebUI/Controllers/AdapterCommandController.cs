using Gemstone.Timeseries.Adapters;
using Microsoft.AspNetCore.Mvc;
using ServiceInterface;
using System.Net;
using System.Reflection;
using ConnectionStringParser = Gemstone.Configuration.ConnectionStringParser<Gemstone.Timeseries.Adapters.ConnectionStringParameterAttribute>;

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
    private readonly IServiceCommands m_serviceCommands = WebServer.ServiceCommands;

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
    /// Executes a static command on an adapter.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("Execute/{assemblyName}/{typeName}/{command}")]
    public IActionResult Execute(string assemblyName, string typeName, string command)
    {
        IActionResult result = TryGetCommandMethod(assemblyName, typeName, command, out _, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (!method.IsStatic)
            return BadRequest($"Command '{command}' is not a static method. Did you mean to call '{nameof(SessionExecute)}' instead?");

        return (IActionResult)method.Invoke(null, GetMethodArguments(method))!;
    }

    /// <summary>
    /// Executes a static command on an adapter with parameters.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("Execute/{assemblyName}/{typeName}/{command}/{*parameters}")]
    public IActionResult Execute(string assemblyName, string typeName, string command, string parameters)
    {
        IActionResult result = TryGetCommandMethod(assemblyName, typeName, command, out _, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (!method.IsStatic)
            return BadRequest($"Command '{command}' is not a static method. Did you mean to call '{nameof(SessionExecute)}' instead?");

        return (IActionResult)method.Invoke(null, GetMethodArguments(method, parameters))!;
    }

    /// <summary>
    /// Executes a parameterless command on an active adapter instance in the Iaon session.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("InstanceExecute/{runtimeID}/{command}")]
    public IActionResult SessionExecute(uint runtimeID, string command)
    {
        IAdapter adapter = m_serviceCommands.GetActiveAdapterInstance(runtimeID);

        IActionResult result = TryGetCommandMethod(adapter.GetType(), command, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (method.IsStatic)
            return BadRequest($"Command '{command}' is not an instance method. Did you mean to call '{nameof(Execute)}' instead?");

        return (IActionResult)method.Invoke(adapter, GetMethodArguments(method))!;
    }

    /// <summary>
    /// Executes a command on an active adapter instance in the Iaon session with parameters.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("InstanceExecute/{runtimeID}/{command}/{*parameters}")]
    public IActionResult SessionExecute(uint runtimeID, string command, string parameters)
    {
        IAdapter adapter = m_serviceCommands.GetActiveAdapterInstance(runtimeID);

        IActionResult result = TryGetCommandMethod(adapter.GetType(), command, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (method.IsStatic)
            return BadRequest($"Command '{command}' is not an instance method. Did you mean to call '{nameof(Execute)}' instead?");

        return (IActionResult)method.Invoke(adapter, GetMethodArguments(method, parameters))!;
    }

    /*

    -- Instance based commands that operate independently of Iaon session using a connection string for context are disabled for now

    /// <summary>
    /// Executes a parameterless command on an active adapter instance in the Iaon session.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("InstanceExecute/{runtimeID}/{command}")]
    public IActionResult SessionExecute(uint runtimeID, string command)
    {
        IAdapter adapter = m_serviceCommands.GetActiveAdapterInstance(runtimeID);

        IActionResult result = TryGetCommandMethod(adapter.GetType(), command, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (method.IsStatic)
            return BadRequest($"Command '{command}' is not an instance method. Did you mean to call '{nameof(Execute)}' instead?");

        return (IActionResult)method.Invoke(adapter, GetMethodArguments(method))!;
    }

    /// <summary>
    /// Executes a command on an active adapter instance in the Iaon session with parameters.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("InstanceExecute/{runtimeID}/{command}/{*parameters}")]
    public IActionResult SessionExecute(uint runtimeID, string command, string parameters)
    {
        IAdapter adapter = m_serviceCommands.GetActiveAdapterInstance(runtimeID);

        IActionResult result = TryGetCommandMethod(adapter.GetType(), command, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (method.IsStatic)
            return BadRequest($"Command '{command}' is not an instance method. Did you mean to call '{nameof(Execute)}' instead?");

        return (IActionResult)method.Invoke(adapter, GetMethodArguments(method, parameters))!;
    }

    /*

    -- Instance based commands that operate independently of Iaon session using a connection string for context are disabled for now

    /// <summary>
    /// Executes a command on an adapter instance, applying connection string values if provided.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("InstanceExecute/{assemblyName}/{typeName}/{command}")]
    public IActionResult InstanceExecute(string assemblyName, string typeName, string command, [FromBody] string connectionString)
    {
        IActionResult result = TryGetCommandMethod(assemblyName, typeName, command, out AdapterInfo info, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (method.IsStatic)
            return BadRequest($"Command '{command}' is not an instance method. Did you mean to call '{nameof(Execute)}' instead?");

        object? instance = Activator.CreateInstance(info.Type);

        if (instance is null)
            return BadRequest("Failed to create adapter instance");

        ApplyConnectionString(connectionString, instance);

        return (IActionResult)method.Invoke(instance, GetMethodArguments(method))!;
    }

    /// <summary>
    /// Executes a command on an adapter instance with parameters, applying connection string values if provided.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the result of the command.</returns>
    [HttpPost, Route("InstanceExecute/{assemblyName}/{typeName}/{command}/{*parameters}")]
    public IActionResult InstanceExecute(string assemblyName, string typeName, string command, string parameters, [FromBody] string connectionString)
    {
        IActionResult result = TryGetCommandMethod(assemblyName, typeName, command, out AdapterInfo info, out MethodInfo method);

        if (result is not OkResult)
            return result;

        if (!method.IsStatic)
            return BadRequest($"Command '{command}' is not an instance method. Did you mean to call '{nameof(Execute)}' instead?");

        object? instance = Activator.CreateInstance(info.Type);

        if (instance is null)
            return BadRequest("Failed to create adapter instance");

        ApplyConnectionString(connectionString, instance);

        return (IActionResult)method.Invoke(instance, GetMethodArguments(method, parameters))!;
    }
    */

    private IActionResult TryGetCommandMethod(string assemblyName, string typeName, string command, out AdapterInfo info, out MethodInfo method)
    {
        assemblyName = WebUtility.UrlDecode(assemblyName);
        typeName = WebUtility.UrlDecode(typeName);
        command = WebUtility.UrlDecode(command);
        info = default!;
        method = default!;

        return !AdapterCache<TIAdapter>.AssemblyTypes.TryGetValue((assemblyName, typeName), out Type? adapterType) ? 
            NotFound() : 
            TryGetCommandMethod(adapterType, command, out method);
    }

    private IActionResult TryGetCommandMethod(Type adapterType, string command, out MethodInfo method)
    {
        method = default!;

        if (!AdapterCache<TIAdapter>.AdapterCommands.TryGetValue(adapterType, out AdapterCommandInfo? commandInfo))
            return NotFound();

        AdapterInfo info = commandInfo.Info;

        if (!commandInfo.MethodAttributeMap.TryGetValue(command, out (MethodInfo, AdapterCommandAttribute) methodAttribute))
            return NotFound();

        (method, AdapterCommandAttribute attribute) = methodAttribute;

        // Verify user is in allowed roles for command
        if (!attribute.AllowedRoles.Any(User.IsInRole))
            return Unauthorized();

        // Verify method return type is IActionResult
        if (method.ReturnType != typeof(IActionResult))
            return BadRequest("Command method must return IActionResult");

        // Verify first parameter is of type ControllerBase
        if (method.GetParameters().Length == 0 || method.GetParameters()[0].ParameterType != typeof(ControllerBase))
            return BadRequest("First parameter of command method must be ControllerBase");

        return Ok();
    }

    private object?[] GetMethodArguments(MethodInfo method, string? parameters = null)
    {
        // Parse URL parameters into method arguments
        string[] parameterValues = (parameters?.Split('/').Select(WebUtility.UrlDecode).ToArray() ?? [])!;
        ParameterInfo[] methodParameters = method.GetParameters();

        if (parameterValues.Length != methodParameters.Length - 1)
            throw new ArgumentException("Invalid number of parameters");

        // First parameter is always the controller instance
        object?[] methodArguments = new object[methodParameters.Length];

        methodArguments[0] = this;

        for (int i = 1; i < methodParameters.Length; i++)
        {
            ParameterInfo parameter = methodParameters[i];
            methodArguments[i] = Convert.ChangeType(parameterValues[i - 1], parameter.ParameterType);
        }

        return methodArguments;
    }

    private static void ApplyConnectionString(string connectionString, object instance)
    {
        ConnectionStringParser parser = new();
        parser.ParseConnectionString(connectionString, instance);
    }
}
