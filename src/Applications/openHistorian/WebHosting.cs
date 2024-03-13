using System.Security.Cryptography.X509Certificates;
using openHistorian.WebUI;

namespace openHistorian;

internal class WebHosting
{
    public static WebServer BuildServer()
    {
        WebServerConfiguration configuration = new()
        {
            CertificateSelector = CreateCertificateSelector(Settings.Instance.HostCertificate)
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

}
