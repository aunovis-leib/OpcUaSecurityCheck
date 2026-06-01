using System.Text.Json;
using Opc.Ua;

namespace OpcUaClient;

static class Program
{
    private static ApplicationConfiguration? _appConfig;
    private static ClientSettings? _settings;

    static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("=== OPC UA Server Security Check (Client) ===\n");

            // Lade Client-Konfiguration
            _settings = LoadClientSettings("appsettings.json");

            // OPC UA Server-Adresse (von args oder aus config)
            string serverUrl = args.Length > 0 ? args[0] : _settings.ServerUrl;

            Console.WriteLine($"Verbinde zu OPC UA Server: {serverUrl}\n");
            Console.WriteLine($"Client-Einstellungen:");
            Console.WriteLine($"  - AutoAcceptUntrustedCertificates: {_settings.AutoAcceptUntrustedCertificates}");
            Console.WriteLine($"  - Session Timeout: {_settings.SessionTimeout}ms\n");

            // Initialisiere die Anwendungskonfiguration
            await InitializeApplicationConfigAsync();

            Console.WriteLine("=== Konfiguration validiert ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nFehler: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Details: {ex.InnerException.Message}");
            }
        }
    }

    private static ClientSettings LoadClientSettings(string configPath)
    {
        try
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"Warnung: {configPath} nicht gefunden. Verwende Standard-Einstellungen.\n");
                return new ClientSettings();
            }

            var json = File.ReadAllText(configPath);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return new ClientSettings
            {
                ServerUrl = root.TryGetProperty("client", out var client) &&
                           client.TryGetProperty("serverUrl", out var url)
                    ? url.GetString() ?? "opc.tcp://localhost:4840"
                    : "opc.tcp://localhost:4840",

                AutoAcceptUntrustedCertificates = root.TryGetProperty("client", out var client2) &&
                                                 client2.TryGetProperty("autoAcceptUntrustedCertificates", out var autoAccept)
                    ? autoAccept.GetBoolean()
                    : false,

                SessionTimeout = root.TryGetProperty("client", out var client3) &&
                                client3.TryGetProperty("sessionTimeout", out var timeout)
                    ? timeout.GetInt32()
                    : 60000,
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Laden der Konfiguration: {ex.Message}");
            return new ClientSettings();
        }
    }

    private static async Task InitializeApplicationConfigAsync()
    {
        var commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var appCertPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "MachineDefault");
        var issuerPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "UA Certificate Authorities");
        var trustedPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "UA Applications");
        var rejectedPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "Rejected Certificates");

        Directory.CreateDirectory(appCertPath);
        Directory.CreateDirectory(issuerPath);
        Directory.CreateDirectory(trustedPath);
        Directory.CreateDirectory(rejectedPath);

        _appConfig = new ApplicationConfiguration
        {
            ApplicationName = "SecurityCheck",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = appCertPath,
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = issuerPath,
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = trustedPath,
                },
                RejectedCertificateStore = new CertificateStoreIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = rejectedPath,
                },
                AutoAcceptUntrustedCertificates = _settings?.AutoAcceptUntrustedCertificates ?? false,
            },
            ClientConfiguration = new ClientConfiguration
            {
                DefaultSessionTimeout = _settings?.SessionTimeout ?? 60000,
            },
        };

        await _appConfig.Validate(_appConfig.ApplicationType).ConfigureAwait(false);
    }
}

class ClientSettings
{
    public string ServerUrl { get; set; } = "opc.tcp://localhost:4840";
    public bool AutoAcceptUntrustedCertificates { get; set; }
    public int SessionTimeout { get; set; } = 60000;
}
