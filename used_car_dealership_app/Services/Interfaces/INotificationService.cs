using System;

namespace used_car_dealership_app.Services.Interfaces;

//INTERFEJS DLA USŁUGI POWIADOMIEŃ
public interface INotificationService
{
    public void ShowError(String title, String message);
    public void ShowInfo(String title, String message);
    public void ShowWarning(String title, String message);
    public void ShowSuccess(String title, String message);
}