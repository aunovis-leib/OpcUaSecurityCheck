using Opc.Ua;
using System.Text.Json;

class ServerConfig
{
    public int Port { get; set; } = 4840;
    public MessageSecurityMode SecurityMode { get; set; } = MessageSecurityMode.SignAndEncrypt;
    public string SecurityPolicyUri { get; set; } = SecurityPolicies.Basic256Sha256;
    public bool AutoAcceptUntrustedCertificates { get; set; } = false;

    public static ServerConfig LoadFromFile(string configPath)
    {
        var config = new ServerConfig();

        try
        {
            // Versuche mehrere Pfade, falls die Datei nicht im aktuellen Verzeichnis ist
            string actualPath = configPath;
            if (!File.Exists(actualPath))
            {
                string altPath = Path.Combine("Server", configPath);
                if (File.Exists(altPath))
                {
                    actualPath = altPath;
                }
                else
                {
                    Console.WriteLine($"Warnung: {configPath} nicht gefunden. Verwende Standard-Einstellungen.\n");
                    return config;
                }
            }

            var json = File.ReadAllText(actualPath);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("server", out var serverSection))
            {
                if (serverSection.TryGetProperty("port", out var portProp))
                {
                    config.Port = portProp.GetInt32();
                }

                if (serverSection.TryGetProperty("securityMode", out var modeProp))
                {
                    string mode = modeProp.GetString()?.ToLower() ?? "signandencrypt";
                    config.SecurityMode = mode switch
                    {
                        "none" => MessageSecurityMode.None,
                        "sign" => MessageSecurityMode.Sign,
                        "signandencrypt" => MessageSecurityMode.SignAndEncrypt,
                        _ => MessageSecurityMode.SignAndEncrypt,
                    };
                }

                if (serverSection.TryGetProperty("securityPolicy", out var policyProp))
                {
                    string policy = policyProp.GetString()?.ToLower() ?? "basic256sha256";
                    config.SecurityPolicyUri = policy switch
                    {
                        "none" => SecurityPolicies.None,
                        "basic128rsa15" => SecurityPolicies.Basic128Rsa15,
                        "basic256" => SecurityPolicies.Basic256,
                        "basic256sha256" => SecurityPolicies.Basic256Sha256,
                        // AES-based policies may not be available in all library versions; fall back to default.
                        "aes128sha256rsaoaep" => SecurityPolicies.Basic256Sha256,
                        "aes256sha256rsapss" => SecurityPolicies.Basic256Sha256,
                        _ => SecurityPolicies.Basic256Sha256,
                    };
                }

                if (serverSection.TryGetProperty("autoAcceptUntrustedCertificates", out var autoAcceptProp))
                {
                    config.AutoAcceptUntrustedCertificates = autoAcceptProp.GetBoolean();
                }
            }

            Console.WriteLine($"Konfiguration geladen aus: {actualPath}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Laden der Konfiguration: {ex.Message}\nVerwende Standard-Einstellungen.\n");
        }

        return config;
    }

    public static ServerConfig FromCommandLine(string[] args)
    {
        var config = new ServerConfig();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--port":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out int port))
                    {
                        config.Port = port;
                        i++;
                    }
                    break;

                case "--security-mode":
                    if (i + 1 < args.Length)
                    {
                        string mode = args[i + 1].ToLower();
                        config.SecurityMode = mode switch
                        {
                            "none" => MessageSecurityMode.None,
                            "sign" => MessageSecurityMode.Sign,
                            "signandencrypt" => MessageSecurityMode.SignAndEncrypt,
                            _ => MessageSecurityMode.SignAndEncrypt,
                        };
                        i++;
                    }
                    break;

                case "--security-policy":
                    if (i + 1 < args.Length)
                    {
                        string policy = args[i + 1].ToLower();
                        config.SecurityPolicyUri = policy switch
                        {
                            "none" => SecurityPolicies.None,
                            "basic128rsa15" => SecurityPolicies.Basic128Rsa15,
                            "basic256" => SecurityPolicies.Basic256,
                            "basic256sha256" => SecurityPolicies.Basic256Sha256,
                            // AES-based policies may not be available; fall back to Basic256Sha256.
                            "aes128sha256rsaoaep" => SecurityPolicies.Basic256Sha256,
                            "aes256sha256rsapss" => SecurityPolicies.Basic256Sha256,
                            _ => SecurityPolicies.Basic256Sha256,
                        };
                        i++;
                    }
                    break;

                case "--auto-accept":
                    config.AutoAcceptUntrustedCertificates = true;
                    break;

                case "--help":
                    PrintHelp();
                    Environment.Exit(0);
                    break;
            }
        }

        return config;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("=== OPC UA Test Server - Hilfe ===\n");
        Console.WriteLine("Verwendung: dotnet run --project ./OpcUaTestServer.csproj -- [Optionen]\n");
        Console.WriteLine("Optionen:");
        Console.WriteLine("  --port <Nummer>                Port (Standard: 4840)");
        Console.WriteLine("  --security-mode <Modus>        None | Sign | SignAndEncrypt (Standard: SignAndEncrypt)");
        Console.WriteLine("  --security-policy <Policy>     none | basic128rsa15 | basic256 | basic256sha256 |");
        Console.WriteLine("                                  aes128sha256rsaoaep | aes256sha256rsapss");
        Console.WriteLine("                                  (Standard: basic256sha256)");
        Console.WriteLine("  --auto-accept                   Auto-Accept untrusted certificates");
        Console.WriteLine("  --help                          Diese Hilfe anzeigen\n");
        Console.WriteLine("Beispiele:");
        Console.WriteLine("  dotnet run -- --port 4840 --security-mode SignAndEncrypt");
        Console.WriteLine("  dotnet run -- --security-mode None");
        Console.WriteLine("  dotnet run -- --auto-accept --security-policy basic256\n");
    }
}
