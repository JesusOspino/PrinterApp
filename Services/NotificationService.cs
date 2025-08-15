
using CommunityToolkit.WinUI.Notifications;
using System.Runtime.InteropServices;

namespace NovaPrinter.Services;

public static class NotificationService
{
    private const string AppUserModelId = "com.lancelot.NovaPrinter";

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int SetCurrentProcessExplicitAppUserModelID(string appID);

    private static bool _initialized = false;

    /// <summary>
    /// Inicializa el servicio para las notificaciones
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;

        SetCurrentProcessExplicitAppUserModelID(AppUserModelId);

        _initialized = true;
    }

    /// <summary>
    /// Muestra un toast con título y mensaje
    /// </summary>
    public static void ShowToast(string title, string message)
    {
        Initialize();

        // Construir el contenido del toast
        var content = new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .GetToastContent();

        // Convertir a XML para la API de Windows
        /*var doc = new XmlDocument();
        doc.LoadXml(content.GetContent());

        // Crear y mostrar notificación
        var toast = new ToastNotification(doc);
        ToastNotificationManager.CreateToastNotifier(AppUserModelId).Show(toast);*/
    }
}
