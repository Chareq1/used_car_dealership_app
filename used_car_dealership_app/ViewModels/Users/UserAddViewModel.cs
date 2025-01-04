using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Users;

//KLASA WIDOKU DO DODAWANIA UŻYTKOWNIKA
[CustomInfo("Widok do dodawania użytkownika", 1.0f)]
public partial class UserAddViewModel : ViewModelBase
{
    //POLE DLA USŁUGI NOTYFIKACJI
    private readonly NotificationService _notifications;
    
    
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<UserUpdateViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly UserRepository _userRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DLA UŻTYKOWNIKA
    [ObservableProperty]
    private User _user = new User();
    
    //WŁAŚCIWOŚĆ DLA LISTY TYPÓW UŻYTKOWNIKÓW
    public ObservableCollection<UserType> UserTypes { get; } = new ObservableCollection<UserType>(Enum.GetValues(typeof(UserType)).Cast<UserType>());
    
    
    //KONSTRUKTOR
    public UserAddViewModel(UserRepository userRepository, MainWindowViewModel mainWindowViewModel)
    {
        _userRepository = userRepository;
        _mainWindowViewModel = mainWindowViewModel;
        _notifications = new NotificationService(_mainWindowViewModel);
        
        User.UserId = Guid.NewGuid();
        
        var attributes = typeof(UserAddViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
    }
    
    
    //METODY
    //Metoda do sprawdzania poprawności numeru PESEL
    private bool IsValidPESEL(String pesel)
    {
        if (pesel.Length != 11 || !long.TryParse(pesel, out _)) { return false; }
        
        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;

        for (int i = 0; i < weights.Length; i++) { sum += weights[i] * (pesel[i] - '0'); }

        int controlDigit = (10 - (sum % 10)) % 10;

        return controlDigit == (pesel[10] - '0');
    }
    
    //Metoda do walidacji pola
    private async Task ValidateInputAsync(String input, String pattern, String errorMessage)
    {
        if (!Regex.IsMatch(input, pattern))
        {
            _notifications.ShowError("Błąd walidacji", errorMessage);
            _logger.LogError(errorMessage, "Błąd walidacji pola!");
            throw new ValidationException(errorMessage);
        }
    }
    
    //Metoda do walidacji wszystkich pól
    private async Task<bool> ValidateFieldsAsync()
    {
        try
        {
            await ValidateInputAsync(User.Username, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9.\-+/@^&*()\s]+$", "Niepoprawny format nazwy!");
            await ValidateInputAsync(User.Password, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9.\-+/@^&*()\s#]+$", "Niepoprawny format hasła!");
            await ValidateInputAsync(User.Email, @"^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$", "Niepoprawny format adresu email!");
            await ValidateInputAsync(User.Name, "^[A-ZĄĆĘŁŃÓŚŹŻ]{1}[a-ząćęłńóśźż]+$", "Niepoprawny format imienia!");
            await ValidateInputAsync(User.Surname, "^[A-ZĄĆĘŁŃÓŚŹŻ]{1}[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż-]+$", "Niepoprawny format nazwiska!");
            await ValidateInputAsync(User.Phone, "^[0-9]+$", "Niepoprawny format numeru telefonu!");
            await ValidateInputAsync(User.ZipCode, "^[0-9-]{2}[-][0-9]{3}$", "Niepoprawny format kodu pocztowego!");
            await ValidateInputAsync(User.City, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż-]+$", "Niepoprawny format miasta!");
            await ValidateInputAsync(User.Street, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż\-\/]+$", "Niepoprawny format ulicy!");
            
            if(User.Password.Length < 8 || 
               !Regex.IsMatch(User.Password, @"[A-Z]") || 
               !Regex.IsMatch(User.Password, @"[a-z]") || 
               !Regex.IsMatch(User.Password, @"[0-9]") || 
               !Regex.IsMatch(User.Password, @"[\W_]"))
            {
                _notifications.ShowWarning("Błąd z hasłem", "Hasło musi mieć co najmniej 8 znaków, zawierać co najmniej 1 dużą literę, 1 małą literę, 1 cyfrę i 1 znak specjalny!");
                _logger.LogError("Hasło musi mieć co najmniej 8 znaków, zawierać co najmniej 1 dużą literę, 1 małą literę, 1 cyfrę i 1 znak specjalny!");

                User.Password = null;
                
                throw new ValidationException("Hasło musi mieć co najmniej 8 znaków, zawierać co najmniej 1 dużą literę, 1 małą literę, 1 cyfrę i 1 znak specjalny!");
            }
            
            if (!IsValidPESEL(User.PESEL))
            {
                _notifications.ShowError("Problem z numerem PESEL", "Niepoprawny numer PESEL!");
                _logger.LogError("Niepoprawny numer PESEL!");
                throw new ValidationException("Niepoprawny numer PESEL!");
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    
    //KOMENDY
    //Komenda do powrotu do poprzedniego widoku
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new UsersViewModel(_mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku użytkowników!");
    }
    
    //Komenda do dodawania użytkownika do bazy danych
    [RelayCommand]
    private async Task AddUserToDatabaseAsync()
    {
        try
        {
            if (await ValidateFieldsAsync())
            {
                _userRepository.AddUser(User);
                _notifications.ShowSuccess("Dodawanie użytkownika", "Operacja zakończona pomyślnie!");
                _mainWindowViewModel.CurrentPage = new UsersViewModel(_mainWindowViewModel);
                _logger.LogInformation("Dodano użytkownika do bazy danych!");
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Problem z dodaniem użytkownika", ex.Message);
            _logger.LogError(ex, "Błąd podczas dodawania użytkownika do bazy danych!");
        }
    }
}