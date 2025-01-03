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
using used_car_dealership_app.ViewModels.Clients;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Locations;

//KLASA WIDOKU DO WYŚWIETLANIA LISTY LOKALIZACJI
[CustomInfo("Widok listy lokalizacji", 1.0f)]
public partial class LocationsViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<LocationsViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly LocationRepository _locationRepository;
    private ObservableCollection<Location> _locations;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DO SPRAWDZANIA CZY SĄ LOKALIZACJE
    [ObservableProperty]
    private bool _areThereLocations = false;
    
    
    //WŁAŚCIWOŚCI DO WYSZUKIWANA LOKALIZACJI
    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private string _selectedSearchField;

    [ObservableProperty] 
    private List<string> _searchFields;
    
    
    //POLE DLA LISTY LOKALIZACJI Z BAZY DANYCH
    public ObservableCollection<Location> Locations
    {
        get => _locations;
        set => SetProperty(ref _locations, value);
    }
    
    
    //KONSTRUKTOR DLA WIDOKU
    public LocationsViewModel()
    {
        _locationRepository = new LocationRepository();
        _searchFields = new List<string> { "Nazwa", "Email", "Telefon", "Miejscowość"};
        
        var attributes = typeof(ClientsViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadLocations();
    }
    
    
    //KONSTRUKTOR DLA WIDOKU Z MAINWINDOWVIEWMODEL
    public LocationsViewModel(MainWindowViewModel mainWindowViewModel) : this()
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
    
    
    //METODY
    //Metoda do wczytywania lokalizacji z bazy danych
    private async Task LoadLocations()
    {
        var dataTable = await Task.Run(() => _locationRepository.GetAllLocations());

        if (dataTable.Rows.Count == 0) { AreThereLocations = false; }
        else
        {
            AreThereLocations = true;
            
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
    
    //Metoda do wyszukiwania lokalizacji w bazie danych
    [RelayCommand]
    private async Task SearchLocationsAsync()
    {
        var selectedColumn = "";
            
        if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(SelectedSearchField))
        {
            await LoadLocations();
            return;
        }

        switch (SelectedSearchField)
        {
            case "Nazwa":
                selectedColumn = "name";
                break;
            case "Telefon":
                selectedColumn = "phone";
                break;
            case "Email":
                selectedColumn = "email";
                break;
            case "Miejscowość":
                selectedColumn = "city";
                break;
        }

        var query = $"SELECT * FROM locations WHERE \"{selectedColumn}\" LIKE @searchText";
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter("@searchText", $"%{SearchText}%")
        };

        var dataTable = await Task.Run(() => _locationRepository.ExecuteQuery(query, parameters));

        if (dataTable.Rows.Count == 0)
        {
            AreThereLocations = false;
        }
        else
        {
            AreThereLocations = true;

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

        _logger.LogInformation("Wyszukano lokalizacje w bazie danych!");
    }
    
    //Metoda do pokazania szczegółów klienta (przejścia do innego widoku)
    [RelayCommand]
    private void ShowLocationDetails(Location location)
    {
        var detailsViewModel = new LocationDetailsViewModel(location.LocationId, _locationRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = detailsViewModel;
        _logger.LogInformation("Przejście do widoku szczegółów lokalizacji o ID {0}!", location.LocationId);
    }
    
    //Metoda do przejścia do widoku dodawania lokalizacji
    [RelayCommand]
    private void GoToAddScreen()
    {
        var addViewModel = new LocationAddViewModel(_locationRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = addViewModel;
        _logger.LogInformation("Przejście do widoku dodawania lokalizacji! ");
    }
    
    //Metoda do usuwania lokalizacji z bazy danych
    [RelayCommand]
    private async void DeleteLocation(Location location)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie lokalizacji", "Czy na pewno chcesz usunąć tą lokalizację?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _locationRepository.DeleteLocation(location.LocationId);
                LoadLocations();
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