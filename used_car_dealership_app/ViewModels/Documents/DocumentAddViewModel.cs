using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Avalonia.Controls;
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
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Documents;

//KLASA WIDOKU DODAWANIA DOKUMENTU
[CustomInfo("Widok do dodawania dokumentu", 1.0f)]
public partial class DocumentAddViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<DocumentUpdateViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly CustomerRepository _customerRepository;
    private readonly UserRepository _userRepository;
    private readonly DocumentRepository _documentRepository;
    private readonly VehicleRepository _vehicleRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DLA DOKUMENTU
    [ObservableProperty]
    private Document _document = new Document();
    
    
    //WŁAŚCIWOŚĆ DLA LISTY KLIENTÓW
    [ObservableProperty]
    private ObservableCollection<Customer> _customers;
    
    
    //WŁAŚCIWOŚĆ DLA LISTY POJAZDÓW
    [ObservableProperty]
    private ObservableCollection<Vehicle> _vehicles;
    
    
    //WŁAŚCIWOŚĆ DLA LISTY UŻYTKOWNIKÓW
    [ObservableProperty]
    private ObservableCollection<User> _users;
    
    
    //WŁAŚCIWOŚCI DLA NOWEGO PLIKU
    [ObservableProperty] 
    private String _newFile;
    
    [ObservableProperty]
    private String _newFileName;
    
    [ObservableProperty]
    private bool _isNewFileSelected;
    
    
    //WŁAŚCIWOŚCI DLA WYBRANEJ DATY
    [ObservableProperty]
    private DateTimeOffset _selectedDate;
    
    
    //KONSTRUKTOR
    public DocumentAddViewModel(DocumentRepository documentRepository, MainWindowViewModel mainWindowViewModel)
    {
        _documentRepository = documentRepository;
        _customerRepository = new CustomerRepository();
        _userRepository = new UserRepository();
        _vehicleRepository = new VehicleRepository();
        _mainWindowViewModel = mainWindowViewModel;
        
        SelectedDate = DateTimeOffset.Now;
        Document.DocumentId = Guid.NewGuid();
        Document.User = _mainWindowViewModel.LoggedUser;
        
        var attributes = typeof(DocumentsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }

        LoadCustomers();
        LoadUsers();
        LoadVehicles();
    }
    
    
    //METODY
    //Metoda do wczytywania klientów z bazy danych
    private async Task LoadCustomers()
    {
        var dataTable = await Task.Run(() => _customerRepository.GetAllCustomers());

        if (dataTable.Rows.Count == 0)
        {
            Customers = new ObservableCollection<Customer>(null); }
        else
        {
            var customers = dataTable.AsEnumerable().Select(row => new Customer
            {
                CustomerId = Guid.Parse(row["customerId"].ToString()),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                PESEL = row["pesel"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString()
            }).ToList();
            
            Customers = new ObservableCollection<Customer>(customers);
        }
        
        _logger.LogInformation("Pobrano klientów z bazy danych!");
    }
    
    //Metoda do wczytywania użytkowników z bazy danych
    private async Task LoadUsers()
    {
        var dataTable = await Task.Run(() => _userRepository.GetAllUsers());

        if (dataTable.Rows.Count == 0)
        {
            Users = new ObservableCollection<User>(null); }
        else
        {
            var users = dataTable.AsEnumerable().Select(row => new User
            {
                UserId = Guid.Parse(row["userId"].ToString()),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                PESEL = row["pesel"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString(),
                Type = Enum.TryParse(row["type"].ToString(), out UserType userType) ? userType : UserType.WORKER
            }).ToList();
            
            Users = new ObservableCollection<User>(users);
        }
        
        _logger.LogInformation("Pobrano użytkowników z bazy danych!");
    }
    
    //Metoda do wczytywania pojazdów z bazy danych
    private async Task LoadVehicles()
    {
        var dataTable = await Task.Run(() => _vehicleRepository.GetAllVehicles());

        if (dataTable.Rows.Count == 0)
        {
            Vehicles = new ObservableCollection<Vehicle>(null); }
        else
        {
            var vehicles = dataTable.AsEnumerable().Select(row => new Vehicle
            {
                VehicleId =  Guid.Parse(row["vehicleId"].ToString()),
                Brand = row["brand"].ToString(),
                Model = row["model"].ToString(),
                VIN = row["VIN"].ToString()
            }).ToList();
            
            Vehicles = new ObservableCollection<Vehicle>(vehicles);
        }
        
        _logger.LogInformation("Pobrano pojazdy z bazy danych!");
    }
    
    //Metoda do pokazywania okienka z błędem
    private async Task ShowPopupAsync(String message)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Błąd walidacji", message, ButtonEnum.Ok, Icon.Error);
        var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
        await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
    }
    
    
    //KOMENDY
    //Komenda do usuwania zdjęcia
    [RelayCommand]
    private void DeleteFile(String file)
    {
        NewFile = null;
        NewFileName = null;
            
        IsNewFileSelected = NewFile != null;
    }
    
    //Komenda do powrotu do poprzedniego widoku
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new DocumentsViewModel(_mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku dokumentów!");
    }
    
    //Komenda do dodania nowego zdjęcia
    [RelayCommand]
    private async Task UploadFileAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Dokument PDF", Extensions = new List<string> { "pdf" } },
                new FileDialogFilter { Name = "Zdjęcia", Extensions = new List<string> { "jpg", "jpeg", "png", "bmp" } },
                new FileDialogFilter { Name = "Word", Extensions = new List<string> { "docx", "doc", "xlsx", "xls", "pptx", "ppt" } },
                new FileDialogFilter { Name = "OpenOffice", Extensions = new List<string> { "odt", "ods", "odp", "odg" } }
            }
        };
        var mainWin = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
        var result = await openFileDialog.ShowAsync(mainWin);
        
        if (result != null && result.Length > 0)
        {
            try
            {
                if (NewFile != null)
                {
                    await ShowPopupAsync("Można dodać tylko jeden plik!");
                    _logger.LogError("Można dodać tylko jeden plik!");
                    throw new ValidationException("Można dodać tylko jeden plik!");
                }

                var filePath = result[0];

                NewFile = filePath;
                IsNewFileSelected = NewFile != null;
                NewFileName = Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Błąd z dodawaniem pliku", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
                await messageBoxStandardWindow.ShowAsPopupAsync(mainWin);
                
                _logger.LogError(ex, "Błąd podczas dodawania pliku!");
            }
        }
    }
    
    //Komenda do dodania dokumentu do bazy danych
    [RelayCommand]
    private async Task AddDocumentToDatabaseAsync()
    {
        try
        {
            if(NewFile == null)
            {
                await ShowPopupAsync("Dodaj plik do dokumentu!");
                _logger.LogError("Dodaj plik do dokumentu!");
                throw new ValidationException("Dodaj plik do dokumentu!");
            }
            
            if (NewFile != null)
            {
                Env.Load();
                
                var newFilePath = Path.Combine(Env.GetString("DOCUMENTS_FOLDER_PATH"), Path.GetFileName(NewFile));
                File.Copy(NewFile, newFilePath, true);
                Document.File = newFilePath;
            }
            
            Document.CreationDate = SelectedDate.DateTime;
            _documentRepository.AddDocument(Document);
            
            _mainWindowViewModel.CurrentPage = new DocumentsViewModel(_mainWindowViewModel);
            _logger.LogInformation("Dodano dokument do bazy danych!");
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Błąd z dodawaniem dokumentu", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas dodawania dokumentu w bazie danych!");
        }
    }
}