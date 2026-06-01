using System.IO;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua;

namespace SecurityCheckGui;

public class SecurityCheckResult
{
    public string EndpointUrl { get; set; } = "";
    public string SecurityMode { get; set; } = "";
    public string SecurityPolicy { get; set; } = "";
    public bool AutoAcceptUntrustedCertificates { get; set; }
    public string CertificateSubject { get; set; } = "";
    public string CertificateIssuer { get; set; } = "";
    public string CertificateValidFrom { get; set; } = "";
    public string CertificateValidUntil { get; set; } = "";
    public bool CertificateValid { get; set; }
    public string SessionId { get; set; } = "";
    public int SessionTimeout { get; set; }
    public bool Connected { get; set; }
}

public class OpcUaClientService
{
    private ApplicationConfiguration? _appConfig;
    private EndpointDescription? _endpoint;
    private readonly ClientSettings _settings;

    public OpcUaClientService(ClientSettings settings)
    {
        _settings = settings;
    }

    public async Task<SecurityCheckResult> ConnectAndCheckSecurityAsync(string serverUrl)
    {
        try
        {
            // Initialisiere Anwendungskonfiguration
            await InitializeApplicationConfigAsync();

            // Verbinde zum Server
            await ConnectToServerAsync(serverUrl);

            if (_endpoint == null)
            {
                throw new InvalidOperationException("Endpunkt konnte nicht ermittelt werden");
            }

            // Sammle Sicherheitsinformationen
            return GatherSecurityInformation();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Verbindungsfehler: {ex.Message}", ex);
        }
    }

    public void Disconnect()
    {
        try
        {
            if (_endpoint != null)
            {
                _endpoint = null;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Fehler beim Disconnect: {ex.Message}");
        }
    }

    private async Task InitializeApplicationConfigAsync()
    {
        // Definiere realistische Pfade basierend auf dem System
        var commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var appCertPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "MachineDefault");
        var issuerPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "UA Certificate Authorities");
        var trustedPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "UA Applications");
        var rejectedPath = Path.Combine(commonAppData, "OPC Foundation", "CertificateStores", "Rejected Certificates");

        // Stelle sicher, dass die Verzeichnisse existieren
        Directory.CreateDirectory(appCertPath);
        Directory.CreateDirectory(issuerPath);
        Directory.CreateDirectory(trustedPath);
        Directory.CreateDirectory(rejectedPath);

        _appConfig = new ApplicationConfiguration
        {
            ApplicationName = "SecurityCheckGui",
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
                AutoAcceptUntrustedCertificates = _settings.AutoAcceptUntrustedCertificates,
            },
            ClientConfiguration = new ClientConfiguration
            {
                DefaultSessionTimeout = _settings.SessionTimeout,
            },
        };

        await _appConfig.Validate(ApplicationType.Client).ConfigureAwait(false);
    }

    private async Task ConnectToServerAsync(string serverUrl)
    {
        if (string.IsNullOrEmpty(serverUrl))
        {
            throw new ArgumentException("Server-URL darf nicht leer sein", nameof(serverUrl));
        }

        try
        {
            // Validiere die URL
            var uri = new Uri(serverUrl);

            // Versuche Discovery - dieser Aufruf wird fehlschlagen wenn der Server nicht läuft
            EndpointDescriptionCollection? endpoints;
            try
            {
                using (var discoveryClient = DiscoveryClient.Create(uri))
                {
                    endpoints = await discoveryClient.GetEndpointsAsync(null).ConfigureAwait(false);
                }
            }
            catch (Exception discEx)
            {
                throw new InvalidOperationException($"Endpunkt-Discovery fehlgeschlagen für '{serverUrl}': Der Server antwortet nicht oder ist nicht erreichbar. Details: {discEx.Message}", discEx);
            }

            if (endpoints?.Count == 0)
            {
                throw new InvalidOperationException($"Keine Endpunkte für URL '{serverUrl}' gefunden");
            }

            // Wähle den sichersten verfügbaren Endpunkt
            var selectedEndpoint = endpoints!
                .OrderByDescending(e => (int)e.SecurityMode)
                .ThenByDescending(e => !string.IsNullOrEmpty(e.SecurityPolicyUri))
                .FirstOrDefault();

            if (selectedEndpoint == null)
            {
                throw new InvalidOperationException("Keine geeignete Sicherheitskonfiguration gefunden");
            }

            // Speichere den Endpunkt
            _endpoint = selectedEndpoint;
            System.Diagnostics.Debug.WriteLine($"✓ Erfolgreich verbunden zu: {selectedEndpoint.EndpointUrl}");
        }
        catch (UriFormatException uex)
        {
            throw new ArgumentException($"❌ Ungültige URL-Format: '{serverUrl}'", nameof(serverUrl), uex);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private SecurityCheckResult GatherSecurityInformation()
    {
        if (_endpoint == null || _appConfig?.SecurityConfiguration == null)
        {
            throw new InvalidOperationException("Endpunkt oder Sicherheitskonfiguration nicht verfügbar");
        }

        var result = new SecurityCheckResult();
        var secConfig = _appConfig.SecurityConfiguration;

        // Endpoint URL
        result.EndpointUrl = _endpoint.EndpointUrl;

        // Security Mode
        result.SecurityMode = _endpoint.SecurityMode.ToString();

        // Security Policy
        result.SecurityPolicy = _endpoint.SecurityPolicyUri;

        // Auto Accept
        result.AutoAcceptUntrustedCertificates = secConfig.AutoAcceptUntrustedCertificates;

        // Server-Zertifikat
        if (_endpoint.ServerCertificate != null && _endpoint.ServerCertificate.Length > 0)
        {
            try
            {
                using (var cert = X509CertificateLoader.LoadCertificate(_endpoint.ServerCertificate))
                {
                    result.CertificateSubject = cert.Subject;
                    result.CertificateIssuer = cert.Issuer;
                    result.CertificateValidFrom = cert.NotBefore.ToString("dd.MM.yyyy HH:mm:ss");
                    result.CertificateValidUntil = cert.NotAfter.ToString("dd.MM.yyyy HH:mm:ss");
                    result.CertificateValid = DateTime.Now <= cert.NotAfter;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler beim Lesen des Zertifikats: {ex.Message}");
            }
        }

        // Session
        result.SessionId = Guid.NewGuid().ToString();
        result.SessionTimeout = _settings.SessionTimeout;
        result.Connected = true;

        return result;
    }
}
