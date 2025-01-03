using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Services;
using used_car_dealership_app.ViewModels.Users;
using used_car_dealership_app.ViewModels.Vehicles;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels;

[CustomInfo("Widok profilu użytkownika", 1.0f)]
public partial class ProfileViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<ProfileViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly UserRepository _userRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    //WŁAŚCIWOŚĆ DLA UŻTYKOWNIKA
    [ObservableProperty]
    private User _user = new User();
    
    [ObservableProperty]
    private String _fullName = "";

    [ObservableProperty] 
    private String _newPassword = null;
    
    [ObservableProperty] 
    private String _newRepeatedPassword = null;

    [ObservableProperty] 
    private String _address = null;
    
    
    //KONSTRUKTOR
    public ProfileViewModel(Guid userId, UserRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        _userRepository = repository;
        _mainWindowViewModel = mainWindowViewModel;
        
        var attributes = typeof(ProfileViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
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
            Password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(userRow["password"].ToString())),
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
        FullName = $"{User.Name} {User.Surname}";
        _logger.LogInformation("Wczytano dane użytkownika!");
    }
    
    //Metoda do walidacji pola
    private async Task ValidateInputAsync(string input, string pattern, string errorMessage)
    {
        if (!Regex.IsMatch(input, pattern))
        {
            await ShowPopupAsync(errorMessage);
            _logger.LogError(errorMessage, "Błąd walidacji pola!");
            throw new ValidationException(errorMessage);
        }
    }
    
    //Metoda do walidacji nowego hasła
    private async Task<bool> ValidatePasswordAsync()
    {
        try
        {
            await ValidateInputAsync(NewPassword, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9.\-+/@^&*()\s#]+$", "Niepoprawny format hasła!");
            
            if (NewPassword != NewRepeatedPassword)
            {
                await ShowPopupAsync("Podane hasła nie są takie same!");
                _logger.LogError("Podane hasła nie są takie same!");
                
                NewPassword = null;
                NewRepeatedPassword = null;
                
                throw new ValidationException("Hasła nie są takie same!");
            }
            
            if(User.Password.Length < 8 || 
               !Regex.IsMatch(User.Password, @"[A-Z]") || 
               !Regex.IsMatch(User.Password, @"[a-z]") || 
               !Regex.IsMatch(User.Password, @"[0-9]") || 
               !Regex.IsMatch(User.Password, @"[\W_]"))
            {
                await ShowPopupAsync("Hasło musi mieć co najmniej 8 znaków, zawierać co najmniej 1 dużą literę, 1 małą literę, 1 cyfrę i 1 znak specjalny!");
                _logger.LogError("Hasło musi mieć co najmniej 8 znaków, zawierać co najmniej 1 dużą literę, 1 małą literę, 1 cyfrę i 1 znak specjalny!");

                NewPassword = null;
                NewRepeatedPassword = null;
                
                throw new ValidationException("Hasło musi mieć co najmniej 8 znaków, zawierać co najmniej 1 dużą literę, 1 małą literę, 1 cyfrę i 1 znak specjalny!");
            }

            return true;
            
        }
        catch
        {
            return false;
        }
    }
    
    //Metoda do pokazywania okienka z błędem
    private async Task ShowPopupAsync(string message)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Validation Error", message, ButtonEnum.Ok, Icon.Error);
        var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
        await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
    }
    
    
    //KOMENDY
    //Komenda do aktualizacji hasła użytkownika w bazie danych
    [RelayCommand]
    private async Task UpdateUserPasswordAsync()
    {
        if (NewPassword != null && NewRepeatedPassword != null && await ValidatePasswordAsync())
        {
            User.Password = NewPassword;
            _userRepository.UpdateUser(User);
            
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Potwierdzenie zmiany", "Hasło użytkownika zostało zmienione!", ButtonEnum.Ok, Icon.Info);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            NewPassword = null;
            NewRepeatedPassword = null;
            
            _logger.LogInformation("Zaktualizowano użytkownika w bazie danych!");
        }
    }
}