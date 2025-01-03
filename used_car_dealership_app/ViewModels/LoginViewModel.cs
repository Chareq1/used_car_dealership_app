using System;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using used_car_dealership_app.Models;
using used_car_dealership_app.Services;
using used_car_dealership_app.Services.Interfaces;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels;

//KLASA WIDOKU DO LOGOWANIA SIĘ UŻYTKOWNIKA
[CustomInfo("Widok do logowania się użytkownika", 1.0f)]
public partial class LoginViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<LoginViewModel>();
    
    
    //POLE DLA USŁUGI UŻYTKOWNIKA
    private readonly UserService _userService;
    
    
    //WŁAŚCIWOŚCI DLA NAZWY UŻYTKOWNIKA, HASŁA I CZY UŻYTKOWNIK SIĘ LOGUJE
    [ObservableProperty]
    private String _username;
    
    [ObservableProperty]
    private String _password;
    
    [ObservableProperty]
    private bool _isLoggingIn;
    

    //KONSTRUKTOR
    public LoginViewModel()
    {
        _userService = new UserService();
        
        var attributes = typeof(LoginViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
    }
    
    
    //KOMENDY
    //Komenda do logowania się użytkownika
    [RelayCommand]
    private async void Login()
    {
        if (Username != null && Password != null)
        {
            IsLoggingIn = true;

            var user = await _userService.AuthenticateUser(Username, Password);
            await Task.Delay(1500, new CancellationToken());
            
            if (user != null)
            {
                _logger.LogInformation("Użytkownik zalogowany pomyślnie!");
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var loginWindow = (LoginWindow)desktop.MainWindow;
                    var mainWindow = new MainWindow() { DataContext = new MainWindowViewModel(user) };

                    desktop.MainWindow = mainWindow;

                    mainWindow.Show();
                    loginWindow.Close();

                }
            }
            else
            {
                IsLoggingIn = false;
                
                Username = null;
                Password = null;
                
                _logger.LogError("Błędne dane logowania!");

                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Błąd logowania", "Błędne dane logowania!", ButtonEnum.Ok, Icon.Error);
                var mainWindow = (LoginWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime)
                    .MainWindow;
                await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            }
        }
    }
    
    //Komenda do anulowania logowania
    [RelayCommand]
    public void Cancel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var loginWindow = (LoginWindow)desktop.MainWindow;
            loginWindow.Close();
        }
    }
}