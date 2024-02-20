using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

public static class AlertExtensions
{
    public static MessageBoxResult ShowDeleteMarkerAlert(string markerId, Action deleteAction, Action cancelAction = null)
    {
        string message;
        var routes = SpatialDataCache.RoutesContaining(markerId);
        if (!routes.Any())
        {
            message = GDLocalizedTextView("general.alert.destructive_undone_message");
        }
        else
        {
            var routeNames = string.Join("\n", routes.Select(r => r.Name));
            message = GDLocalizedTextView("markers.destructive_delete_message.routes", routeNames);
        }

        var result = MessageBox.Show(message, GDLocalizedTextView("markers.destructive_delete_message"), MessageBoxButton.OKCancel);
        if (result == MessageBoxResult.OK)
        {
            deleteAction?.Invoke();
        }
        else
        {
            cancelAction?.Invoke();
        }

        return result;
    }
}
