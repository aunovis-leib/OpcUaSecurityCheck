using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SecurityCheckGui.ViewModels
{
    public partial class SecurityCheckViewModel : ObservableObject
    {
        private OpcUaClientService? _opcUaClient;

        [ObservableProperty]
        private string serverUrl = "opc.tcp://localhost:4840";

        [ObservableProperty]
        private bool autoAcceptUntrustedCertificates;

        [ObservableProperty]
        private int sessionTimeout = 60000;

        [ObservableProperty]
        private string statusMessage = "Bereit für Verbindung...";

        [ObservableProperty]
        private string connectionStatus = "Keine Verbindung";

        [ObservableProperty]
        private string securityMode = "Nicht geprüft";

        [ObservableProperty]
        private string securityPolicy = "Nicht geprüft";

        [ObservableProperty]
        private string autoAcceptStatus = "Nicht geprüft";

        [ObservableProperty]
        private string certificateSubject = "Nicht geprüft";

        [ObservableProperty]
        private string certificateIssuer = "";

        [ObservableProperty]
        private string certificateDate = "";

        [ObservableProperty]
        private string certificateStatus = "";

        [ObservableProperty]
        private string sessionId = "Nicht geprüft";

        [ObservableProperty]
        private string sessionTimeoutDisplay = "";

        [ObservableProperty]
        private string sessionConnected = "";

        [ObservableProperty]
        private ObservableCollection<string> errorLogs = new();

        [ObservableProperty]
        private bool isConnecting;

        public SecurityCheckViewModel()
        {
            // XAML-Design-Zeit Support
        }

        [RelayCommand]
        public async Task Connect()
        {
            try
            {
                IsConnecting = true;
                ErrorLogs.Clear();
                ClearResults();

                StatusMessage = "Verbindung wird hergestellt...";

                var settings = new ClientSettings
                {
                    ServerUrl = ServerUrl,
                    AutoAcceptUntrustedCertificates = AutoAcceptUntrustedCertificates,
                    SessionTimeout = SessionTimeout
                };

                _opcUaClient = new OpcUaClientService(settings);

                try
                {
                    var result = await _opcUaClient.ConnectAndCheckSecurityAsync(ServerUrl);

                    StatusMessage = "Verbindung erfolgreich!";

                    DisplaySecuritySettings(result);
                    AddErrorLog("✓ Verbindungsprüfung abgeschlossen");
                }
                finally
                {
                    _opcUaClient?.Disconnect();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "❌ Verbindungsfehler!";
                ConnectionStatus = "❌ Keine Verbindung";
                SecurityMode = "❌ Fehler";
                SecurityPolicy = "❌ Fehler";
                AutoAcceptStatus = "❌ Fehler";
                CertificateSubject = "❌ Fehler";
                CertificateIssuer = "❌ Fehler";
                CertificateDate = "❌ Fehler";
                CertificateStatus = "❌ Fehler";
                SessionId = "❌ Fehler";
                SessionTimeoutDisplay = "❌ Fehler";
                SessionConnected = "❌ Fehler";

                AddErrorLog($"Fehler: {ex.Message}");
                if (ex.InnerException != null)
                {
                    AddErrorLog($"Details: {ex.InnerException.Message}");
                }
            }
            finally
            {
                IsConnecting = false;
            }
        }

        private void DisplaySecuritySettings(SecurityCheckResult result)
        {
            try
            {
                ConnectionStatus = $"Endpunkt: {result.EndpointUrl}";

                // Security Mode
                string securityModeStatus = result.SecurityMode switch
                {
                    "None" => "Modus: None ⚠️ WARNUNG: Keine Verschlüsselung!",
                    "Sign" => "Modus: Sign ✓ Nachrichten sind signiert",
                    "SignAndEncrypt" => "Modus: SignAndEncrypt ✓ Nachrichten sind signiert und verschlüsselt",
                    _ => $"Modus: {result.SecurityMode}"
                };

                SecurityMode = securityModeStatus;

                SecurityPolicy = result.SecurityPolicy;

                // Auto Accept Untrusted Certificates
                AutoAcceptStatus = $"Aktiviert: {result.AutoAcceptUntrustedCertificates}";
                if (result.AutoAcceptUntrustedCertificates)
                {
                    AutoAcceptStatus += "\n⚠️ WARNUNG: Nicht vertrauenswürdige Zertifikate werden automatisch akzeptiert!";
                }
                else
                {
                    AutoAcceptStatus += "\n✓ Nur vertrauenswürdige Zertifikate werden akzeptiert";
                }

                // Certificate Information
                CertificateSubject = $"Subject: {result.CertificateSubject}";
                CertificateIssuer = $"Issuer: {result.CertificateIssuer}";
                CertificateDate = $"Gültig von {result.CertificateValidFrom} bis {result.CertificateValidUntil}";

                CertificateStatus = result.CertificateValid
                    ? "✓ Zertifikat ist gültig"
                    : "⚠️ Zertifikat ist ungültig oder abgelaufen";

                // Session Information
                SessionId = $"Session ID: {result.SessionId}";
                SessionTimeoutDisplay = $"Session Timeout: {result.SessionTimeout} ms";
                SessionConnected = $"Verbunden: {result.Connected}";
            }
            catch (Exception ex)
            {
                AddErrorLog($"Fehler bei der Anzeige der Sicherheitseinstellungen: {ex.Message}");
            }
        }

        private void ClearResults()
        {
            ConnectionStatus = "⏳ Wird geprüft...";
            SecurityMode = "⏳ Wird geprüft...";
            SecurityPolicy = "⏳ Wird geprüft...";
            AutoAcceptStatus = "⏳ Wird geprüft...";
            CertificateSubject = "⏳ Wird geprüft...";
            CertificateIssuer = "⏳ Wird geprüft...";
            CertificateDate = "⏳ Wird geprüft...";
            CertificateStatus = "⏳ Wird geprüft...";
            SessionId = "⏳ Wird geprüft...";
            SessionTimeoutDisplay = "⏳ Wird geprüft...";
            SessionConnected = "⏳ Wird geprüft...";
        }

        private void AddErrorLog(string message)
        {
            ErrorLogs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}
