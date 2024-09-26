
// Example of a model controller for the Connection Model

//using System.Reflection;
//using Gemstone.Web.APIController;
//using Gemstone.Data.Model;
//using Microsoft.AspNetCore.Mvc;
//using ServiceInterface;
//using System.Text.RegularExpressions;
//using openHistorian.WebUI;

//namespace StreamSplitter.WebUI.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class ConnectionController : ControllerBase, IModelController<Connection>
//{
//    private readonly IServiceCommands m_serviceCommands = WebServer.ServiceCommands;
//    private const int ConPerPage = 25;

//    /// <summary>
//    /// Gets all records from associated table.
//    /// </summary>
//    /// <param name="parentID">Parent ID is not used by this Controller. </param>
//    /// <param name="page">the 0 based page number to be returned.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing <see cref="IEnumerable{Connection}"/> or <see cref="Exception"/>.</returns>
//    public Task<IActionResult> Get(string parentID, int page, CancellationToken cancellationToken)
//    {
//        return Get(nameof(Connection.ID), false, page, cancellationToken);
//    }

//    /// <summary>
//    /// Gets all records from associated table sorted by the provided field.
//    /// </summary>
//    /// <param name="sort"> Field to be used for sorting.</param>
//    /// <param name="ascending"> true t sort by ascending.</param>
//    /// <param name="page">the 0 based page number to be returned.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing <see cref="IEnumerable{Connection}"/> or <see cref="Exception"/>.</returns>
//    [HttpGet, Route("{page:min(0)}/{sort}/{ascending:bool}")]
//    public async Task<IActionResult> Get(string sort, bool ascending, int page, CancellationToken cancellationToken)
//    {
//        int start = (page - 1) * ConPerPage;
        
//        return Ok(ascending ? 
//            (await GetConnections(cancellationToken)).OrderBy(GetKeySelector(sort)).Skip(start).Take(ConPerPage).ToList() : 
//            (await GetConnections(cancellationToken)).OrderByDescending(GetKeySelector(sort)).Skip(start).Take(ConPerPage).ToList());
//    }

//    /// <summary>
//    /// Gets all records from associated table sorted by the provided field.
//    /// </summary>
//    /// <param name="sort"> Field to be used for sorting.</param>
//    /// <param name="ascending"> true t sort by ascending.</param>
//    /// <param name="page">the 0 based page number to be returned.</param>
//    /// <param name="parentID">Parent ID is not used by this Controller.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing <see cref="IEnumerable{Connection}"/> or <see cref="Exception"/>.</returns>
//    public Task<IActionResult> Get(string parentID, string sort, bool ascending, int page, CancellationToken cancellationToken)
//    {
//        return Get(sort, ascending, page, cancellationToken);
//    }

//    /// <summary>
//    /// Gets a single record from associated table.
//    /// </summary>
//    /// <param name="id"> The PrimaryKey value of the Model to be returned.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing a <see cref="Connection"/> or <see cref="Exception"/>.</returns>
//    [HttpGet, Route("One/{id}")]
//    public async Task<IActionResult> GetOne(string id, CancellationToken cancellationToken)
//    {
//        Connection? connection = (await GetConnections(cancellationToken)).FirstOrDefault(c => c.ID == new Guid(id));
//        return connection is null ? NotFound() : Ok(connection);
//    }

//    /// <summary>
//    /// Gets the pagination information for the provided search criteria.
//    /// </summary>
//    /// <param name="postData">Search criteria.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>A <see cref="PageInfo"/> object containing the pagination information or <see cref="Exception"/>.</returns>
//    [HttpPost, Route("PageInfo")]
//    public async Task<IActionResult> GetPageInfo(SearchPost<Connection> postData, CancellationToken cancellationToken)
//    {
//        IEnumerable<Connection> connections = (await GetConnections(cancellationToken)).Where(c => postData.Searches.All(s => ApplyFilter(s, c)));
//        int count = connections.Count();

//        return Ok(new PageInfo
//        {
//            PageCount = (int)Math.Ceiling((double)count / ConPerPage),
//            PageSize = ConPerPage,
//            TotalCount = count
//        });
//    }

