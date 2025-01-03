using System;
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

[CustomInfo("Widok do wyświetlania danych klienta", 1.0f)]
public partial class ClientDetailsViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<ClientDetailsViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly CustomerRepository _customerRepository;
    
    
    //WŁAŚCIWOŚCI
    //Właściwość dla adresu
    [ObservableProperty]
    public String _address = "";
    
    //Właściwość dla klienta
    [ObservableProperty]
    private Customer _customer;

    
    //KONSTRUKTOR
    public ClientDetailsViewModel(Guid customerId, CustomerRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        _customerRepository = repository;
        _mainWindowViewModel = mainWindowViewModel;
        
        var attributes = typeof(ClientDetailsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
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
        
        Address = $"ul. {Customer.Street} {Customer.HouseNumber}, {Customer.ZipCode} {Customer.City}";
        
        _logger.LogInformation("Wczytano dane klienta!");
    }
    
    
    //KOMENDY
    //Komenda do powrotu do listy klientów
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new ClientsViewModel(_mainWindowViewModel);
        _logger.LogInformation("Powrót do listy klientów!");
    }
    
    //Komenda do aktualizacji klienta
    [RelayCommand]
    private void UpdateSelectedCustomer(Customer customer)
    {
        var updateViewModel = new ClientUpdateViewModel(customer.CustomerId, _customerRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = updateViewModel;
        _logger.LogInformation("Przejście do widoku aktualizacji klienta o ID {0}!", customer.CustomerId);
    }
    
    //Komenda do usuwania klienta
    [RelayCommand]
    private async void DeleteCustomer(Customer customer)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie klienta",
                "Czy na pewno chcesz usunąć tego klienta?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime)
                .MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _customerRepository.DeleteCustomer(customer.CustomerId);
                _mainWindowViewModel.CurrentPage = new ClientsViewModel(_mainWindowViewModel);
                _logger.LogInformation("Usunięto klienta!");
            }
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Validation Error", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas usuwania klienta z bazy danych!");
        }
    }
}