using System.Security.Cryptography.X509Certificates;
using Gemstone;
using Gemstone.Configuration;
using Gemstone.EventHandlerExtensions;

namespace openHistorian.WebUI;

/// <summary>
/// Represents settings for hosting a web service.
/// </summary>
public class WebHosting : ISupportLifecycle, IPersistSettings
{
    /// <summary>
    /// Specifies the default value for the <see cref="SettingsCategory"/> property.
    /// </summary>
    public const string DefaultSettingsCategory = nameof(WebHosting);

    /// <summary>
    /// Specifies the default value for the <see cref="PersistSettings"/> property.
    /// </summary>
    public const bool DefaultPersistSettings = true;

    /// <summary>
    /// Default value for <see cref="HostURLs"/> property.
    /// </summary>
#if DEBUG
    public const string DefaultHostURLs = "http://localhost:8180";
#else
    public const string DefaultHostURLs = "http://*:8180";
#endif

    /// <summary>
    /// Default value for <see cref="HostCertificate"/> property.
    /// </summary>
    public const string DefaultHostCertificate = "";

    /// <inheritdoc />
    public event EventHandler? Disposed;

    /// <summary>
    /// Gets or sets the URLs the hosted service will listen on.
    /// </summary>
    public string HostURLs { get; set; } = DefaultHostURLs;

    /// <summary>
    /// Gets or sets the certificate used to host the service.
    /// </summary>
    public string HostCertificate { get; set; } = DefaultHostCertificate;

    /// <inheritdoc />
    public bool PersistSettings { get; init; } = DefaultPersistSettings;

    /// <inheritdoc />
    public string SettingsCategory { get; init; } = DefaultSettingsCategory;

    /// <inheritdoc />
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public bool IsDisposed { get; private set; }

    /// <inheritdoc />
    public void Initialize()
    {
        LoadSettings();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (IsDisposed)
            return;

        try
        {
            SaveSettings();
        }
        finally
        {
            IsDisposed = true;
            Disposed?.SafeInvoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    public void SaveSettings()
    {
        if (!PersistSettings)
            return;

        // Ensure that settings category is specified.
        if (string.IsNullOrEmpty(SettingsCategory))
            throw new InvalidOperationException("SettingsCategory property has not been set");

        // Save settings under the specified category.
        dynamic settings = Settings.Instance[SettingsCategory];

        settings.HostURLs = HostURLs;
        settings.HostCertificate = HostCertificate;
    }

    /// <inheritdoc />
    public void LoadSettings()
    {
        if (!PersistSettings)
            return;

        // Ensure that settings category is specified.
        if (string.IsNullOrEmpty(SettingsCategory))
            throw new InvalidOperationException("SettingsCategory property has not been set");

        // Load settings from the specified category.
        dynamic settings = Settings.Instance[SettingsCategory];

        HostURLs = settings.HostURLs ?? DefaultHostURLs;
        HostCertificate = settings.HostCertificate ?? DefaultHostCertificate;
    }

    /// <summary>
    /// Builds a new <see cref="WebServer"/> instance.
    /// </summary>
    /// <returns>New <see cref="WebServer"/> instance.</returns>
    public WebServer BuildServer()
    {
        WebServerConfiguration configuration = new()
        {
            Hosting = this,
            CertificateSelector = CreateCertificateSelector(HostCertificate)
        };

        return new WebServer(configuration);
    }

    private static Func<X509Certificate2?> CreateCertificateSelector(string? setting)
    {
        if (setting is null)
            return () => null;

        return () =>
        {
            using X509Store store = new(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            return store.Certificates.FirstOrDefault(cert => cert.Thumbprint.Equals(setting, StringComparison.OrdinalIgnoreCase));
        };
    }

    /// <inheritdoc cref="IDefineSettings.DefineSettings" />
    public static void DefineSettings(Settings settings, string settingsCategory = DefaultSettingsCategory)
    {
        dynamic section = settings[settingsCategory];

        section.HostURLs = (DefaultHostURLs, "URLs the hosted service will listen on.", "-u", "--HostURLs");
        section.HostCertificate = (DefaultHostCertificate, "Certificate used to host the service.", "-s", "--HostCertificate");
    }
}
