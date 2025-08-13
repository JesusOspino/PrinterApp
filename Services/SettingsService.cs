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

    // Estado global(singleton estático)
    private static AppSettings _currentSettings = new AppSettings();

    // Inicialización estática
    static SettingsService()
    {
        // Asegurar que el directorio existe
        Directory.CreateDirectory(AppFolder);
    }

    // Propiedad pública para acceder a los settings
    public static AppSettings Current
    {
        get
        {
            // Carga lazy (sólo cuando se accede por primera vez)
            if (_currentSettings == null)
            {
                Load();
            }
            return _currentSettings!;
        }
    }

    public static void Load()
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                _currentSettings = new AppSettings();
                return;
            }

            var json = File.ReadAllText(ConfigPath);
            _currentSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch (Exception ex)
        {
            // Log del error (considera usar un logger real)
            Console.WriteLine($"Error loading settings: {ex.Message}");
            _currentSettings = new AppSettings();
        }
    }

    public static void Save()
    {
        Save(Current);
    }

    public static void Save(AppSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);

            // Actualizar la referencia actual si se guardan settings diferentes
            if (!ReferenceEquals(_currentSettings, settings))
            {
                _currentSettings = settings;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
            throw; // O manejar de otra forma según tu necesidad
        }
    }

    // Métodos adicionales para manipulación específica
    public static void UpdateSetting<T>(string key, T value)
    {
        // Implementación específica según tu modelo AppSettings
        // Ejemplo genérico:
        var property = typeof(AppSettings).GetProperty(key);
        if (property != null && property.CanWrite)
        {
            property.SetValue(Current, value);
        }
        Save();
    }
}
