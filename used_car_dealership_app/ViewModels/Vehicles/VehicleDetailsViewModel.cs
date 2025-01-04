using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using used_car_dealership_app.ViewModels.Locations;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Vehicles;

//KLASA WIDOKU DO WYŚWIETLANIA DANYCH POJAZDU
[CustomInfo("Widok do wyświetlania danych pojazdu", 1.0f)]
public partial class VehicleDetailsViewModel : ViewModelBase
{
    //POLE DLA USŁUGI NOTYFIKACJI
    private readonly NotificationService _notifications;
    
    
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<VehicleDetailsViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly VehicleRepository _vehicleRepository;
    private readonly ImageRepository _imageRepository;
    private readonly LocationRepository _locationRepository;

    
    //WŁAŚCIWOŚCI DLA POJAZDU
    [ObservableProperty]
    private Vehicle _vehicle;
    
    
    //WŁAŚCIWOŚCI DLA OBRAZÓW
    [ObservableProperty]
    private ObservableCollection<Image> _images;

    [ObservableProperty]
    private bool _areThereImages;
    
    
    //WŁAŚCIWOŚĆ DLA WYBRANEGO OBRAZU
    [ObservableProperty]
    private int _selectedImageIndex;
    
    
    //WŁAŚCIWOŚĆ DLA LISTY WYPOSAŻENIA
    [ObservableProperty] 
    private String _equipmentListString;
    
    
    //WŁAŚCIWOŚCI DLA TYPÓW POJAZDÓW
    [ObservableProperty]
    private bool _isFuelVehicle;
    
    [ObservableProperty]
    private bool _isElectricVehicle;
    
