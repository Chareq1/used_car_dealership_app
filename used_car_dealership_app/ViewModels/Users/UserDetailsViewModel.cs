using System;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Services;
using used_car_dealership_app.ViewModels.Clients;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Users;

[CustomInfo("Widok do wyświetlania danych użytkownika", 1.0f)]
public partial class UserDetailsViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<UserDetailsViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly UserRepository _userRepository;
    
    
    //WŁAŚCIWOŚCI
    //Właściwość dla adresu
    [ObservableProperty]
    public String _address = "";
    
    //Właściwość dla użytkownika
    [ObservableProperty]
    private User _user;
    
    
    //KONSTRUKTOR
    public UserDetailsViewModel(Guid userId, UserRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        _userRepository = repository;
        _mainWindowViewModel = mainWindowViewModel;
        
        var attributes = typeof(UserDetailsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadSelectedUser(userId);
    }
    
    
    //METODY
    //Metoda do wczytywania wybranego użytkownika
    private void LoadSelectedUser(Guid userId)
    {
        var userRow = _userRepository.GetUserById(userId);
        User = new User
        {
            UserId = userId,
            Username = userRow["username"].ToString(),
            Name = userRow["name"].ToString(),
            Surname = userRow["surname"].ToString(),
            PESEL = userRow["pesel"].ToString(),
            Email = userRow["email"].ToString(),
            Phone = userRow["phone"].ToString(),
            Street = userRow["street"].ToString(),
            City = userRow["city"].ToString(),
            ZipCode = userRow["zipCode"].ToString(),
            HouseNumber = userRow["houseNumber"].ToString(),
            Type = Enum.TryParse(userRow["type"].ToString(), out UserType userType) ? userType : UserType.WORKER
        };
        
        Address = $"ul. {User.Street} {User.HouseNumber}, {User.ZipCode} {User.City}";
        
        _logger.LogInformation("Wczytano dane użytkownika!");
    }
    
    
    //KOMENDY
    //Komenda do powrotu do listy użytkowników
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new UsersViewModel(_mainWindowViewModel);
        _logger.LogInformation("Powrót do listy użytkowników!");
    }
    
    //Komenda do aktualizacji użytkownika
    [RelayCommand]
    private void UpdateSelectedUser(User user)
    {
        var updateViewModel = new UserUpdateViewModel(user.UserId, _userRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = updateViewModel;
        _logger.LogInformation("Przejście do widoku aktualizacji użytkownika o ID {0}!", user.UserId);
    }
    
    //Komenda do usuwania użytkownika
    [RelayCommand]
    private async void DeleteUser(User user)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie użytkownika",
                "Czy na pewno chcesz usunąć tego użytkownika?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime)
                .MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _userRepository.DeleteUser(user.UserId);
                _mainWindowViewModel.CurrentPage = new UsersViewModel(_mainWindowViewModel);
                _logger.LogInformation("Usunięto użytkownika!");
            }
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Validation Error", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas usuwania użytkownika z bazy danych!");
        }
    }
}