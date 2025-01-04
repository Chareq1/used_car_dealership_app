using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
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

namespace used_car_dealership_app.ViewModels.Documents;

//KLASA WIDOKU DO WYŚWIETLANIA SZCZEGÓŁÓW DOKUMENTU
[CustomInfo("Widok szczegółów dokumentu", 1.0f)]
public partial class DocumentDetailsViewModel : ViewModelBase
{
    //POLE DLA USŁUGI NOTYFIKACJI
    private readonly NotificationService _notifications;
    
    
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<DocumentDetailsViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly CustomerRepository _customerRepository;
    private readonly UserRepository _userRepository;
    private readonly DocumentRepository _documentRepository;
    private readonly VehicleRepository _vehicleRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DLA DOKUMENTU
    [ObservableProperty]
    private Document _document = new Document();
    
    
    //KONSTRUKTOR
    public DocumentDetailsViewModel(Guid documentId, DocumentRepository documentRepository, MainWindowViewModel mainWindowViewModel)
    {
        _documentRepository = documentRepository;
        _customerRepository = new CustomerRepository();
        _userRepository = new UserRepository();
        _vehicleRepository = new VehicleRepository();
        _mainWindowViewModel = mainWindowViewModel;
        _notifications = new NotificationService(_mainWindowViewModel);
        
        var attributes = typeof(DocumentsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadSelectedDocument(documentId);
    }
    
    
    //METODY
    //Metoda do wczytywania wybranego dokumentu
    private void LoadSelectedDocument(Guid documentId)
    {
        var documentRow = _documentRepository.GetDocumentById(documentId);
        
        Document = new Document
        {
            DocumentId = documentId,
            Description = documentRow["description"].ToString(),
            File = documentRow["file"].ToString(),
            CreationDate = Convert.ToDateTime(documentRow["creationDate"]),
            Customer = LoadSelectedCustomer(Guid.Parse(documentRow["customerId"].ToString())),
            User = LoadSelectedUser(Guid.Parse(documentRow["userId"].ToString())),
            Vehicle = LoadSelectedVehicle(Guid.Parse(documentRow["vehicleId"].ToString()))
        };
        
        _logger.LogInformation("Wczytano dane dokumentu!");
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
    
    
    //KOMENDY
    //Komenda do powrotu do listy dokumentów
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new DocumentsViewModel(_mainWindowViewModel);
        _logger.LogInformation("Powrót do listy dokumentów!");
    }
    
    //Komenda do aktualizacji dokumentu
    [RelayCommand]
    private void UpdateSelectedDocument(Document document)
    {
        var updateViewModel = new DocumentUpdateViewModel(document.DocumentId, _documentRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = updateViewModel;
        _logger.LogInformation("Przejście do widoku aktualizacji dokumentu ID {0}!", document.DocumentId);
        
    }
    
    //Komenda do zapisu dokumentu lokalnie
    [RelayCommand]
    private async Task SaveFileAsync()
    {
        var saveFileDialog = new SaveFileDialog
        {
            InitialFileName = Path.GetFileName(Document.File),
            DefaultExtension = Path.GetExtension(Document.File),
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Dokument PDF", Extensions = new List<string> { "pdf" } },
                new FileDialogFilter { Name = "Zdjęcia", Extensions = new List<string> { "jpg", "jpeg", "png", "bmp" } },
                new FileDialogFilter { Name = "Word", Extensions = new List<string> { "docx", "doc", "xlsx", "xls", "pptx", "ppt" } },
                new FileDialogFilter { Name = "OpenOffice", Extensions = new List<string> { "odt", "ods", "odp", "odg" } }
            }
        };
        var mainWin = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
        var result = await saveFileDialog.ShowAsync(mainWin);
        
        if (result != null && result.Length > 0)
        {
            var filePath = Document.File;
            String destinationPath = Path.Combine(result);
            
            Console.WriteLine("-------------");
            Console.WriteLine(filePath);
            Console.WriteLine(destinationPath);
            Console.WriteLine("-------------");
            
            File.Copy(filePath, destinationPath, true);
            
            _notifications.ShowSuccess("Zapis dokumentu", "Plik lokalnie zapisany w " + destinationPath);
            _logger.LogInformation("Plik lokalnie zapisany w {0}", destinationPath);
        }
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
                _mainWindowViewModel.CurrentPage = new DocumentsViewModel(_mainWindowViewModel);
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Problem z usunięciem dokumentu", ex.Message);
            _logger.LogError(ex, "Błąd podczas usuwania dokuemntu z bazy danych!");
        }
    }
}