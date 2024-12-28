using System;
using System.Collections.ObjectModel;
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

[CustomInfo("Widok do dodawania użytkownika", 1.0f)]
public partial class UserAddViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<UserUpdateViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly UserRepository _userRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    //WŁAŚCIWOŚĆ DLA UŻTYKOWNIKA
    [ObservableProperty]
    private User _user = new User();
    
    public ObservableCollection<UserType> UserTypes { get; } = new ObservableCollection<UserType>(Enum.GetValues(typeof(UserType)).Cast<UserType>());
    
    
    //KONSTRUKTOR
    public UserAddViewModel(UserRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        _userRepository = repository;
        
        var attributes = typeof(UserAddViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        _mainWindowViewModel = mainWindowViewModel;
    }
    
    
    //METODY
        //Metoda do sprawdzania poprawności numeru PESEL
    private bool IsValidPESEL(string pesel)
    {
        if (pesel.Length != 11 || !long.TryParse(pesel, out _)) { return false; }
        
        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;

        for (int i = 0; i < weights.Length; i++) { sum += weights[i] * (pesel[i] - '0'); }

        int controlDigit = (10 - (sum % 10)) % 10;

        return controlDigit == (pesel[10] - '0');
    }
    
    //Metoda do walidacji pola
    private async Task ValidateInputAsync(string input, string pattern, string errorMessage)
    {
        if (!Regex.IsMatch(input, pattern))
        {
            await ShowPopupAsync(errorMessage);
            _logger.LogError(errorMessage, "Błąd walidacji pola!");
            throw new Exception(errorMessage);
        }
    }
    
    //Metoda do walidacji wszystkich pól
    private async Task<bool> ValidateFieldsAsync()
    {
        try
        {
            await ValidateInputAsync(User.Username, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9.\-+/@^&*()\s]+$", "Niepoprawny format nazwy!");
            await ValidateInputAsync(User.Password, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9.\-+/@^&*()\s]+$", "Niepoprawny format hasła!");
            await ValidateInputAsync(User.Email, @"^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$", "Niepoprawny format adresu email!");
            await ValidateInputAsync(User.Name, "^[A-ZĄĆĘŁŃÓŚŹŻ]{1}[a-ząćęłńóśźż]+$", "Niepoprawny format imienia!");
            await ValidateInputAsync(User.Surname, "^[A-ZĄĆĘŁŃÓŚŹŻ]{1}[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż-]+$", "Niepoprawny format nazwiska!");
            await ValidateInputAsync(User.Phone, "^[0-9]+$", "Niepoprawny format numeru telefonu!");
            await ValidateInputAsync(User.ZipCode, "^[0-9-]{2}[-][0-9]{3}$", "Niepoprawny format kodu pocztowego!");
            await ValidateInputAsync(User.City, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż-]+$", "Niepoprawny format miasta!");
            await ValidateInputAsync(User.Street, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż\-\/]+$", "Niepoprawny format ulicy!");
            
            if(User.Password.Length < 8)
            {
                await ShowPopupAsync("Hasło musi mieć co najmniej 8 znaków!");
                _logger.LogError("Hasło musi mieć co najmniej 8 znaków!");
                throw new Exception("Hasło musi mieć co najmniej 8 znaków!");
            }
            
            if (!IsValidPESEL(User.PESEL))
            {
                await ShowPopupAsync("Niepoprawny numer PESEL!");
                _logger.LogError("Niepoprawny numer PESEL!");
                throw new Exception("Niepoprawny numer PESEL!");
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
        if (await ValidateFieldsAsync())
        {
            User.UserId = Guid.NewGuid();
            _userRepository.AddUser(User);
            _mainWindowViewModel.CurrentPage = new UsersViewModel(_mainWindowViewModel);
            _logger.LogInformation("Dodano użytkownika do bazy danych!");
        }
    }
}