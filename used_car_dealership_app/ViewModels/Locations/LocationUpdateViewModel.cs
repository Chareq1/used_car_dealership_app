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
using used_car_dealership_app.ViewModels.Clients;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Locations;

//KLASA WIDOKU DO AKTUALIZOWANIA DANYCH LOKALIZACJI
[CustomInfo("Widok do aktualizowania danych lokalizacji", 1.0f)]
public partial class LocationUpdateViewModel : ViewModelBase
{
    //POLE DLA USŁUGI NOTYFIKACJI
    private readonly NotificationService _notifications;
    
    
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<LocationUpdateViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly LocationRepository _locationRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DLA LOKALIZACJI
    [ObservableProperty]
    private Location _location = new Location();
        
    
    //KONSTRUKTOR
    public LocationUpdateViewModel(Guid locationId, LocationRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        _locationRepository = repository;
        _mainWindowViewModel = mainWindowViewModel;
        _notifications = new NotificationService(_mainWindowViewModel);
        
        var attributes = typeof(LocationUpdateViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadSelectedLocation(locationId);
    }
       
    
    //METODY
    //Metoda do wczytywania wybranej lokalizacji
    private void LoadSelectedLocation(Guid locationId)
    {
        var locationRow = _locationRepository.GetLocationById(locationId);
        Location = new Location
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
        
        _logger.LogInformation("Wczytano dane lokalizacji!");
    }
    
       
    //Metoda do walidacji pola
    private async Task ValidateInputAsync(String input, String pattern, String errorMessage)
    {
        if (!Regex.IsMatch(input, pattern))
        {
            _notifications.ShowError("Błąd walidacji", errorMessage);
            _logger.LogError(errorMessage, "Błąd walidacji pola!");
            throw new ValidationException(errorMessage);
        }
    }
    
    
    //METODY  
    //Metoda do walidacji wszystkich pól
    private async Task<bool> ValidateFieldsAsync()
    {
        try
        {
            await ValidateInputAsync(Location.Email, @"^([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x22([^\x0d\x22\x5c\x80-\xff]|\x5c[\x00-\x7f])*\x22))*\x40([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d)(\x2e([^\x00-\x20\x22\x28\x29\x2c\x2e\x3a-\x3c\x3e\x40\x5b-\x5d\x7f-\xff]+|\x5b([^\x0d\x5b-\x5d\x80-\xff]|\x5c[\x00-\x7f])*\x5d))*$", "Niepoprawny format adresu email!");
            await ValidateInputAsync(Location.Name, @"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9.\-+/@^&*()\s]+$", "Niepoprawny format nazwy!");
            await ValidateInputAsync(Location.Phone, "^[0-9]+$", "Niepoprawny format numeru telefonu!");
            await ValidateInputAsync(Location.ZipCode, "^[0-9-]{2}[-][0-9]{3}$", "Niepoprawny format kodu pocztowego!");
            await ValidateInputAsync(Location.City, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż-]+$", "Niepoprawny format miasta!");
            await ValidateInputAsync(Location.Street, @"^[\sA-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż\-\/]+$", "Niepoprawny format ulicy!");
            
            return true;
        }
        catch
        {
            return false;
        }
    }
 
    
    //KOMENDY
    //Komenda do powrotu do poprzedniego widoku
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new LocationDetailsViewModel(Location.LocationId, _locationRepository, _mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku wybranej lokalizacji!");
    }
    
    //Komenda do aktualizacji danych lokalizacji w bazie danych
    [RelayCommand]
    private async Task UpdateLocationDataInDatabaseAsync()
    {
        try
        {
            if (await ValidateFieldsAsync())
            {
                _locationRepository.UpdateLocation(Location);
                _notifications.ShowSuccess("Aktualizacja lokalizacji", "Operacja zakończona pomyślnie!");
                _mainWindowViewModel.CurrentPage = new LocationDetailsViewModel(Location.LocationId, _locationRepository, _mainWindowViewModel);
                _logger.LogInformation("Zaktualizowano lokalizację w bazie danych!");
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Problem z aktualizacją lokalizacji", ex.Message);
            _logger.LogError(ex, "Błąd podczas aktualizacji lokalizacji w bazie danych!");
        }
    }
}