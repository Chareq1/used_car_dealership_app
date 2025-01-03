using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Npgsql;
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Services;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Vehicles;

//KLASA WIDOKU DO LISTY POJAZDÓW
[CustomInfo("Widok listy pojazdów", 1.0f)]
public partial class VehiclesViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<VehiclesViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly VehicleRepository _vehicleRepository;
    private readonly ImageRepository _imageRepository;
    private ObservableCollection<Vehicle> _vehicles;
    
    
    //WŁAŚCIWOŚĆ DO SPRAWDZANIA CZY SĄ POJAZDY
    [ObservableProperty]
    private bool _areThereVehicles;

    
    //WŁAŚCIWOŚCI DO WYSZUKIWANA POJAZDÓW
    [ObservableProperty]
    private String _searchText;

    [ObservableProperty]
    private String _selectedSearchField;

    [ObservableProperty]
    private List<String> _searchFields;
    
    
    //POLE DLA LISTY POJAZDÓW Z BAZY DANYCH
    public ObservableCollection<Vehicle> Vehicles
    {
        get => _vehicles;
        set => SetProperty(ref _vehicles, value);
    }
    
    
    //KONSTRUKTOR DLA WIDOKU
    public VehiclesViewModel()
    {
        _vehicleRepository = new VehicleRepository();
        _imageRepository = new ImageRepository();
        _searchFields = new List<string> { "Marka", "Model", "VIN", "Typ pojazdu" };
        
        var attributes = typeof(VehiclesViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadVehicles();
    }
    
    
    //KONSTRUKTOR DLA WIDOKU Z MAINWINDOWVIEWMODEL
    public VehiclesViewModel(MainWindowViewModel mainWindowViewModel) : this()
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
    
    
    //METODY
    //Metoda do wczytywania pojazdów z bazy danych
    private async Task LoadVehicles()
    {
        var dataTable = await Task.Run(() => _vehicleRepository.GetAllVehicles());
        
        if (dataTable.Rows.Count == 0)
        {
            AreThereVehicles = false;
        }
        else
        {
            AreThereVehicles = true;

            var vehicles = dataTable.AsEnumerable().Select(row => new Vehicle
            {
                VehicleId = Guid.Parse(row["vehicleId"].ToString()),
                Brand = row["brand"].ToString(),
                Model = row["model"].ToString(),
                VIN = row["VIN"].ToString(),
                Type = Enum.TryParse(row["type"].ToString(), out VehicleType vehicleType) ? vehicleType : VehicleType.Samochód
            }).ToList();
            
            foreach (var vehicle in vehicles)
            {
                var images = await Task.Run(() => _imageRepository.GetImagesByVehicleId(vehicle.VehicleId));
                vehicle.Images = images.AsEnumerable().Select(row => new Image
                {
                    ImageId = Guid.Parse(row["imageId"].ToString()),
                    FileName = row["fileName"].ToString(),
                    FilePath = row["filePath"].ToString(),
                    VehicleId = vehicle.VehicleId
                }).ToList();

                vehicle.FirstImage = vehicle.Images.Count > 0 ? vehicle.Images[0] : null;
            }
            
            Vehicles = new ObservableCollection<Vehicle>(vehicles);
        }
        
        _logger.LogInformation("Pobrano pojazdy z bazy danych!");
    }
    
    //Metoda do wyszukiwania pojazdów w bazie danych
    [RelayCommand]
    private async Task SearchVehiclesAsync()
    {
        var selectedColumn = "";

        if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchField))
        {
            await LoadVehicles();
            return;
        }

        switch (SelectedSearchField)
        {
            case "Marka":
                selectedColumn = "brand";
                break;
            case "Model":
                selectedColumn = "model";
                break;
            case "VIN":
                selectedColumn = "VIN";
                break;
            case "Typ pojazdu":
                selectedColumn = "type";
                break;
        }

        var query = $"SELECT * FROM vehicles WHERE \"{selectedColumn}\" LIKE @searchText";
        var parameters = new List<NpgsqlParameter>();
        
        if (SelectedSearchField == "Typ pojazdu")
        {
            if (Enum.TryParse(typeof(VehicleType), SearchText, true, out var vehicleType))
            {
                parameters.Add(new NpgsqlParameter("@searchText", vehicleType.ToString()));
                query  = $"SELECT * FROM vehicles WHERE \"{selectedColumn}\" = @searchText::\"vehicletype\";";
            }
            else
            {
                AreThereVehicles = false;
                _logger.LogWarning("Nieprawidłowa wartość dla typu pojazdu!");
                return;
            }
        }
        else
        {
            parameters.Add(new NpgsqlParameter("@searchText", $"%{SearchText}%"));
        }

        var dataTable = await Task.Run(() => _vehicleRepository.ExecuteQuery(query, parameters));

        if (dataTable.Rows.Count == 0)
        {
            AreThereVehicles = false;
        }
        else
        {
            AreThereVehicles = true;

            var vehicles = dataTable.AsEnumerable().Select(row => new Vehicle
            {
                VehicleId = Guid.Parse(row["vehicleId"].ToString()),
                Brand = row["brand"].ToString(),
                Model = row["model"].ToString(),
                VIN = row["VIN"].ToString(),
                Type = Enum.TryParse(row["type"].ToString(), out VehicleType vehicleType) ? vehicleType : VehicleType.Samochód
            }).ToList();

            foreach (var vehicle in vehicles)
            {
                var images = await Task.Run(() => _imageRepository.GetImagesByVehicleId(vehicle.VehicleId));
                vehicle.Images = images.AsEnumerable().Select(row => new Image
                {
                    ImageId = Guid.Parse(row["imageId"].ToString()),
                    FileName = row["fileName"].ToString(),
                    FilePath = row["filePath"].ToString(),
                    VehicleId = vehicle.VehicleId
                }).ToList();

                vehicle.FirstImage = vehicle.Images.Count > 0 ? vehicle.Images[0] : null;
            }

            Vehicles = new ObservableCollection<Vehicle>(vehicles);
            _logger.LogInformation("Wyszukano pojazdy w bazie danych!");
        }
    }
    
    //KOMENDY
    //Komenda do pokazania szczegółów pojazdu
    [RelayCommand]
    private void ShowVehicleDetails(Vehicle vehicle)
    {
        var vehicleDetailsViewModel = new VehicleDetailsViewModel(vehicle.VehicleId, _vehicleRepository, _imageRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = vehicleDetailsViewModel;
        _logger.LogInformation("Przejście do widoku szczegółów pojazdu! ");
    }
    
    //Komenda do przejścia do widoku dodawania pojazdu
    [RelayCommand]
    private void GoToAddScreen()
    {
        var addViewModel = new VehicleAddViewModel(_vehicleRepository, _imageRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = addViewModel;
        _logger.LogInformation("Przejście do widoku dodawania pojazdu! ");
    }

    //Komenda do usuwania pojazdu z bazy danych
    [RelayCommand]
    private async void DeleteVehicle(Vehicle vehicle)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie pojazdu", "Czy na pewno chcesz usunąć ten pojazd?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                Env.Load();

                var folderPath = Env.GetString("IMAGES_FOLDER_PATH");
                var carFolderPath = $"{vehicle.Brand.ToLower()}_{vehicle.Model.ToLower()}_{vehicle.VIN}";
                var fullFolderPath = Path.Combine(folderPath, carFolderPath);

                if (Directory.Exists(folderPath))
                {
                    _imageRepository.DeleteImagesByVehicleId(vehicle.VehicleId);
                    _vehicleRepository.DeleteEquipment(vehicle.VehicleId);
                    _vehicleRepository.DeleteVehicle(vehicle.VehicleId);
                    Directory.Delete(fullFolderPath, true);
                }

                LoadVehicles();
            }
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Validation Error", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas usuwania pojazdu z bazy danych!");
        }
    }
}