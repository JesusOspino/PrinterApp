
using Microsoft.Toolkit.Uwp.Notifications;

namespace NovaPrinter.Services;

public static class NotificationService
{
    
    /// <summary>
    /// Muestra un toast con título y mensaje
    /// </summary>
    public static void ShowToast(string title, string message)
    {
        new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .Show();
    }
}
