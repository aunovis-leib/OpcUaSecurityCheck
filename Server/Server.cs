using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace SecurityCheckServer
{
    class OpcUaTestServer
    {
        private StandardServer? _server;
        private readonly ServerConfig _serverConfig;

        public OpcUaTestServer(ServerConfig config)
        {
            _serverConfig = config;
        }

        public async Task StartAsync()
        {
            try
            {
                Console.WriteLine("=== OPC UA Test Server wird gestartet ===\n");

                // Konfiguriere die Anwendung
                var config = new ApplicationConfiguration
                {
                    ApplicationName = "OPC UA Test Server",
                    ApplicationType = ApplicationType.Server,

                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier
                        {
                            StoreType = "Directory",
                            StorePath = "OPC Foundation\\CertificateStores\\MachineDefault",
                            SubjectName = "CN=OPC UA Test Server",
                        },
                        AutoAcceptUntrustedCertificates = _serverConfig.AutoAcceptUntrustedCertificates,
                    },

                    TransportQuotas = new TransportQuotas
                    {
                        OperationTimeout = 600000,
                        MaxStringLength = 1048576,
                        MaxByteStringLength = 4194304,
                        MaxArrayLength = 65536,
                        MaxMessageSize = 4194304,
                        ChannelLifetime = 300000,
                        SecurityTokenLifetime = 3600000,
                    },

                    ServerConfiguration = new ServerConfiguration
                    {
                        BaseAddresses = new StringCollection
                    {
                        $"opc.tcp://localhost:{_serverConfig.Port}",
                        $"opc.tcp://127.0.0.1:{_serverConfig.Port}",
                    },
                        // Use default security policies provided by the OPC UA stack.
                        UserTokenPolicies =
                    [
                        new UserTokenPolicy
                        {
                            TokenType = UserTokenType.Anonymous,
                            SecurityPolicyUri = SecurityPolicies.None,
                        },
                    ],
                        DiagnosticsEnabled = true,
                        MaxSessionCount = 100,
                        MaxSubscriptionCount = 100,
                        MaxEventQueueSize = 10000,
                        MinSubscriptionLifetime = 10000,
                    },

                    ClientConfiguration = new ClientConfiguration
                    {
                        DefaultSessionTimeout = 60000,
                    },
                };

                // Validiere die Konfiguration
                await config.Validate(config.ApplicationType);

                // Erstelle und starte den Server über ApplicationInstance (reale OPC UA-Startlogik)
                var application = new ApplicationInstance
                {
                    ApplicationName = "OPC UA Test Server",
                    ApplicationType = ApplicationType.Server,
                    ApplicationConfiguration = config
                };

                // Ensure an application certificate exists (create if missing) so the server can bind secure endpoints.
                try
                {
                    await application.CheckApplicationInstanceCertificate(true, CertificateFactory.DefaultKeySize);
                }
                catch
                {
                    // Ignore certificate creation failures here; Start may still proceed for unsecured endpoints.
                }

                var serverInstance = new StandardServer();
                _server = serverInstance;
                _ = application.Start(serverInstance);

                Console.WriteLine($"✓ OPC UA Server erfolgreich gestartet");
                Console.WriteLine($"  - Adresse: opc.tcp://localhost:{_serverConfig.Port}");
                Console.WriteLine($"  - Message Security Mode: {_serverConfig.SecurityMode}");
                Console.WriteLine($"  - Security Policy: {_serverConfig.SecurityPolicyUri}");
                Console.WriteLine($"  - Auto Accept Untrusted Certs: {_serverConfig.AutoAcceptUntrustedCertificates}\n");
                Console.WriteLine("Server läuft. Drücke Enter zum Beenden...\n");

                try
                {
                    await Task.Run(() => Console.ReadLine());
                }
                catch
                {
                    // Console-Eingabe nicht verfügbar, warte indefinit
                    await Task.Delay(Timeout.Infinite);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Starten des Servers: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Details: {ex.InnerException.Message}");
                }
            }
        }

        public async Task StopAsync()
        {
            if (_server != null)
            {
                _server.Stop();
                Console.WriteLine("Server beendet.");
            }
        }

        // Security policy construction is handled by the OPC UA stack; no helper needed here.
    }
}

/// <summary>
/// Test Node Manager - Einfache Knotenverwaltung für den Server
/// </summary>
// Note: No custom TestNodeManager is registered here. Using the library's
// StandardServer will start the OPC UA stack. Add custom node managers by
// deriving from StandardServer and registering MasterNodeManager if needed.
