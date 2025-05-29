using Gemstone;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Data.Model;
using Microsoft.AspNetCore.Mvc;
using openHistorian.WebUI.Controllers.JsonModels;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        JsonElement defaultJsonElement = LoadDefaults();
        JsonElement userOverrides = LoadUserSpecific("user", "auth");

        JsonElement merged = MergeJson(defaultJsonElement, userOverrides);

        return Ok(merged);
    }

    /// <summary>
    /// Updates the user specific settings
    /// </summary>
    /// <returns></returns>
    [HttpPatch, Route("settings")]
    public IActionResult UpdateSettings([FromBody] JsonElement settings)
    {
        SaveUserSettings("user", "auth", settings);
        return Ok(settings);
    }

    private JsonElement LoadDefaults()
    {
        dynamic settings = Settings.Default["UserSettings"];

        if (string.IsNullOrEmpty(settings.ConfigFile))
            return new JsonElement();

        Environment.SpecialFolder specialFolder = Environment.SpecialFolder.CommonApplicationData;
        string appDataPath = Environment.GetFolderPath(specialFolder);
        string fullPath = Path.Combine(appDataPath, Common.ApplicationName, settings.ConfigFile);

        if (!System.IO.File.Exists(fullPath))
            return new JsonElement();

        if (Path.GetExtension(fullPath) != ".json")
            return new JsonElement();

        string json = System.IO.File.ReadAllText(fullPath);

        return JsonSerializer.Deserialize<JsonElement>(json);
    }

    private JsonElement LoadUserSpecific(string userID, string authProviderID)
    {
        using AdoDataConnection connection = new(Settings.Instance);
        UserSettings? settings = new TableOperations<UserSettings>(connection).QueryRecordWhere("UserID = {0} AND AuthProviderID = {1}", userID, authProviderID);
        return JsonSerializer.Deserialize<JsonElement>(settings?.Settings ?? "{}");
    }

    private void SaveUserSettings(string userID, string authProviderID, JsonElement settings)
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

    public static JsonElement MergeJson(JsonElement baseElement, JsonElement overrideElement)
    {
        JsonObject baseObj = JsonNode.Parse(baseElement.GetRawText())?.AsObject() ?? new JsonObject();
        JsonObject overrideObj = JsonNode.Parse(overrideElement.GetRawText())?.AsObject() ?? new JsonObject();

        foreach (KeyValuePair<string, JsonNode?> kvp in overrideObj)
            baseObj[kvp.Key] = kvp.Value?.DeepClone();

        return JsonDocument.Parse(baseObj.ToJsonString()).RootElement;
    }
}