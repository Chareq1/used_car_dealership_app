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

//KLASA WIDOKU DO AKTUALIZACJI SPOTKANIA
[CustomInfo("Widok dodawania spotkania", 1.0f)]
public partial class MeetingUpdateViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<MeetingUpdateViewModel>();
    
    
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
    public MeetingUpdateViewModel(Guid meetingId, MeetingRepository meetingRepository, CustomerRepository customerRepository, LocationRepository locationRepository, MainWindowViewModel mainWindowViewModel)
    {
        _meetingRepository = meetingRepository;
        _customerRepository = customerRepository;
        _locationRepository = locationRepository;
        _mainWindowViewModel = mainWindowViewModel;

        var attributes = typeof(MeetingUpdateViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }

        LoadSelectedMeeting(meetingId);
        LoadCustomers();
        LoadLocations();
    }
    
    // METODY
    // Metoda do wczytywania wybranego spotkania
    private void LoadSelectedMeeting(Guid meetingId)
    {
        var meetingRow = _meetingRepository.GetMeetingById(meetingId);
        Meeting = new Meeting
        {
            MeetingId = meetingId,
            Description = meetingRow["description"].ToString(),
            Date = DateTime.Parse(meetingRow["date"].ToString()),
            UserId = Guid.Parse(meetingRow["userId"].ToString()),
            Customer = LoadSelectedCustomer(Guid.Parse(meetingRow["customerId"].ToString())),
            Location = LoadSelectedLocation(Guid.Parse(meetingRow["locationId"].ToString()))
        };
        
        SelectedCustomer = Meeting.Customer;
        SelectedLocation = Meeting.Location;
        SelectedDate = Meeting.Date;
        SelectedTime = Meeting.Date.TimeOfDay;

        _logger.LogInformation("Wczytano dane spotkania!");
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
    
    //Metoda do wczytywania wybranej lokalizacji
    private Location LoadSelectedLocation(Guid locationId)
    {
        var locationRow = _locationRepository.GetLocationById(locationId);
        _logger.LogInformation("Wczytano dane lokalizacji!");
        return new Location
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

    //Metoda do wczytywania klientów
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

    //Metoda do wczytywania lokalizacji
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

    
    //KOMENDY
    //Komenda do powrotu do poprzedniego widoku
    [RelayCommand]
    private void GoBack()
    {
        _mainWindowViewModel.CurrentPage = new CalendarViewModel(_mainWindowViewModel);
        _logger.LogInformation("Przejście do widoku kalendarza!");
    }

    //Komenda do aktualizacji spotkania w bazie danych
    [RelayCommand]
    private async Task UpdateMeetingInDatabaseAsync()
    {
        try
        {
            Meeting.Date = new DateTime(SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, SelectedTime.Hours, SelectedTime.Minutes, SelectedTime.Seconds);

            Meeting.Customer = SelectedCustomer;
            Meeting.Location = SelectedLocation;

            _meetingRepository.UpdateMeeting(Meeting);
            _mainWindowViewModel.CurrentPage = new CalendarViewModel(_mainWindowViewModel);
            _logger.LogInformation("Zaktualizowano spotkanie w bazie danych!");
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