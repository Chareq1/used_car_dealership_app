using Avalonia.Notification;
using CommunityToolkit.Mvvm.ComponentModel;

namespace used_car_dealership_app.ViewModels;

//KLASA BAZOWA DLA WIDOKÓW
public class ViewModelBase : ObservableObject
{
    public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
}