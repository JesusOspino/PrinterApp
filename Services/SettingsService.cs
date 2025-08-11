using NovaPrinter.Models;
using System.IO;
using System.Text.Json;

namespace NovaPrinter.Services;

public static class SettingsService
{
    private static readonly string AppFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NovaPrinter");

    private static readonly string ConfigPath = Path.Combine(AppFolder, "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(ConfigPath)) return new AppSettings();

            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch (Exception)
        {
            return new AppSettings();
        }
    }

    public static void Save(AppSettings settings)
    {
        Directory.CreateDirectory(AppFolder);
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}
