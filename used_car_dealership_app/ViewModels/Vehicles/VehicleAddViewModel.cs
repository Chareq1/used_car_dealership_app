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
using used_car_dealership_app.Views;
using Image = used_car_dealership_app.Models.Image;
using Location = used_car_dealership_app.Models.Location;

namespace used_car_dealership_app.ViewModels.Vehicles;

[CustomInfo("Widok do dodawania pojazdu", 1.0f)]
public partial class VehicleAddViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<VehicleAddViewModel>();
    
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
    
    
    //KONSTRUKTOR
    public VehicleAddViewModel(VehicleRepository vehicleRepository, ImageRepository imageRepository, MainWindowViewModel mainWindowViewModel)
    {
        _vehicleRepository = vehicleRepository;
        _imageRepository = imageRepository;
        _locationRepository = new LocationRepository();
        _equipmentRepository = new EquipmentRepository();
        _equipmentList = new ObservableCollection<Equipment>();
        _selectedEquipment = new ObservableCollection<Equipment>();
        _newImages = new ObservableCollection<Image>();
        
        var attributes = typeof(VehicleAddViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        _mainWindowViewModel = mainWindowViewModel;
        
        SelectedDate = DateTimeOffset.Now;

        Vehicle.VehicleId = Guid.NewGuid();
        
        VehicleTypes = new ObservableCollection<VehicleType>(Enum.GetValues(typeof(VehicleType)).Cast<VehicleType>());
        
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<VehicleUpdateViewModel>();
        
        LoadEquipment();
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
        
        _logger.LogInformation("Pobrano wyposażenie z bazy danych!");
    }
    
        //Metoda do wczytania danych do list rozwijanych
    private void LoadComboBoxData()
    {
        BodyTypes = new ObservableCollection<string> {"SUV", "Sedan", "Coupe", "Dual cowl", "Fastback", "Hatchback", "Kabriolet", "Kombi", "Kombivan", "Liftback", "Limuzyna", "Mikrovan", "Minivan", "Pickup", "Roadster", "Targa", "Van", "Trambus", "Piętrobus", "Autobus przegubowy", "Mikrobus", "Autokar", "Furgonowe", "Skrzyniowe", "Inny"};
        ProductionCountries = new ObservableCollection<string> { "Afganistan", "Albania", "Algieria", "Andora", "Angola", "Antigua i Barbuda", "Argentyna",
            "Armenia", "Australia", "Austria", "Azerbejdżan", "Bahamy", "Bahrajn", "Bangladesz",
            "Barbados", "Belgia", "Belize", "Benin", "Bhutan", "Białoruś", "Boliwia", "Bośnia i Hercegowina",
            "Botswana", "Brazylia", "Brunei", "Bułgaria", "Burkina Faso", "Burundi", "Chile", "Chiny",
            "Chorwacja", "Cypr", "Czechy", "Czad", "Dania", "Dominika", "Dominikana", "Dżibuti",
            "Egipt", "Ekwador", "Erytrea", "Estonia", "Eswatini (dawniej Suazi)", "Etiopia",
            "Fidżi", "Filipiny", "Finlandia", "Francja", "Gabon", "Gambia", "Ghana", "Grecja",
            "Grenada", "Gruzja", "Gwinea", "Gwinea Bissau", "Gwinea Równikowa", "Gwatemala",
            "Gujana", "Haiti", "Hiszpania", "Holandia", "Honduras", "Indie", "Indonezja", "Irak",
            "Iran", "Irlandia", "Islandia", "Izrael", "Jamajka", "Japonia", "Jemen", "Jordania",
            "Kambodża", "Kamerun", "Kanada", "Katar", "Kazachstan", "Kenia", "Kirgistan", "Kiribati",
            "Kolumbia", "Komory", "Kongo (Brazzaville)", "Kostaryka", "Kuba", "Kuwejt", "Laos",
            "Lesotho", "Liban", "Liberia", "Libia", "Liechtenstein", "Litwa", "Luksemburg", "Łotwa",
            "Madagaskar", "Malawi", "Malediwy", "Malezja", "Mali", "Malta", "Maroko", "Mauretania",
            "Mauritius", "Meksyk", "Mikronezja", "Mjanma (Birma)", "Mołdawia", "Monako", "Mongolia",
            "Mozambik", "Namibia", "Nauru", "Nepal", "Niemcy", "Niger", "Nigeria", "Nikaragua",
            "Norwegia", "Nowa Zelandia", "Oman", "Pakistan", "Palau", "Panama", "Papua-Nowa Gwinea",
            "Paragwaj", "Peru", "Polska", "Portugalia", "Republika Środkowoafrykańska", "Rosja",
            "Rumunia", "Rwanda", "Saint Kitts i Nevis", "Saint Lucia", "Saint Vincent i Grenadyny",
            "Salwador", "Samoa", "San Marino", "Sao Tome i Principe", "Senegal", "Serbia", "Seszele",
            "Sierra Leone", "Singapur", "Słowacja", "Słowenia", "Somalia", "Sri Lanka", "Sudan",
            "Sudan Południowy", "Surinam", "Szwajcaria", "Szwecja", "Syria", "Tadżykistan", "Tajlandia",
            "Tajwan", "Tanzania", "Timor Wschodni", "Togo", "Tonga", "Trynidad i Tobago", "Tunezja",
            "Turcja", "Turkmenistan", "Tuvalu", "Uganda", "Ukraina", "Urugwaj", "USA", "Uzbekistan",
            "Vanuatu", "Watykan", "Wenezuela", "Węgry", "Wielka Brytania", "Wietnam", "Włochy",
            "Wyspy Marshalla", "Wyspy Salomona", "Zambia", "Zimbabwe", "Zjednoczone Emiraty Arabskie" };
        OriginCountries = new ObservableCollection<string> { "Afganistan", "Albania", "Algieria", "Andora", "Angola", "Antigua i Barbuda", "Argentyna",
            "Armenia", "Australia", "Austria", "Azerbejdżan", "Bahamy", "Bahrajn", "Bangladesz",
            "Barbados", "Belgia", "Belize", "Benin", "Bhutan", "Białoruś", "Boliwia", "Bośnia i Hercegowina",
            "Botswana", "Brazylia", "Brunei", "Bułgaria", "Burkina Faso", "Burundi", "Chile", "Chiny",
            "Chorwacja", "Cypr", "Czechy", "Czad", "Dania", "Dominika", "Dominikana", "Dżibuti",
            "Egipt", "Ekwador", "Erytrea", "Estonia", "Eswatini (dawniej Suazi)", "Etiopia",
            "Fidżi", "Filipiny", "Finlandia", "Francja", "Gabon", "Gambia", "Ghana", "Grecja",
            "Grenada", "Gruzja", "Gwinea", "Gwinea Bissau", "Gwinea Równikowa", "Gwatemala",
            "Gujana", "Haiti", "Hiszpania", "Holandia", "Honduras", "Indie", "Indonezja", "Irak",
            "Iran", "Irlandia", "Islandia", "Izrael", "Jamajka", "Japonia", "Jemen", "Jordania",
            "Kambodża", "Kamerun", "Kanada", "Katar", "Kazachstan", "Kenia", "Kirgistan", "Kiribati",
            "Kolumbia", "Komory", "Kongo (Brazzaville)", "Kostaryka", "Kuba", "Kuwejt", "Laos",
            "Lesotho", "Liban", "Liberia", "Libia", "Liechtenstein", "Litwa", "Luksemburg", "Łotwa",
            "Madagaskar", "Malawi", "Malediwy", "Malezja", "Mali", "Malta", "Maroko", "Mauretania",
            "Mauritius", "Meksyk", "Mikronezja", "Mjanma (Birma)", "Mołdawia", "Monako", "Mongolia",
            "Mozambik", "Namibia", "Nauru", "Nepal", "Niemcy", "Niger", "Nigeria", "Nikaragua",
            "Norwegia", "Nowa Zelandia", "Oman", "Pakistan", "Palau", "Panama", "Papua-Nowa Gwinea",
            "Paragwaj", "Peru", "Polska", "Portugalia", "Republika Środkowoafrykańska", "Rosja",
            "Rumunia", "Rwanda", "Saint Kitts i Nevis", "Saint Lucia", "Saint Vincent i Grenadyny",
            "Salwador", "Samoa", "San Marino", "Sao Tome i Principe", "Senegal", "Serbia", "Seszele",
            "Sierra Leone", "Singapur", "Słowacja", "Słowenia", "Somalia", "Sri Lanka", "Sudan",
            "Sudan Południowy", "Surinam", "Szwajcaria", "Szwecja", "Syria", "Tadżykistan", "Tajlandia",
            "Tajwan", "Tanzania", "Timor Wschodni", "Togo", "Tonga", "Trynidad i Tobago", "Tunezja",
            "Turcja", "Turkmenistan", "Tuvalu", "Uganda", "Ukraina", "Urugwaj", "USA", "Uzbekistan",
            "Vanuatu", "Watykan", "Wenezuela", "Węgry", "Wielka Brytania", "Wietnam", "Włochy",
            "Wyspy Marshalla", "Wyspy Salomona", "Zambia", "Zimbabwe", "Zjednoczone Emiraty Arabskie" };
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
        NewImages.Remove(image);
    }
    
    //Komenda do powrotu do poprzedniego widoku
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new VehiclesViewModel(_mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku pojazdów!");
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
    
    //Komenda do dodwania pojazdu do bazy danych
    [RelayCommand]
    private async Task AddVehicleToDatabaseAsync()
    {
        try
        {
            if (await ValidateFieldsAsync())
            {
                var newImagesCopy = new List<Image>(NewImages);
                var equipmentCopy = new List<Equipment>(SelectedEquipment);
                var carFolderName = Vehicle.Brand.ToLower() + "_" + Vehicle.Model.ToLower() + "_" + Vehicle.VIN ;
                
                if(newImagesCopy.Count == 0)
                {
                    await ShowPopupAsync("Dodaj przynajmniej 1 zdjęcie pojazdu!");
                    _logger.LogError("Dodaj przynajmniej 1 zdjęcie pojazdu!");
                    throw new ValidationException("Dodaj przynajmniej 1 zdjęcie pojazdu!");
                }
                
                Vehicle.FirstRegistrationDate = SelectedDate.DateTime;
                Vehicle.EngineType = SelectedEngineType;
                _vehicleRepository.AddVehicle(Vehicle);

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
                    
                    Directory.CreateDirectory(Path.Combine(Env.GetString("IMAGES_FOLDER_PATH"), carFolderName));
                    File.Copy(image.FilePath, destinationPath, true);

                    image.FilePath = Path.Combine(Env.GetString("IMAGES_FOLDER_PATH"), carFolderName + "/");
                }

                foreach (var image in newImagesCopy)
                {
                    _imageRepository.AddImage(image);
                }

                foreach (var equipment in equipmentCopy)
                {
                    _vehicleRepository.AddEquipment(Vehicle.VehicleId, equipment.EquipmentId);
                }
                
                _mainWindowViewModel.CurrentPage = new VehiclesViewModel(_mainWindowViewModel);
                _logger.LogInformation("Dodano pojazd do bazy danych!");

            }
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Błąd z dodawaniem pojazdu", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas dodawania pojazdu do bazy danych!");
        }
    }
}