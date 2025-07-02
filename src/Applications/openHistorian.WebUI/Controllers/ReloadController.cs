using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using ServiceInterface;

namespace openHistorian.WebUI.Controllers;

public abstract class ReloadController<T> : ModelController<T> where T : class, new()
{
    private readonly IServiceCommands m_serviceCommands = WebServer.ServiceCommands;

    /// <summary>
    /// Updates a record from associated table and reloads configuration.
    /// </summary>
    /// <param name="record">The record to be updated.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing the new record <see cref="T"/> or <see cref="Exception"/>.</returns>
    [HttpPatch, Route("")]
    public override async Task<IActionResult> Patch([FromBody] T record, CancellationToken cancellationToken)
    {
        IActionResult result = await base.Patch(record, cancellationToken);

        m_serviceCommands.ReloadConfig();

        return result;
    }

    /// <summary>
    /// Creates new record in associated table and reloads configuration.
    /// </summary>
    /// <param name="record">The record to be created.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing the new <see cref="T"/> or <see cref="Exception"/>.</returns>
    [HttpPost, Route("")]
    public override async Task<IActionResult> Post([FromBody] T record, CancellationToken cancellationToken)
    {
        IActionResult result = await base.Post(record, cancellationToken);

        m_serviceCommands.ReloadConfig();

        return result;
    }

    /// <summary>
    /// Deletes a record from associated table by primary key and reloads configuration.
    /// </summary>
    /// <param name="id">The primary key of the record to be deleted.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>An <see cref="IActionResult"/> containing 1 or <see cref="Exception"/>.</returns>
    [HttpDelete, Route("{id}")]
    public override async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        IActionResult result = await base.Delete(id, cancellationToken);

        m_serviceCommands.ReloadConfig();

        return result;
    }

}