    [ObservableProperty]
    private bool _isHybridVehicle;

    
    //KONSTRUKTOR
    public VehicleDetailsViewModel(Guid vehicleId, VehicleRepository vehicleRepository, ImageRepository imageRepository, MainWindowViewModel mainWindowViewModel)
    {
        _vehicleRepository = vehicleRepository;
        _imageRepository = imageRepository;
        _locationRepository = new LocationRepository();
        _mainWindowViewModel = mainWindowViewModel;
        _notifications = new NotificationService(_mainWindowViewModel);
        
        var attributes = typeof(VehicleDetailsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadSelectedVehicle(vehicleId);
    }
    
    
    //METODY
    //Metoda do załadowania lokalizacji
    private Location LoadSelectedLocation(Guid locationId)
    {
        Console.WriteLine("Test");
        var locationRow = _locationRepository.GetLocationById(locationId);
        _logger.LogInformation("Wczytano dane lokalizacji!");
        return new Location()
        {
            LocationId = locationId,
            Name = locationRow["name"].ToString(),
            Email = locationRow["email"].ToString(),
            Phone = locationRow["phone"].ToString(),
            Street = locationRow["street"].ToString(),
            City = locationRow["city"].ToString(),
            ZipCode = locationRow["zipCode"].ToString(),
            HouseNumber = locationRow["houseNumber"].ToString()
        };
    }
    
    //Metoda do wczytywania danych pojazdu
    private void LoadSelectedVehicle(Guid vehicleId)
    {
        var vehicleRow = _vehicleRepository.GetVehicleById(vehicleId);
        Vehicle = CreateVehicleFromDataRow(vehicleRow);
        
        var imagesTable = _imageRepository.GetImagesByVehicleId(vehicleId);
        Images = new ObservableCollection<Image>(imagesTable.AsEnumerable().Select(row => new Image
        {
            ImageId = Guid.Parse(row["imageId"].ToString()),
            FileName = row["fileName"].ToString(),
            FilePath = row["filePath"].ToString(),
            VehicleId = vehicleId
        }).ToList());
        
        _logger.LogInformation("Wczytano dane pojazdu!");
    }
    
    //Metoda do tworzenia obiektu pojazdu na podstawie wiersza z bazy danych
    private Vehicle CreateVehicleFromDataRow(DataRow row)
    {
        Vehicle vehicle = new Vehicle();
        
        vehicle.VehicleId = Guid.Parse(row["vehicleId"].ToString());
        vehicle.Brand = row["brand"].ToString();
        vehicle.Model = row["model"].ToString();
        vehicle.Type = Enum.TryParse(row["type"].ToString(), out VehicleType vehicleType) ? vehicleType : VehicleType.Samochód;
        vehicle.BodyType = row["bodyType"].ToString();
        vehicle.ProductionYear = int.Parse(row["productionYear"].ToString());
        vehicle.ProductionCountry = row["productionCountry"].ToString();
        vehicle.FirstRegistrationDate = DateTime.Parse(row["firstRegistrationDate"].ToString());
        vehicle.OriginCountry = row["originCountry"].ToString();
        vehicle.Mileage = int.Parse(row["mileage"].ToString());
        vehicle.Doors = int.Parse(row["doors"].ToString());
        vehicle.Color = row["color"].ToString();
        vehicle.Transmission = row["transmission"].ToString();
        vehicle.VIN = row["VIN"].ToString();
        vehicle.Description = row["description"].ToString();
        vehicle.Drive = row["drive"].ToString();
        vehicle.Price = decimal.Parse(row["price"].ToString());
        vehicle.EngineType = row["engineType"].ToString();
        vehicle.BatterySize = String.IsNullOrEmpty(row["batterySize"].ToString())
            ? null
            : decimal.Parse(row["batterySize"].ToString());
        vehicle.ElectricEnginePower = String.IsNullOrEmpty(row["electricEnginePower"].ToString())
            ? null
            : decimal.Parse(row["electricEnginePower"].ToString());
        vehicle.Consumption = String.IsNullOrEmpty(row["consumption"].ToString())
            ? null
            : decimal.Parse(row["consumption"].ToString());
        vehicle.FuelType = String.IsNullOrEmpty(row["fuelType"].ToString())
            ? null
            : row["fuelType"].ToString();
        vehicle.EngineSize = String.IsNullOrEmpty(row["engineSize"].ToString())
            ? null
            : int.Parse(row["engineSize"].ToString());
        vehicle.Power = String.IsNullOrEmpty(row["power"].ToString()) ? 0 : int.Parse(row["power"].ToString());
        vehicle.Co2Emission = String.IsNullOrEmpty(row["co2Emission"].ToString())
            ? null
            : row["co2Emission"].ToString();
        vehicle.Location = LoadSelectedLocation(Guid.Parse(row["locationId"].ToString()));;
        vehicle.Equipment = LoadEquipment(vehicle.VehicleId);
        
        if(vehicle.EngineType == "Spalinowy")
        {
            IsFuelVehicle = true;
            IsElectricVehicle = false;
            IsHybridVehicle = false;
        }
        else if(vehicle.EngineType == "Elektryczny")
        {
            IsElectricVehicle = true;
            IsFuelVehicle = false;
            IsHybridVehicle = false;
        }
        else if(vehicle.EngineType == "Hybrydowy")
        {
            IsHybridVehicle = true;
            IsFuelVehicle = false;
            IsElectricVehicle = false;
        }
        
        foreach (var equipment in vehicle.Equipment)
        {
            EquipmentListString += $"{equipment.Name}, ";
        }
        
        var images = _imageRepository.GetImagesByVehicleId(vehicle.VehicleId);
        vehicle.Images = images.AsEnumerable().Select(row => new Image
        {
            ImageId = Guid.Parse(row["imageId"].ToString()),
            FileName = row["fileName"].ToString(),
            FilePath = row["filePath"].ToString(),
            VehicleId = vehicle.VehicleId
        }).ToList();

        if(vehicle.Images.Count > 0) { AreThereImages = true; } else { AreThereImages = false;}
        
        return vehicle;
    }
    
    //Metoda do załadowania wyposażenia
    private List<Equipment> LoadEquipment(Guid vehicleId)
    {
        var equipmentTable = _vehicleRepository.GetEquipment(vehicleId);
        _logger.LogInformation("Wczytano wyposażenie pojazdu!");
        return equipmentTable.AsEnumerable().Select(row => new Equipment
        {
            EquipmentId = Guid.Parse(row["equipmentId"].ToString()),
            Name = row["name"].ToString()
        }).ToList();
    }
    
    
    //KOMENDY
    //Komenda do powrotu do listy pojazdów
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new VehiclesViewModel(_mainWindowViewModel);
        _logger.LogInformation("Powrót do listy lokalizacji!");
    }

    //Komenda do aktualizacji pojazdu
    [RelayCommand]
    private void UpdateSelectedVehicle(Vehicle vehicle)
    {
        var updateViewModel = new VehicleUpdateViewModel(vehicle.VehicleId, _vehicleRepository, _imageRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = updateViewModel;
        _logger.LogInformation("Przejście do widoku aktualizacji pojazdu o ID {0}!", vehicle.VehicleId);
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

                    _vehicleRepository.DeleteVehicle(Vehicle.VehicleId);
                    _notifications.ShowSuccess("Usuwanie pojazdu", "Operacja zakończona pomyślnie!");
                    _mainWindowViewModel.CurrentPage = new VehiclesViewModel(_mainWindowViewModel);
                    _logger.LogInformation("Usunięto pojazd!");
                }
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Problem z usunięciem pojazdu", ex.Message);
            _logger.LogError(ex, "Błąd podczas usuwania lokalizacji z bazy danych!");
        }
    }
}