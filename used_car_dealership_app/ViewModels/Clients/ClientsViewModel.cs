using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Services;
using used_car_dealership_app.Views;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace used_car_dealership_app.ViewModels.Clients;

//KLASA WIDOKU DO WYŚWIETLANIA LISTY KLIENTÓW
[CustomInfo("Widok listy klientów", 1.0f)]
public partial class ClientsViewModel : ViewModelBase
{
    //POLE DLA USŁUGI NOTYFIKACJI
    private readonly NotificationService _notifications;


    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<ClientsViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly CustomerRepository _customerRepository;
    private ObservableCollection<Customer> _customers;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DO SPRAWDZANIA CZY SĄ KLIENCI
    [ObservableProperty]
    private bool _areThereCustomers = false;
    
    
    //WŁAŚCIWOŚCI DO WYSZUKIWANA KLIENTÓW
    [ObservableProperty]
    private String _searchText;

    [ObservableProperty]
    private String _selectedSearchField;

    [ObservableProperty] 
    private List<String> _searchFields;
    
    
    //POLE DLA LISTY KLIENTÓW Z BAZY DANYCH
    public ObservableCollection<Customer> Customers
    {
        get => _customers;
        set => SetProperty(ref _customers, value);
    }

    
    //KONSTRUKTOR DLA WIDOKU
    public ClientsViewModel()
    {
        _customerRepository = new CustomerRepository();
        _searchFields = new List<String> { "Imię", "Nazwisko", "Email", "Telefon" };
        
        var attributes = typeof(ClientsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadCustomers();
    }
    
    
    //KONSTRUKTOR DLA WIDOKU Z MAINWINDOWVIEWMODEL
    public ClientsViewModel(MainWindowViewModel mainWindowViewModel) : this()
    {
        _mainWindowViewModel = mainWindowViewModel;
        _notifications = new NotificationService(_mainWindowViewModel);
    }
    
    
    //METODY
    //Metoda do wczytywania klientów z bazy danych
    private async Task LoadCustomers()
    {
        var dataTable = await Task.Run(() => _customerRepository.GetAllCustomers());

        if (dataTable.Rows.Count == 0)
        {
            AreThereCustomers = false;
            _notifications.ShowInfo("Klienci", "Brak klientów!");
        }
        else
        {
            AreThereCustomers = true;
            
            _notifications.ShowInfo("Klienci", "Ilość klientów: " + dataTable.Rows.Count);
            
            var customers = dataTable.AsEnumerable().Select(row => new Customer
            {
                CustomerId = Guid.Parse(row["customerId"].ToString()),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString()
            }).ToList();
            
            Customers = new ObservableCollection<Customer>(customers);
        }
        
        _logger.LogInformation("Pobrano klientów z bazy danych!");
        
    }
    
    //Metoda do wyszukiwania klientów w bazie danych
    [RelayCommand]
    private async Task SearchCustomersAsync()
    {
        var selectedColumn = "";
            
        if (String.IsNullOrEmpty(SearchText) || String.IsNullOrEmpty(SelectedSearchField))
        {
            await LoadCustomers();
            return;
        }

        switch (SelectedSearchField)
        {
            case "Imię":
                selectedColumn = "name";
                break;
            case "Nazwisko":
                selectedColumn = "surname";
                break;
            case "Email":
                selectedColumn = "email";
                break;
            case "Telefon":
                selectedColumn = "phone";
                break;
        }

        var query = $"SELECT * FROM customers WHERE \"{selectedColumn}\" LIKE @searchText";
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@searchText", $"%{SearchText}%")
        };

        var dataTable = await Task.Run(() => _customerRepository.ExecuteQuery(query, parameters));

        if (dataTable.Rows.Count == 0)
        {
            AreThereCustomers = false;
            _notifications.ShowInfo("Klienci", "Brak klientów o podanych informacjach!");
        }
        else
        {
            AreThereCustomers = true;
            
            _notifications.ShowInfo("Klienci", "Ilość znalezionych klientów: " + dataTable.Rows.Count);

            var customers = dataTable.AsEnumerable().Select(row => new Customer
            {
                CustomerId = Guid.Parse(row["customerId"].ToString()),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString()
            }).ToList();

            Customers = new ObservableCollection<Customer>(customers);
        }

        _logger.LogInformation("Wyszukano klientów w bazie danych!");
    }
    
    //Metoda do pokazania szczegółów klienta (przejścia do innego widoku)
    [RelayCommand]
    private void ShowCustomerDetails(Customer customer)
    {
        var detailsViewModel = new ClientDetailsViewModel(customer.CustomerId, _customerRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = detailsViewModel;
        _logger.LogInformation("Przejście do widoków szczegółów klienta o ID {0}!", customer.CustomerId);
    }
    
    //Metoda do przejścia do widoku dodawania klienta
    [RelayCommand]
    private void GoToAddScreen()
    {
        var addViewModel = new ClientAddViewModel(_customerRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = addViewModel;
        _logger.LogInformation("Przejście do widoków dodawania klientów!");
    }
    
    //Metoda do usuwania klienta z bazy danych
    [RelayCommand]
    private async void DeleteCustomer(Customer customer)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie klienta", "Czy na pewno chcesz usunąć tego klienta?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _customerRepository.DeleteCustomer(customer.CustomerId);
                _notifications.ShowSuccess("Usuwanie klienta", "Operacja zakończona pomyślnie!");
                LoadCustomers();
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Problem z usunięciem klienta", "Operacja zakończona niepowodzeniem!");
            _logger.LogError(ex, "Błąd podczas usuwania klienta z bazy danych!");
        }
    }
}
