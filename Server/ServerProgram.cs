using System;
using System.Threading.Tasks;

class ServerProgram
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("=== OPC UA Test Server ===\n");

            // Lade Konfiguration von Datei
            ServerConfig config = ServerConfig.LoadFromFile("config.json");

            // Überschreibe mit Kommandozeilen-Argumenten (falls vorhanden)
            if (args.Length > 0)
            {
                config = ServerConfig.FromCommandLine(args);
            }

            Console.WriteLine($"Server-Konfiguration:");
            Console.WriteLine($"  Port: {config.Port}");
            Console.WriteLine($"  SecurityMode: {config.SecurityMode}");
            Console.WriteLine($"  SecurityPolicy: {config.SecurityPolicyUri}");
            Console.WriteLine($"  AutoAcceptUntrustedCertificates: {config.AutoAcceptUntrustedCertificates}\n");

            // Erstelle und starte den Server
            var server = new OpcUaTestServer(config);
            await server.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Kritischer Fehler: {ex.Message}");
            Environment.Exit(1);
        }
    }
}

