using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using openHistorian.WebUI.Controllers.JsonModels;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ThemeController : ModelController<Theme>
{
    public static string DefaultTheme
    {
        get
        {
            try
            {
                dynamic section = Gemstone.Configuration.Settings.Default[Settings.SystemSettingsCategory];
                return section.DefaultTheme;
            }
            catch (Exception ex)
            {
                return "./Styles/bootstrap.min.css";
            }
        }
    }

    public static string WebRoot
    {
        get
        {
            try
            {
                dynamic section = Gemstone.Configuration.Settings.Default[WebHosting.DefaultSettingsCategory];
                return section.WebRoot;
            }
            catch (Exception ex)
            {
                return "wwwroot";
            }
        }
    }

    public static string ThemesFolder
    {
        get
        {
            return "Themes";
        }
    }

    [HttpPost, Route("Upload/{encodedTheme}")]
    public async Task<IActionResult> Post([FromForm] IFormFile uploadedFile, string encodedTheme, CancellationToken cancellationToken)
    {
        if (!PostAuthCheck())
            return Unauthorized();

        // Decode and deserialize the Theme model from the route parameter.
        string json = Uri.UnescapeDataString(encodedTheme);
        Theme? themeRecord = JsonConvert.DeserializeObject<Theme>(json);

        if (themeRecord is null)
            return BadRequest("Theme record is null");

        // Generate a unique temp filename disregarding the user input.
        string safeFileName = Guid.NewGuid().ToString() + ".css";
        themeRecord.FileName = safeFileName;

        IActionResult fileUploadResult = UploadCSSFile(uploadedFile, safeFileName);
        if (!(fileUploadResult is OkResult))
            return fileUploadResult;

        await using AdoDataConnection connection = CreateConnection();
        TableOperations<Theme> tableOperations = new(connection);
        await tableOperations.AddNewRecordAsync(themeRecord, cancellationToken);

        return Ok(themeRecord);
    }

    public IActionResult UploadCSSFile(IFormFile uploadedFile, string safeFileName)
    {
        if (uploadedFile == null || uploadedFile.Length == 0)
            return BadRequest("No file provided.");

        // Validate the file extension
        if (Path.GetExtension(uploadedFile.FileName) != ".css")
            return BadRequest("Invalid file type. Only CSS files are allowed.");

        string filePath = Path.Combine(WebRoot, ThemesFolder, safeFileName);

        // Save the file
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            uploadedFile.CopyTo(stream);
        }

        return Ok();
    }

    [HttpGet, Route("{themeID}.css")]
    public IActionResult GetCSSFile(int themeID)
    {
        Theme? theme;

        using (AdoDataConnection connection = new AdoDataConnection(Settings.Default))
            theme = new TableOperations<Theme>(connection).QueryRecordWhere("ID = {0}", themeID);

        string filename = theme?.FileName ?? DefaultTheme;

        if (Path.GetExtension(filename) != ".css")
            return Unauthorized();

        string filePath = Path.Combine(WebRoot, ThemesFolder, Path.GetFileName(filename));

        Stream? stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        if (stream is null)
            return NotFound();

        return File(stream, "text/css");
    }

    [HttpDelete, Route("Delete")]
    public new async Task<IActionResult> Delete([FromBody] Theme record, CancellationToken cancellationToken)
    {
        if (!DeleteAuthCheck())
            return Unauthorized();

        await using AdoDataConnection connection = CreateConnection();
        TableOperations<Theme> tableOperations = new(connection);
        await tableOperations.DeleteRecordAsync(record, cancellationToken);

        // strip out any path information, leaving just the file name.
        string fileNameOnly = Path.GetFileName(record.FileName);

        string safeFileName = Path.GetFileNameWithoutExtension(fileNameOnly) + ".css";
        string filePath = Path.Combine(WebRoot, ThemesFolder, safeFileName);

        string fullPath = Path.GetFullPath(filePath);
        string expectedBase = Path.GetFullPath(Path.Combine(WebRoot, ThemesFolder));

        if (!fullPath.StartsWith(expectedBase, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Invalid file path detected.");
        }

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        return Ok(1);
    }
}