//    /// <summary>
//    /// Gets the pagination information.
//    /// </summary>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>A <see cref="PageInfo"/> object containing the pagination information or <see cref="Exception"/>.</returns> 
//    [HttpGet, Route("PageInfo")]
//    public async Task<IActionResult> GetPageInfo(CancellationToken cancellationToken)
//    {
//        IEnumerable<Connection> connections = await GetConnections(cancellationToken);
//        int count = connections.Count();

//        return Ok(new PageInfo
//        {
//            PageCount = (int)Math.Ceiling((double)count / ConPerPage),
//            PageSize = ConPerPage,
//            TotalCount = count
//        });
//    }

//    /// <summary>
//    /// Gets all records from associated table matching the provided search criteria.
//    /// </summary>
//    /// <param name="postData">The search criteria.</param>
//    /// <param name="page">The zero-based page number to be returned.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing <see cref="IEnumerable{Connection}"/> or <see cref="Exception"/>.</returns>
//    [HttpPost, Route("Search/{page:min(0)}")]
//    public async Task<IActionResult> Search(SearchPost<Connection> postData, int page, CancellationToken cancellationToken)
//    {
//        int start = (page - 1) * ConPerPage;

//        if (!postData.Ascending)
//        {
//            return Ok((await GetConnections(cancellationToken)).OrderByDescending(GetKeySelector(postData.OrderBy))
//                .Where(c => postData.Searches.All(s => ApplyFilter(s, c)))
//                .Skip(start).Take(ConPerPage).ToList());
//        }
        
//        IOrderedEnumerable<Connection> connections = (await GetConnections(cancellationToken)).OrderBy(GetKeySelector(postData.OrderBy));
//        IEnumerable<Connection> filteredConnections = connections.Where(c => postData.Searches.All(s => ApplyFilter(s, c)));
//        List<Connection> pageOfConnections = filteredConnections.Skip(start).Take(ConPerPage).ToList();

//        return Ok(pageOfConnections);
//    }

//    /// <summary>
//    /// Updates a record from associated table.
//    /// </summary>
//    /// <param name="record">The record to be updated.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing the new record <see cref="Connection"/> or <see cref="Exception"/>.</returns>
//    [HttpPatch, Route("")]
//    public async Task<IActionResult> Patch(Connection record, CancellationToken cancellationToken)
//    {
//        await m_serviceCommands.UploadConnections([record], cancellationToken);
//        return Ok(record);
//    }

//    /// <summary>
//    /// creates new records in associated table.
//    /// </summary>
//    /// <param name="record"> The record to be created.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing the new <see cref="Connection"/> or <see cref="Exception"/>.</returns>
//    [HttpPost, Route("")]
//    public async Task<IActionResult> Post(Connection record, CancellationToken cancellationToken)
//    {
//        record.ID = Guid.NewGuid();
//        await m_serviceCommands.UploadConnections([record], cancellationToken);
//        return Ok(record);
//    }

//    /// <summary>
//    /// Deletes a record from associated table.
//    /// </summary>
//    /// <param name="record">The record to be deleted.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing 1 or <see cref="Exception"/>.</returns>
//    [HttpDelete, Route("")]
//    public Task<IActionResult> Delete(Connection record, CancellationToken cancellationToken)
//    {
//        return Delete(record.ID.ToString(), cancellationToken);
//    }

//    /// <summary>
//    /// Deletes a record from associated table
//    /// </summary>
//    /// <param name="id">The primary key of the record to be deleted.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing 1 or <see cref="Exception"/>.</returns>
//    [HttpDelete, Route("{id}")]
//    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
//    {
//        IEnumerable<Connection> connections = (await m_serviceCommands.DownloadActiveConfiguration(cancellationToken))
//            .Where(c => c.ID != new Guid(id));

//        await m_serviceCommands.UploadActiveConfiguration(connections, cancellationToken);

//        return Ok(1);
//    }

//    /// <summary>
//    /// Gets the console messages for a specific connection.
//    /// </summary>
//    /// <param name="id">ID of the connection to get console messages for.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>Returns the console messages for the connection.</returns>
//    [HttpGet, Route("console/{id}")]
//    public IActionResult GetConsoleMessages(string id, CancellationToken cancellationToken)
//    {
//        IStreamProxyStatus? status = m_serviceCommands.GetStreamProxyStatus().FirstOrDefault(cs => cs.ID == new Guid(id));

//        if (status is null)
//            return NotFound();

