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

namespace used_car_dealership_app.ViewModels.Documents;

//KLASA WIDOKU DO WYŚWIETLANIA LISTY DOKUMENTÓW
[CustomInfo("Widok listy dokumentów", 1.0f)]
public partial class DocumentsViewModel : ViewModelBase
{
    //POLE DLA USŁUGI NOTYFIKACJI
    private readonly NotificationService _notifications;
    
    
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<DocumentsViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly CustomerRepository _customerRepository;
    private readonly UserRepository _userRepository;
    private readonly DocumentRepository _documentRepository;
    private readonly VehicleRepository _vehicleRepository;
    private ObservableCollection<Document> _documents;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DO SPRAWDZANIA CZY SĄ DOKUMENTY
    [ObservableProperty]
    private bool _areThereDocuments = false;
    
    
    //WŁAŚCIWOŚCI DO WYSZUKIWANA DOKUMENTÓW
    [ObservableProperty]
    private String _searchText;

    [ObservableProperty]
    private String _selectedSearchField;

    [ObservableProperty] 
    private List<String> _searchFields;
    
    
    //POLE DLA LISTY DOKUMENTÓW Z BAZY DANYCH
    public ObservableCollection<Document> Documents
    {
        get => _documents;
        set => SetProperty(ref _documents, value);
    }
    
    
    //KONSTRUKTOR DLA WIDOKU
    public DocumentsViewModel()
    {
        _documentRepository = new DocumentRepository();
        _customerRepository = new CustomerRepository();
        _userRepository = new UserRepository();
        _vehicleRepository = new VehicleRepository();
        
        _searchFields = new List<String> { "Opis", "Nazwisko klienta", "VIN pojazdu", "Nazwisko użytkownika" };
        
        var attributes = typeof(DocumentsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadDocuments();
    }
    
    
    //KONSTRUKTOR DLA WIDOKU Z MAINWINDOWVIEWMODEL
    public DocumentsViewModel(MainWindowViewModel mainWindowViewModel) : this()
    {
        _mainWindowViewModel = mainWindowViewModel;
        _notifications = new NotificationService(_mainWindowViewModel);
    }
    
    
    //METODY
    //Metoda do wczytywania dokumentów z bazy danych
    private async Task LoadDocuments()
    {
        var query = "SELECT * FROM documents ORDER BY \"creationDate\" DESC";
        var parameters = new List<NpgsqlParameter>();
        var dataTable = await Task.Run<DataTable>(() => _documentRepository.ExecuteQuery(query, parameters));

        if (dataTable.Rows.Count == 0)
        {
            AreThereDocuments = false;
            _notifications.ShowInfo("Dokumenty", "Brak dokumentów!");
        }
        else
        {
            AreThereDocuments = true;
            
            _notifications.ShowInfo("Dokumenty", "Ilość dokumentów: " + dataTable.Rows.Count);

            var documents = dataTable.AsEnumerable().Select(row => new Document
            {
                DocumentId = Guid.Parse(row["documentId"].ToString()),
                Description = row["description"].ToString(),
                Customer = LoadSelectedCustomer(Guid.Parse(row["customerId"].ToString())),
                User = LoadSelectedUser(Guid.Parse(row["userId"].ToString())),
                Vehicle = LoadSelectedVehicle(Guid.Parse(row["vehicleId"].ToString())),
                CreationDate = DateTime.Parse(row["creationDate"].ToString())
            }).ToList();

            Documents = new ObservableCollection<Document>(documents);
        }

        _logger.LogInformation("Pobrano dokumenty z bazy danych!");
    }
    
    //Metoda do wczytywania wybranego klienta
    private Customer LoadSelectedCustomer(Guid customerId)
    {
        var customerRow = _customerRepository.GetCustomerById(customerId);
        _logger.LogInformation("Wczytano dane klienta!");
        
        return new Customer
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
    }
    
    //Metoda do wczytywania wybranego użytkonika
    private User LoadSelectedUser(Guid userId)
    {
        var userRow = _userRepository.GetUserById(userId);
        _logger.LogInformation("Wczytano dane użytkownika!");
        
        return new User
        {
            UserId = userId,
            Name = userRow["name"].ToString(),
            Surname = userRow["surname"].ToString(),
            Email = userRow["email"].ToString(),
            Phone = userRow["phone"].ToString(),
            Type = Enum.TryParse(userRow["type"].ToString(), out UserType userType) ? userType : UserType.WORKER
        };
    }
    
    //Metoda do wczytywania wybranego pojazdu
    private Vehicle LoadSelectedVehicle(Guid vehicleId)
    {
        var vehicleRow = _vehicleRepository.GetVehicleById(vehicleId);
        _logger.LogInformation("Wczytano dane pojazdu!");
        
        return new Vehicle
        {
            VehicleId = vehicleId,
            Brand = vehicleRow["brand"].ToString(),
            Model = vehicleRow["model"].ToString(),
            VIN = vehicleRow["VIN"].ToString()
        };
    }
    
    //Metoda do wyszukiwania dokumentów w bazie danych
    [RelayCommand]
    private async Task SearchDocumentsAsync()
    {
        var selectedColumn = "";

        if (String.IsNullOrEmpty(SearchText) || String.IsNullOrEmpty(SelectedSearchField))
        {
            await LoadDocuments();
            return;
        }
        
        var query = "";

        switch (SelectedSearchField)
        {
            case "Opis":
                query = $"SELECT * FROM documents WHERE \"description\" LIKE @searchText ORDER BY \"creationDate\" DESC";
                break;
            case "Nazwisko klienta":
                query = $"SELECT * FROM \"documents\" WHERE \"customerId\" IN (SELECT \"customerId\" FROM \"customers\" WHERE \"surname\" LIKE @searchText) ORDER BY \"creationDate\" DESC";
                break;
            case "VIN pojazdu":
                query = $"SELECT * FROM \"documents\" WHERE \"vehicleId\" IN (SELECT \"vehicleId\" FROM \"vehicles\" WHERE \"VIN\" LIKE @searchText) ORDER BY \"creationDate\" DESC";
                break;
            case "Nazwisko użytkownika":
                query = $"SELECT * FROM \"documents\" WHERE \"userId\" IN (SELECT \"userId\" FROM \"users\" WHERE \"surname\" LIKE @searchText) ORDER BY \"creationDate\" DESC";
                break;
        }
        
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@searchText", $"%{SearchText}%")
        };

        var dataTable = await Task.Run(() => _documentRepository.ExecuteQuery(query, parameters));

        if (dataTable.Rows.Count == 0)
        {
            AreThereDocuments = false;
            _notifications.ShowInfo("Dokumenty", "Brak dokumentów o podanych informacjach!");
        }
        else
        {
            AreThereDocuments = true;
            
            _notifications.ShowInfo("Dokumenty", "Ilość znalezionych dokumentów: " + dataTable.Rows.Count);

            var documents = dataTable.AsEnumerable().Select(row => new Document
            {
                DocumentId = Guid.Parse(row["documentId"].ToString()),
                Description = row["description"].ToString(),
                Customer = LoadSelectedCustomer(Guid.Parse(row["customerId"].ToString())),
                User = LoadSelectedUser(Guid.Parse(row["userId"].ToString())),
                Vehicle = LoadSelectedVehicle(Guid.Parse(row["vehicleId"].ToString())),
                CreationDate = DateTime.Parse(row["creationDate"].ToString())
            }).ToList();

            Documents = new ObservableCollection<Document>(documents);
        }

        _logger.LogInformation("Wyszukano dokumenty w bazie danych!");
    }
    
