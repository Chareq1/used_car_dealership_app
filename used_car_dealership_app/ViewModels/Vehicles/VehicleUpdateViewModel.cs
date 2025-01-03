using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
using used_car_dealership_app.ViewModels.Users;
using used_car_dealership_app.Views;
using Image = used_car_dealership_app.Models.Image;
using Location = used_car_dealership_app.Models.Location;

namespace used_car_dealership_app.ViewModels.Vehicles;

[CustomInfo("Widok do aktualizowania pojazdu", 1.0f)]
public partial class VehicleUpdateViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<VehicleUpdateViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly VehicleRepository _vehicleRepository;
    private readonly LocationRepository _locationRepository;
    private readonly EquipmentRepository _equipmentRepository;
    private readonly ImageRepository _imageRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    //WŁAŚCIWOŚĆ DLA POJAZDU
    [ObservableProperty]
    private Vehicle _vehicle = new Vehicle();
    
    
    //WŁAŚCIWOŚCI DLA WYBRANEJ DATY
    [ObservableProperty]
    private DateTimeOffset _selectedDate;
    
    
    //WŁAŚCIWOŚCI DLA TYPÓW POJAZDÓW
    private string _selectedEngineType;

    public string SelectedEngineType
    {
        get => _selectedEngineType;
        set
        {
            SetProperty(ref _selectedEngineType, value);
            OnPropertyChanged(nameof(IsFuelVehicle));
            OnPropertyChanged(nameof(IsHybridVehicle));
            OnPropertyChanged(nameof(IsElectricVehicle));
        }
    }

    public bool IsFuelVehicle => SelectedEngineType == "Spalinowy";
    public bool IsHybridVehicle => SelectedEngineType == "Hybrydowy";
    public bool IsElectricVehicle => SelectedEngineType == "Elektryczny";
    
    
    //WŁAŚCIWOŚCI DLA KOLEKCJI ELEMENTÓW LIST
    [ObservableProperty]
    private ObservableCollection<string> _bodyTypes;
    [ObservableProperty]
    private ObservableCollection<string> _productionCountries;
    [ObservableProperty]
    private ObservableCollection<string> _originCountries;
    [ObservableProperty]
    private ObservableCollection<string> _transmissions;
    [ObservableProperty]
    private ObservableCollection<string> _drives;
    [ObservableProperty]
    private ObservableCollection<string> _engineTypes;
    [ObservableProperty]
    private ObservableCollection<string> _fuelTypes;
    [ObservableProperty]
    private ObservableCollection<Location> _locations;
    [ObservableProperty]
    private ObservableCollection<VehicleType> _vehicleTypes;
    [ObservableProperty]
    private ObservableCollection<String> _emissionStandards;
    
    
    //WŁAŚCIWOŚCI DLA WYBRANYCH ELEMENTÓW Z LIST
    [ObservableProperty]
    private ObservableCollection<Equipment> _equipmentList;
    [ObservableProperty]
    private ObservableCollection<Equipment> _selectedEquipment;
    
    //WŁAŚCIWOŚCI DLA NOWYCH ZDJĘĆ
    [ObservableProperty] 
    public ObservableCollection<Image> _newImages;
    
    //WŁAŚCIWOŚCI DLA USUWANYCH ZDJĘĆ
    [ObservableProperty] 
    public ObservableCollection<Image> _imagesToDelete;
    
    
    //KONSTRUKTOR
    public VehicleUpdateViewModel(Guid vehicleId, VehicleRepository vehicleRepository, ImageRepository imageRepository, MainWindowViewModel mainWindowViewModel)
    {
        _vehicleRepository = vehicleRepository;
        _imageRepository = imageRepository;
        _locationRepository = new LocationRepository();
        _equipmentRepository = new EquipmentRepository();
        _equipmentList = new ObservableCollection<Equipment>();
        _selectedEquipment = new ObservableCollection<Equipment>();
        _newImages = new ObservableCollection<Image>();
        _imagesToDelete = new ObservableCollection<Image>();
        
        var attributes = typeof(VehicleUpdateViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        _mainWindowViewModel = mainWindowViewModel;
        
        VehicleTypes = new ObservableCollection<VehicleType>(Enum.GetValues(typeof(VehicleType)).Cast<VehicleType>());
        
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<VehicleUpdateViewModel>();
        
        LoadEquipment();
        LoadSelectedVehicle(vehicleId);
        LoadLocations();
        LoadComboBoxData();
    }
    
    
    //METODY
    // Metoda do wczytywania lokalizacji
    private async void LoadLocations()
    {
        var dataTable = await Task.Run(() => _locationRepository.GetAllLocations());

        if (dataTable.Rows.Count == 0) { Locations = new ObservableCollection<Location>(null); }
        else
        {
            var locations = dataTable.AsEnumerable().Select(row => new Location
            {
                LocationId = Guid.Parse(row["locationId"].ToString()),
                Name = row["name"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString(),
                City = row["city"].ToString()
            }).ToList();
            
            Locations = new ObservableCollection<Location>(locations);
        }
        
        _logger.LogInformation("Pobrano lokalizacje z bazy danych!");
    }
    
    // Metoda do wczytywania listy wyposażenia
    private async void LoadEquipment()
    {
        var dataTable = await Task.Run(() => _equipmentRepository.GetAllEquipment());

        if (dataTable.Rows.Count == 0) { EquipmentList = new ObservableCollection<Equipment>(null); }
        else
        {
            var equipment = dataTable.AsEnumerable().Select(row => new Equipment
            {
                EquipmentId = Guid.Parse(row["equipmentId"].ToString()),
                Name = row["name"].ToString()
            }).ToList();
            
            EquipmentList = new ObservableCollection<Equipment>(equipment);
        }
        
        foreach (var equipment in EquipmentList)
        {
            if (Vehicle.Equipment.Any(e => e.EquipmentId == equipment.EquipmentId))
            {
                equipment.IsSelected = true;
                SelectedEquipment.Add(equipment);
            }
        }
        
        _logger.LogInformation("Pobrano wyposażenie z bazy danych!");
    }
    
    //Metoda do załadowania lokalizacji
    private Location LoadSelectedLocation(Guid locationId)
    {
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
        Vehicle.Images = new List<Image>(imagesTable.AsEnumerable().Select(row => new Image
        {
            ImageId = Guid.Parse(row["imageId"].ToString()),
            FileName = row["fileName"].ToString(),
            FilePath = row["filePath"].ToString(),
            VehicleId = vehicleId
        }).ToList());
        
        _logger.LogInformation("Wczytano dane pojazdu!");
    }
    
    //Metoda do załadowania wyposażenia
    private List<Equipment> LoadEquipments(Guid vehicleId)
    {
        var equipmentTable = _vehicleRepository.GetEquipment(vehicleId);
        _logger.LogInformation("Wczytano wyposażenie pojazdu!");
        return equipmentTable.AsEnumerable().Select(row => new Equipment
        {
            EquipmentId = Guid.Parse(row["equipmentId"].ToString()),
            Name = row["name"].ToString()
        }).ToList();
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
        vehicle.BatterySize = string.IsNullOrEmpty(row["batterySize"].ToString())
            ? null
            : decimal.Parse(row["batterySize"].ToString());
        vehicle.ElectricEnginePower = string.IsNullOrEmpty(row["electricEnginePower"].ToString())
            ? null
            : decimal.Parse(row["electricEnginePower"].ToString());
        vehicle.Consumption = string.IsNullOrEmpty(row["consumption"].ToString())
            ? null
            : decimal.Parse(row["consumption"].ToString());
        vehicle.FuelType = row["fuelType"].ToString();
        vehicle.EngineSize = string.IsNullOrEmpty(row["engineSize"].ToString())
            ? null
            : int.Parse(row["engineSize"].ToString());
        vehicle.Power = string.IsNullOrEmpty(row["power"].ToString()) ? 0 : int.Parse(row["power"].ToString());
        vehicle.Co2Emission = string.IsNullOrEmpty(row["co2Emission"].ToString())
            ? null
            : row["co2Emission"].ToString();
        vehicle.Location = LoadSelectedLocation(Guid.Parse(row["locationId"].ToString()));
        vehicle.Equipment = LoadEquipments(vehicle.VehicleId);

        SelectedEngineType = vehicle.EngineType;

        SelectedDate = vehicle.FirstRegistrationDate;
        
        var images = _imageRepository.GetImagesByVehicleId(vehicle.VehicleId);
        vehicle.Images = images.AsEnumerable().Select(row => new Image
        {
            ImageId = Guid.Parse(row["imageId"].ToString()),
            FileName = row["fileName"].ToString(),
            FilePath = row["filePath"].ToString(),
            VehicleId = vehicle.VehicleId
        }).ToList();
        
        return vehicle;
    }
    
    //Metoda do wczytania danych do list rozwijanych
    private void LoadComboBoxData()
    {
        BodyTypes = new ObservableCollection<string> {"SUV", "Sedan", "Coupe", "Dual cowl", "Fastback", "Hatchback", "Kabriolet", "Kombi", "Kombivan", "Liftback", "Limuzyna", "Mikrovan", "Minivan", "Pickup", "Roadster", "Targa", "Van", "Trambus", "Piętrobus", "Autobus przegubowy", "Mikrobus", "Autokar", "Furgonowe", "Skrzyniowe", "Inny"};
        ProductionCountries = new ObservableCollection<string> { "Afganistan", "Albania", "Algieria", "Andora", "Angola", "Antigua i Barbuda", "Argentyna",
            "Armenia", "Australia", "Austria", "Azerbejdżan", "Bahamy", "Bahrajn", "Bangladesz",
            "Barbados", "Białoruś", "Belgia", "Belize", "Benin", "Bhutan", "Boliwia", "Bośnia i Hercegowina",
            "Botswana", "Brazylia", "Brunei", "Bułgaria", "Burkina Faso", "Burundi", "Cabo Verde", "Kambodża",
            "Kamerun", "Kanada", "Republika Środkowoafrykańska", "Czad", "Chile", "Chiny", "Kolumbia",
            "Komory", "Kongo (Brazzaville)", "Kostaryka", "Chorwacja", "Kuba", "Cypr", "Czechy",
            "Dania", "Dżibuti", "Dominika", "Dominikana", "Ekwador", "Egipt", "Salwador",
            "Gwinea Równikowa", "Erytrea", "Estonia", "Eswatini (dawniej Suazi)", "Etiopia", "Fidżi",
            "Finlandia", "Francja", "Gabon", "Gambia", "Gruzja", "Niemcy", "Ghana", "Grecja", "Grenada",
            "Gwatemala", "Gwinea", "Gwinea Bissau", "Gujana", "Haiti", "Honduras", "Węgry", "Islandia",
            "Indie", "Indonezja", "Iran", "Irak", "Irlandia", "Izrael", "Włochy", "Jamajka", "Japonia",
            "Jordania", "Kazachstan", "Kenia", "Kiribati", "Korea Południowa", "Korea Północna", "Kuwejt",
            "Kirgistan", "Laos", "Łotwa", "Liban", "Lesotho", "Liberia", "Libia", "Liechtenstein", "Litwa",
            "Luksemburg", "Madagaskar", "Malawi", "Malezja", "Malediwy", "Mali", "Malta", "Wyspy Marshalla",
            "Mauretania", "Mauritius", "Meksyk", "Mikronezja", "Mołdawia", "Monako", "Mongolia", "Czarnogóra",
            "Maroko", "Mozambik", "Myanmar (Birma)", "Namibia", "Nauru", "Nepal", "Holandia", "Nowa Zelandia",
            "Nikaragua", "Niger", "Nigeria", "Macedonia Północna", "Norwegia", "Oman", "Pakistan", "Palau",
            "Panama", "Papua-Nowa Gwinea", "Paragwaj", "Peru", "Filipiny", "Polska", "Portugalia", "Katar",
            "Rumunia", "Rosja", "Rwanda", "Saint Kitts i Nevis", "Saint Lucia", "Saint Vincent i Grenadyny",
            "Samoa", "San Marino", "Sao Tome i Principe", "Arabia Saudyjska", "Senegal", "Serbia", "Seszele",
            "Sierra Leone", "Singapur", "Słowacja", "Słowenia", "Wyspy Salomona", "Somalia", "RPA", "Sudan Południowy",
            "Hiszpania", "Sri Lanka", "Sudan", "Surinam", "Szwecja", "Szwajcaria", "Syria", "Tajwan", "Tadżykistan",
            "Tanzania", "Tajlandia", "Timor Wschodni", "Togo", "Tonga", "Trynidad i Tobago", "Tunezja", "Turcja",
            "Turkmenistan", "Tuvalu", "Uganda", "Ukraina", "Zjednoczone Emiraty Arabskie", "Wielka Brytania",
            "USA", "Urugwaj", "Uzbekistan", "Vanuatu", "Watykan", "Wenezuela", "Wietnam", "Jemen", "Zambia", "Zimbabwe" };
        OriginCountries = new ObservableCollection<string> { "Afganistan", "Albania", "Algieria", "Andora", "Angola", "Antigua i Barbuda", "Argentyna",
            "Armenia", "Australia", "Austria", "Azerbejdżan", "Bahamy", "Bahrajn", "Bangladesz",
            "Barbados", "Białoruś", "Belgia", "Belize", "Benin", "Bhutan", "Boliwia", "Bośnia i Hercegowina",
            "Botswana", "Brazylia", "Brunei", "Bułgaria", "Burkina Faso", "Burundi", "Cabo Verde", "Kambodża",
            "Kamerun", "Kanada", "Republika Środkowoafrykańska", "Czad", "Chile", "Chiny", "Kolumbia",
            "Komory", "Kongo (Brazzaville)", "Kostaryka", "Chorwacja", "Kuba", "Cypr", "Czechy",
            "Dania", "Dżibuti", "Dominika", "Dominikana", "Ekwador", "Egipt", "Salwador",
            "Gwinea Równikowa", "Erytrea", "Estonia", "Eswatini (dawniej Suazi)", "Etiopia", "Fidżi",
            "Finlandia", "Francja", "Gabon", "Gambia", "Gruzja", "Niemcy", "Ghana", "Grecja", "Grenada",
            "Gwatemala", "Gwinea", "Gwinea Bissau", "Gujana", "Haiti", "Honduras", "Węgry", "Islandia",
            "Indie", "Indonezja", "Iran", "Irak", "Irlandia", "Izrael", "Włochy", "Jamajka", "Japonia",
            "Jordania", "Kazachstan", "Kenia", "Kiribati", "Korea Południowa", "Korea Północna", "Kuwejt",
            "Kirgistan", "Laos", "Łotwa", "Liban", "Lesotho", "Liberia", "Libia", "Liechtenstein", "Litwa",
            "Luksemburg", "Madagaskar", "Malawi", "Malezja", "Malediwy", "Mali", "Malta", "Wyspy Marshalla",
            "Mauretania", "Mauritius", "Meksyk", "Mikronezja", "Mołdawia", "Monako", "Mongolia", "Czarnogóra",
            "Maroko", "Mozambik", "Myanmar (Birma)", "Namibia", "Nauru", "Nepal", "Holandia", "Nowa Zelandia",
            "Nikaragua", "Niger", "Nigeria", "Macedonia Północna", "Norwegia", "Oman", "Pakistan", "Palau",
            "Panama", "Papua-Nowa Gwinea", "Paragwaj", "Peru", "Filipiny", "Polska", "Portugalia", "Katar",
            "Rumunia", "Rosja", "Rwanda", "Saint Kitts i Nevis", "Saint Lucia", "Saint Vincent i Grenadyny",
            "Samoa", "San Marino", "Sao Tome i Principe", "Arabia Saudyjska", "Senegal", "Serbia", "Seszele",
            "Sierra Leone", "Singapur", "Słowacja", "Słowenia", "Wyspy Salomona", "Somalia", "RPA", "Sudan Południowy",
            "Hiszpania", "Sri Lanka", "Sudan", "Surinam", "Szwecja", "Szwajcaria", "Syria", "Tajwan", "Tadżykistan",
            "Tanzania", "Tajlandia", "Timor Wschodni", "Togo", "Tonga", "Trynidad i Tobago", "Tunezja", "Turcja",
            "Turkmenistan", "Tuvalu", "Uganda", "Ukraina", "Zjednoczone Emiraty Arabskie", "Wielka Brytania",
            "USA", "Urugwaj", "Uzbekistan", "Vanuatu", "Watykan", "Wenezuela", "Wietnam", "Jemen", "Zambia", "Zimbabwe" };
        Transmissions = new ObservableCollection<string> { "Manualna", "Automatyczna" };
        Drives = new ObservableCollection<string> { "FWD", "RWD", "AWD", "4WD" };
        EngineTypes = new ObservableCollection<string> { "Spalinowy", "Elektryczny", "Hybrydowy" };
        FuelTypes = new ObservableCollection<string> { "Benzyna", "Benzyna+CNG", "Benzyna+LPG", "Diesel", "Etanol" };
        EmissionStandards = new ObservableCollection<String> { "EURO1", "EURO2", "EURO3", "EURO4", "EURO5", "EURO6" };
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
            await ValidateInputAsync(Vehicle.Brand, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9\-\s\.]+$", "Niepoprawny format nazwy marki!");
            await ValidateInputAsync(Vehicle.Model, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9\-\s\.]+$", "Niepoprawny format nazwy modelu!");
            await ValidateInputAsync(Vehicle.ProductionYear.ToString(), @"^[0-9]{4}$", "Niepoprawny format roku produkcji!");
            await ValidateInputAsync(Vehicle.Mileage.ToString(), "^[0-9]{1,7}$", "Niepoprawny format przebiegu!");
            await ValidateInputAsync(Vehicle.Doors.ToString(), "^[0-9]{1,2}$", "Niepoprawny format liczby drzwi!");
            await ValidateInputAsync(Vehicle.Color, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9\-\s]+$", "Niepoprawny format nazwy koloru!");
            await ValidateInputAsync(Vehicle.VIN, @"^[(A-H|J-N|P|R-Z|0-9)]{17}$", "Niepoprawny format numeru VIN!");
            
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
    //Komenda do zaznaczania i odznaczania wyposażenia
    [RelayCommand]
    private void SelectAndUnselectEquipment(Equipment equipment)
    {
        if (!_selectedEquipment.Contains(equipment))
        {
            _selectedEquipment.Add(equipment);
        }
        else
        {
            _selectedEquipment.Remove(equipment);
        }
    }
    
    //Komenda do usuwania zdjęcia
    [RelayCommand]
    private void DeleteImage(Image image)
    {
        if (!NewImages.Remove(image))
        {
            ImagesToDelete.Add(image);
            Vehicle.Images.Remove(image);
        }
    }
    
    //Komenda do powrotu do poprzedniego widoku
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new VehicleDetailsViewModel(Vehicle.VehicleId, _vehicleRepository, _imageRepository, _mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku wybranego pojazdu!");
    }
    
    //Komenda do dodania nowego zdjęcia
    [RelayCommand]
    private async Task UploadImageAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "Images", Extensions = new List<string> { "jpg", "jpeg", "png", "bmp" } }
            }
        };
        var mainWin = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
        var result = await openFileDialog.ShowAsync(mainWin);
        
        if (result != null && result.Length > 0)
        {
            var filePath = result[0];
            var fileName = Path.GetFileName(filePath);
            
            var image = new Image
            {
                ImageId = Guid.NewGuid(),
                FileName = fileName,
                FilePath = filePath,
                VehicleId = Vehicle.VehicleId
            };

            NewImages.Add(image);
        }
    }
    
    //Komenda do aktualizacji danych pojazdu w bazie danych
    [RelayCommand]
    private async Task UpdateVehicleDataInDatabaseAsync()
    {
        try
        {
            if (await ValidateFieldsAsync())
            {
                var newImagesCopy = new List<Image>(_newImages);
                var equipmentCopy = new List<Equipment>(_selectedEquipment);
                var imagesToDeleteCopy = new List<Image>(_imagesToDelete);

                if(newImagesCopy.Count == 0 && Vehicle.Images.Count == 0)
                {
                    await ShowPopupAsync("Dodaj przynajmniej 1 zdjęcie pojazdu!");
                    _logger.LogError("Dodaj przynajmniej 1 zdjęcie pojazdu!");
                    throw new ValidationException("Dodaj przynajmniej 1 zdjęcie pojazdu!");
                }
                
                Vehicle.FirstRegistrationDate = SelectedDate.DateTime;
                Vehicle.EngineType = SelectedEngineType;
                
                _vehicleRepository.UpdateVehicle(Vehicle);
                
                var carFolderName = Vehicle.Brand.ToLower() + "_" + Vehicle.Model.ToLower() + "_" + Vehicle.VIN ;

                if (Vehicle.EngineType == "Spalinowy")
                {
                    Vehicle.BatterySize = null;
                    Vehicle.ElectricEnginePower = null;
                }
                else if (Vehicle.EngineType == "Elektryczny")
                {
                    Vehicle.FuelType = null;
                    Vehicle.EngineSize = null;
                    Vehicle.Power = null;
                    Vehicle.Co2Emission = null;
                }
                else if (Vehicle.EngineType == "Hybrydowy")
                {
                    Vehicle.FuelType = null;
                }

                foreach (var image in newImagesCopy)
                {
                    Env.Load();
                    var destinationPath = Path.Combine(Env.GetString("IMAGES_FOLDER_PATH"), carFolderName, image.FileName);
                    
                    File.Copy(image.FilePath, destinationPath, true);

                    image.FilePath = Path.Combine(Env.GetString("IMAGES_FOLDER_PATH"), carFolderName + "/");
                }

                foreach (var image in newImagesCopy)
                {
                    _imageRepository.AddImage(image);
                }

                _vehicleRepository.DeleteEquipment(Vehicle.VehicleId);

                foreach (var equipment in equipmentCopy)
                {
                    _vehicleRepository.AddEquipment(Vehicle.VehicleId, equipment.EquipmentId);
                }

                foreach (var image in imagesToDeleteCopy)
                {
                    var filePath = Path.Combine(image.FilePath, image.FileName);
                    
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    
                    _imageRepository.DeleteImage(image.ImageId);
                }

                _mainWindowViewModel.CurrentPage = new VehicleDetailsViewModel(Vehicle.VehicleId, _vehicleRepository,
                    _imageRepository, _mainWindowViewModel);
                _logger.LogInformation("Zaktualizowano pojazd w bazie danych!");

            }
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Błąd z aktualizacją pojazdu", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas aktualizacji pojazdu w bazie danych!");
        }
    }
}