//        return Ok(status.RecentStatusMessages);
//    }

//    /// <summary>
//    /// Gets all connection statuses.
//    /// </summary>
//    /// <returns>All connection statuses.</returns>
//    [HttpGet, Route("statuses")]
//    public IActionResult GetAllStatus()
//    {
//        return Ok(m_serviceCommands.GetStreamProxyStatus().Select(s => new ConnectionStatus
//        {
//            ID = s.ID,
//            Name = s.Name,
//            Status = s.ConnectionState
//        }));

//    }

//    /// <summary>
//    /// Gets the status of a specific connection.
//    /// </summary>
//    /// <param name="id">ID of the connection to get the status for.</param>
//    /// <returns>Status of the connection.</returns>
//    [HttpGet, Route("status/{id}")]
//    public IActionResult GetStatus(string id)
//    {
//        IStreamProxyStatus? status = m_serviceCommands.GetStreamProxyStatus().FirstOrDefault(cs => cs.ID == new Guid(id));

//        if (status is null)
//            return NotFound();

//        return Ok(new ConnectionStatus
//        {
//            ID = status.ID,
//            Name = status.Name,
//            Status = status.ConnectionState
//        });
//    }

//    /// <summary>
//    /// Comment this
//    /// </summary>
//    /// <param name="id">ID of the connection to get the status for.</param>
//    /// <returns>Status of the connection.</returns>
//    [HttpPost, Route("ConnectionParameters")]
//    public IActionResult GetConnectionParameters(Connection connection)
//    {
//        return Ok(m_serviceCommands.GetDefaultConnectionParameters(connection));
//    }

//    /// <summary>
//    /// Bulk upload of connection records.
//    /// </summary>
//    /// <param name="records">Records to be uploaded.</param>
//    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
//    /// <returns>An <see cref="IActionResult"/> containing 1 or <see cref="Exception"/>.</returns>
//    [HttpPost, Route("bulkupload")]
//    public async Task<IActionResult> BulkUpload(List<Connection> records, CancellationToken cancellationToken)
//    {
//        await m_serviceCommands.UploadConnections(records, cancellationToken);
//        return Ok(1);
//    }

//    private bool ApplyFilter(RecordFilter<Connection> filter, Connection record)
//    {
//        Func<Connection, object?> keySelector = GetKeySelector(filter.FieldName);

//        if (filter.SearchParameter is null)
//            return false;

//        object? selection = keySelector(record);

//        if (selection is null)
//            return false;

//        bool RegexMatch(string pattern, string selection)
//        {
//            string rawPattern = pattern.Replace("*", ".*");
//            string escapedPattern = Regex.Replace(rawPattern, @"([()\\[\]])", @"\$1");
//            return Regex.IsMatch(selection, "^" + escapedPattern + "$", RegexOptions.IgnoreCase);
//        }

//        return filter.Operator switch
//        {
//            "=" => selection.ToString() == filter.SearchParameter.ToString(),
//            "<>" => selection.ToString() != filter.SearchParameter.ToString(),
//            ">" => (double)selection > (double)filter.SearchParameter,
//            "<" => (double)selection < (double)filter.SearchParameter,
//            ">=" => (double)selection >= (double)filter.SearchParameter,
//            "<=" => (double)selection <= (double)filter.SearchParameter,
//            "LIKE" => RegexMatch(filter.SearchParameter.ToString()!, selection.ToString()!),
//            "NOT LIKE" => !RegexMatch(filter.SearchParameter.ToString()!, selection.ToString()!),
//            "IN" => ((List<string>)filter.SearchParameter).Contains(selection.ToString()!),
//            "NOT IN" => !((List<string>)filter.SearchParameter).Contains(selection.ToString()!),
//            _ => false
//        };
//    }

//    private Task<IEnumerable<Connection>> GetConnections(CancellationToken cancellationToken)
//    {
//        return m_serviceCommands.DownloadActiveConfiguration(cancellationToken);
//    }

//    // Builds a KeySelector for a given string
//    private static Func<Connection, object?> GetKeySelector(string field)
//    {
//        PropertyInfo? property = typeof(Connection).GetProperty(field);

//        if (property is null)
//            throw new Exception($"Invalid field: \"{field}\"");
        
//        return c => property.GetValue(c) ?? c.ID;
//    }
//}