    //Metoda do pokazania szczegółów dokumentu (przejścia do innego widoku)
    [RelayCommand]
    private void ShowDocumentDetails(Document document)
    {
        var detailsViewModel = new DocumentDetailsViewModel(document.DocumentId, _documentRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = detailsViewModel;
        _logger.LogInformation("Przejście do widoków szczegółów dokumentu o ID {0}!", document.DocumentId);
        
    }
        
    //Metoda do przejścia do widoku dodawania dokumentu
    [RelayCommand]
    private void GoToAddScreen()
    {
        var addViewModel = new DocumentAddViewModel(_documentRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = addViewModel;
        _logger.LogInformation("Przejście do widoku dodawania dokumentu! ");
    }
    
    //Metoda do usuwania dokumentu z bazy danych
    [RelayCommand]
    private async void DeleteDocument(Document document)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie dokumentu", "Czy na pewno chcesz usunąć ten dokument?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _documentRepository.DeleteDocument(document.DocumentId);
                _notifications.ShowSuccess("Usuwanie dokumentu", "Operacja zakończona pomyślnie!");
                LoadDocuments();
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Problem z usunięciem dokumentu", ex.Message);
            _logger.LogError(ex, "Błąd podczas usuwania użytkownika z bazy danych!");
        }
    }
}