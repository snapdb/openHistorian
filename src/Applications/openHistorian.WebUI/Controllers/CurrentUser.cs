using Gemstone;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Data.Model;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using openHistorian.WebUI.Controllers.JsonModels;
using System.Security.AccessControl;
using System.Text.Json;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
public class CurrentUserController : Controller
{

    /// <summary>
    /// Returns the user specific settings
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("settings")]
    public IActionResult GetSettings()
    {
        JObject defaults = LoadDefaults();
        defaults.Merge(LoadUserSpecific("user", "auth"));
        return Ok(JsonSerializer.Serialize(defaults));
    }

    /// <summary>
    /// Updates the user specific settings
    /// </summary>
    /// <returns></returns>
    [HttpPatch, Route("settings")]
    public IActionResult UpdateSettings(JObject settings)
    {
        SaveUserSettings("user", "auth", settings);
        return Ok(1);
    }

    private JObject LoadDefaults()
    {
        dynamic settings = Gemstone.Configuration.Settings.Default["UserSettings"];

        if (string.IsNullOrEmpty(settings.ConfigFile))
            return new JObject();


        Environment.SpecialFolder specialFolder = Environment.SpecialFolder.CommonApplicationData;
        string appDataPath = Environment.GetFolderPath(specialFolder);
        string fullPath = Path.Combine(appDataPath, Common.ApplicationName, settings.ConfigFile);

        if (!System.IO.File.Exists(fullPath))
            return new JObject();

        if (Path.GetExtension(fullPath) != ".json")
            return new JObject();

        string json = System.IO.File.ReadAllText(fullPath);

        return JObject.Parse(json);
    }

    private JObject LoadUserSpecific(string userID, string authProviderID)
    {
        using AdoDataConnection connection = new(Settings.Instance);
        UserSettings? settings = new TableOperations<UserSettings>(connection).QueryRecordWhere("UserID = {0} AND AuthProviderID = {1}", userID, authProviderID);
        return JObject.Parse(settings?.Settings ?? "{}");
    }

    private void SaveUserSettings(string userID, string authProviderID, JObject settings)
    {
        using AdoDataConnection connection = new(Settings.Instance);
        UserSettings? userSettings = new TableOperations<UserSettings>(connection).QueryRecordWhere("UserID = {0} AND AuthProviderID = {1}", userID, authProviderID);
        if (userSettings == null)
        {
            userSettings = new UserSettings
            {
                UserID = userID,
                AuthProviderID = authProviderID,
                Settings = settings.ToString()
            };
            new TableOperations<UserSettings>(connection).AddNewRecord(userSettings);
        }
        else
        {
            userSettings.Settings = settings.ToString();
            new TableOperations<UserSettings>(connection).UpdateRecord(userSettings);
        }
    }
}
