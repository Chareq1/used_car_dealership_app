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
using used_car_dealership_app.ViewModels.Clients;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Locations;

[CustomInfo("Widok do wyświetlania danych lokalizacji", 1.0f)]
public partial class LocationDetailsViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<LocationDetailsViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly MainWindowViewModel _mainWindowViewModel;
    private readonly LocationRepository _locationRepository;
    
    
    //WŁAŚCIWOŚCI
    //Właściwość dla adresu
    [ObservableProperty]
    public String _address = "";
    
    //Właściwość dla lokalizacji
    [ObservableProperty]
    private Location _location;
    
    
    //KONSTRUKTOR
    public LocationDetailsViewModel(Guid locationId, LocationRepository repository, MainWindowViewModel mainWindowViewModel)
    {
        _locationRepository = repository;
        _mainWindowViewModel = mainWindowViewModel;
        
        var attributes = typeof(LocationDetailsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
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
        
        Address = $"ul. {Location.Street} {Location.HouseNumber}, {Location.ZipCode} {Location.City}";
        
        _logger.LogInformation("Wczytano dane lokalizacji!");
    }
       
    
    //KOMENDY
    //Komenda do powrotu do listy klientów
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new LocationsViewModel(_mainWindowViewModel);
        _logger.LogInformation("Powrót do listy lokalizacji!");
    }
    
    //Komenda do aktualizacji lokalizacji
    [RelayCommand]
    private void UpdateSelectedLocation(Location location)
    {
        var updateViewModel = new LocationUpdateViewModel(location.LocationId, _locationRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = updateViewModel;
        _logger.LogInformation("Przejście do widoku aktualizacji lokalizacji o ID {0}!", location.LocationId);
    }
    
    //Komenda do usuwania lokalizacji
    [RelayCommand]
    private async void DeleteLocation(Location location)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie lokalizacji",
                "Czy na pewno chcesz usunąć tą lokalizację?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime)
                .MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _locationRepository.DeleteLocation(location.LocationId);
                _mainWindowViewModel.CurrentPage = new LocationsViewModel(_mainWindowViewModel);
                _logger.LogInformation("Usunięto lokalizację!");
            }
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Validation Error", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas usuwania lokalizacji z bazy danych!");
        }
    }
}