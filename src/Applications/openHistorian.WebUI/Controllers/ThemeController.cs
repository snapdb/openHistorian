using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Timeseries.Adapters;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Data.Types;
using openHistorian.Model;
using openHistorian.WebUI.Controllers.JsonModels;
using openHistorian.WebUI.Properties;
using System.Net;
using System.Reflection;
using static System.Collections.Specialized.BitVector32;

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

    [HttpGet, Route("{themeID}.css")]
    public IActionResult GetCSSFile(int themeID)
    {
        Theme? theme;

        using (AdoDataConnection connection = new AdoDataConnection(Settings.Default))
            theme = new TableOperations<Theme>(connection).QueryRecordWhere("ID = {0}", themeID);

        string path = "";

        string filename = theme?.FileName ?? DefaultTheme;

        if (Path.GetExtension(filename) != ".css")
            return Unauthorized();

       string filePath = Path.Combine(WebRoot, "css", Path.GetFileName(filename));


        Stream? stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        
        if (stream is null)
            return NotFound();

        return File(stream, "text/css");
    }
}
