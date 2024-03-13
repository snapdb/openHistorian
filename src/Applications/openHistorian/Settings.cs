using Gemstone.Configuration.AppSettings;
using Gemstone.StringExtensions;

namespace openHistorian;

/// <summary>
/// Defines settings for the openHistorian service.
/// </summary>
public class Settings : Gemstone.Timeseries.Settings
{
    /// <summary>
    /// Defines the configuration section name for web-related settings.
    /// </summary>
    public const string WebSettings = nameof(WebSettings);

    /// <summary>
    /// Default value for <see cref="HostURLs"/> property.
    /// </summary>
#if DEBUG || DEVELOPMENT
    public const string DefaultHostURLs = "http://localhost:8180";
#else
    public const string DefaultHostURLs = "http://*:8180";
#endif

    /// <summary>
    /// Default value for <see cref="HostCertificate"/> property.
    /// </summary>
    public const string DefaultHostCertificate = "";

    /// <summary>
    /// Gets or sets the URLs the hosted service will listen on.
    /// </summary>
    public string HostURLs { get; set; } = DefaultHostURLs;

    /// <summary>
    /// Gets or sets the certificate used to host the service.
    /// </summary>
    public string HostCertificate { get; set; } = DefaultHostCertificate;

    /// <inheritdoc/>
    public override void Initialize(IConfiguration configuration)
    {
        base.Initialize(configuration);

        IConfigurationSection webSettings = Configuration.GetSection(WebSettings);

        HostURLs = webSettings[nameof(HostURLs)].ToNonNullNorWhiteSpace(DefaultHostURLs);
        HostCertificate = webSettings[nameof(HostCertificate)].ToNonNullNorWhiteSpace(DefaultHostCertificate);
    }

    /// <inheritdoc/>
    public override void ConfigureAppSettings(IAppSettingsBuilder builder)
    {
        base.ConfigureAppSettings(builder);

        builder.Add($"{WebSettings}:{nameof(HostURLs)}", DefaultHostURLs, "Defines the URLs the hosted service will listen on.");
        builder.Add($"{WebSettings}:{nameof(HostCertificate)}", DefaultHostCertificate, "Defines the certificate used to host the service.");
        
        SwitchMappings[$"--{nameof(HostURLs)}"] = $"{WebSettings}:{nameof(HostURLs)}";
        SwitchMappings["-u"] = $"{WebSettings}:{nameof(HostURLs)}";

        SwitchMappings[$"--{nameof(HostCertificate)}"] = $"{WebSettings}:{nameof(HostCertificate)}";
        SwitchMappings["-s"] = $"{WebSettings}:{nameof(HostCertificate)}";
    }

    /// <summary>
    /// Gets the default instance of <see cref="Settings"/>.
    /// </summary>
    public new static Settings Instance => (Settings)Gemstone.Timeseries.Settings.Instance;
}
