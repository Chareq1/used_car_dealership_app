using System;
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
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Services;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Calendar;

[CustomInfo("Widok do dodawania spotkania", 1.0f)]
public partial class MeetingAddViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<MeetingAddViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly MeetingRepository _meetingRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly LocationRepository _locationRepository;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    //WŁAŚCIWOŚĆ DLA SPOTKANIA
    [ObservableProperty]
    private Meeting _meeting = new Meeting();
    
    //WŁAŚCIWOŚĆ DLA LISTY KLIENTÓW
    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new ObservableCollection<Customer>();

    //WŁAŚCIWOŚĆ DLA LISTY LOKALIZACJI
    [ObservableProperty]
    private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
    
    //WŁAŚCIWOŚCI DLA WYBRANEGO KLIENTA I LOKALIZACJI
    [ObservableProperty]
    private Customer _selectedCustomer;
    
    [ObservableProperty]
    private Location _selectedLocation;
       
    //WŁAŚCIWOŚCI DLA WYBRANEGO CZASU I GODZINU
    [ObservableProperty]
    private DateTimeOffset _selectedDate;
    
    [ObservableProperty]
    private TimeSpan _selectedTime;
    
    //KONSTRUKTOR
    public MeetingAddViewModel(MeetingRepository meetingRepository, CustomerRepository customerRepository, LocationRepository locationRepository, MainWindowViewModel mainWindowViewModel)
    {
        _meetingRepository = meetingRepository;
        _customerRepository = customerRepository;
        _locationRepository = locationRepository;
        _mainWindowViewModel = mainWindowViewModel;
        
        Meeting.MeetingId = Guid.NewGuid();

        var attributes = typeof(MeetingUpdateViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        SelectedDate = DateTime.Today;
        SelectedTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        
        LoadCustomers();
        LoadLocations();
    }
    
       
    // METODY
    // Metoda do wczytywania klientów
    private async void LoadCustomers()
    {
        var dataTable = await Task.Run(() => _customerRepository.GetAllCustomers());

        if (dataTable.Rows.Count == 0) { Customers = new ObservableCollection<Customer>(null); }
        else
        {
            
            var customers = dataTable.AsEnumerable().Select(row => new Customer
            {
                CustomerId = Guid.Parse(row["customerId"].ToString()),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString()
            }).ToList();
            
            Customers = new ObservableCollection<Customer>(customers);
        }
        
        _logger.LogInformation("Pobrano klientów z bazy danych!");
    }

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
    
    
    // KOMENDY
    // Komenda do powrotu do poprzedniego widoku
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new CalendarViewModel(_mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku kalendarza!");
    }

    // Komenda do dodania spotkania do bazy danych
    [RelayCommand]
    private async Task AddMeetingToDatabaseAsync()
    {
        try
        {
            Meeting.Date = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day, _selectedTime.Hours,
                _selectedTime.Minutes, _selectedTime.Seconds);

            Meeting.Customer = SelectedCustomer;
            Meeting.Location = SelectedLocation;
            
            Meeting.UserId = _mainWindowViewModel.LoggedUser.UserId;

            _meetingRepository.AddMeeting(Meeting);
            _mainWindowViewModel.CurrentPage = new CalendarViewModel(_mainWindowViewModel);
            _logger.LogInformation("Dodano spotkanie w bazie danych!");
        }
        catch (Exception ex)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Validation Error", $"Wystąpił błąd: {ex.Message}", ButtonEnum.Ok, Icon.Error);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);
            
            _logger.LogError(ex, "Błąd podczas usuwania spotkania z bazy danych!");
        }
    }
}