using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using SecurityCheckGui.ViewModels;

namespace SecurityCheckGui;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var viewModel = new SecurityCheckViewModel();
        LoadSettings(viewModel);
        DataContext = viewModel;
    }

    private void LoadSettings(SecurityCheckViewModel viewModel)
    {
        try
        {
            var settings = LoadClientSettings("appsettings.json");
            if (settings != null)
            {
                viewModel.ServerUrl = settings.ServerUrl;
                viewModel.AutoAcceptUntrustedCertificates = settings.AutoAcceptUntrustedCertificates;
                viewModel.SessionTimeout = settings.SessionTimeout;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Fehler beim Laden der Einstellungen: {ex.Message}");
        }
    }

    private static ClientSettings LoadClientSettings(string configPath)
    {
        try
        {
            if (!File.Exists(configPath))
            {
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
            throw new Exception($"Fehler beim Laden der Konfiguration: {ex.Message}", ex);
        }
    }
}

public class ClientSettings
{
    public string ServerUrl { get; set; } = "opc.tcp://localhost:4840";
    public bool AutoAcceptUntrustedCertificates { get; set; } = false;
    public int SessionTimeout { get; set; } = 60000;
}

