using System;
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
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Clients;

[CustomInfo("Widok do aktualizowania danych klienta", 1.0f)]
public partial class ClientUpdateViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<ClientUpdateViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly CustomerRepository _customerRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    //WŁAŚCIWOŚĆ DLA KLIENTA
    [ObservableProperty]
    private Customer _customer = new Customer();
    
    
    //KONSTRUKTOR
    public ClientUpdateViewModel(Guid customerId, CustomerRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        _customerRepository = repository;
        _mainWindowViewModel = mainWindowViewModel;
        
        var attributes = typeof(ClientUpdateViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadSelectedCustomer(customerId);
    }
    
    
    //METODY
    //Metoda do wczytywania wybranego klienta
    private void LoadSelectedCustomer(Guid customerId)
    {
        var customerRow = _customerRepository.GetCustomerById(customerId);
        Customer = new Customer
        {
            CustomerId = customerId,
            Name = customerRow["name"].ToString(),
            Surname = customerRow["surname"].ToString(),
            PESEL = customerRow["pesel"].ToString(),
            IdCardNumber = customerRow["idCardNumber"].ToString(),
            Email = customerRow["email"].ToString(),
            Phone = customerRow["phone"].ToString(),
            Street = customerRow["street"].ToString(),
            City = customerRow["city"].ToString(),
            ZipCode = customerRow["zipCode"].ToString(),
            HouseNumber = customerRow["houseNumber"].ToString()
        };
        
        _logger.LogInformation("Wczytano dane klienta!");
    }
    
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
            throw new ValidationException(errorMessage);
        }
    }
    
    //Metoda do walidacji wszystkich pól
    private async Task<bool> ValidateFieldsAsync()
    {
        try
        {
            await ValidateInputAsync(Customer.Email, @"^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$", "Niepoprawny format adresu email!");
            await ValidateInputAsync(Customer.Name, "^[A-ZĄĆĘŁŃÓŚŹŻ]{1}[a-ząćęłńóśźż]+$", "Niepoprawny format imienia!");
            await ValidateInputAsync(Customer.Surname, "^[A-ZĄĆĘŁŃÓŚŹŻ]{1}[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż-]+$", "Niepoprawny format nazwiska!");
            await ValidateInputAsync(Customer.IdCardNumber, "^[A-Z]{3}[0-9]{6}$", "Niepoprawny format numeru dowodu osobistego!");
            await ValidateInputAsync(Customer.Phone, "^[0-9]+$", "Niepoprawny format numeru telefonu!");
            await ValidateInputAsync(Customer.ZipCode, "^[0-9-]{2}[-][0-9]{3}$", "Niepoprawny format kodu pocztowego!");
            await ValidateInputAsync(Customer.City, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż-]+$", "Niepoprawny format miasta!");
            await ValidateInputAsync(Customer.Street, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż\-\/]+$", "Niepoprawny format ulicy!");
            
            if (!IsValidPESEL(Customer.PESEL))
            {
                await ShowPopupAsync("Niepoprawny numer PESEL!");
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
        _mainWindowViewModel.CurrentPage = new ClientDetailsViewModel(Customer.CustomerId, _customerRepository, _mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku wybranego klienta!");
    }
    
    //Komenda do aktualizacji danych klienta w bazie danych
    [RelayCommand]
    private async Task UpdateClientDataInDatabaseAsync()
    {
        if (await ValidateFieldsAsync())
        {
            _customerRepository.UpdateCustomer(Customer);
            _mainWindowViewModel.CurrentPage = new ClientDetailsViewModel(Customer.CustomerId, _customerRepository, _mainWindowViewModel);
            _logger.LogInformation("Zaktualizowano klienta w bazie danych!");
        }
    }
}