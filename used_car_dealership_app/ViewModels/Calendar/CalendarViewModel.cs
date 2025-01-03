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

[CustomInfo("Widok kalendarza", 1.0f)]
public partial class CalendarViewModel : ViewModelBase
{
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<CalendarViewModel>();
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly MeetingRepository _meetingRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly LocationRepository _locationRepository;
    private ObservableCollection<Meeting> _meetings;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    //WŁAŚCIWOŚĆ DO SPRAWDZANIA CZY SĄ SPOTKANIA
    [ObservableProperty]
    private bool _areThereMeetings;
    
    //WŁAŚCIWOŚĆ DLA WYBRANEJ DATY
    [ObservableProperty]
    private DateTime _selectedDate;
    
    //WŁAŚCIWOŚĆ DLA KLIENTA
    [ObservableProperty]
    private Customer _selectedCustomer;
    
    //WŁAŚCIWOŚĆ DLA UŻYTKOWNIKA
    [ObservableProperty]
    private User _selectedUser;
    
    
    //KONSTRUKTOR DLA WIDOKU
    public CalendarViewModel()
    {
        _meetingRepository = new MeetingRepository();
        _locationRepository = new LocationRepository();
        _customerRepository = new CustomerRepository();
        
        SelectedDate = DateTime.Today;

        var attributes = typeof(CalendarViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }

        LoadMeetings();
    }
    
    
    //KONSTRUKTOR DLA WIDOKU Z MAINWINDOWVIEWMODEL
    public CalendarViewModel(MainWindowViewModel mainWindowViewModel) : this()
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
    
    
    //POLE DLA LISTY SPOTKAŃ Z BAZY DANYCH
    public ObservableCollection<Meeting> Meetings
    {
        get => _meetings;
        set => SetProperty(ref _meetings, value);
    }
    
    
    //METODY
    //Metoda do wczytywania spotkań z bazy danych
    private async Task LoadMeetings()
    {
        var userId = _mainWindowViewModel.LoggedUser.UserId;
        
        var dataTable = await Task.Run<DataTable>(() => _meetingRepository.GetMeetingsByDateAndUser(SelectedDate, userId));

        if (dataTable.Rows.Count == 0)
        {
            AreThereMeetings = false;
        }
        else
        {
            AreThereMeetings = true;

            var meetings = dataTable.AsEnumerable().Select(row => new Meeting
            {
                MeetingId = Guid.Parse(row["meetingId"].ToString()),
                Description = row["description"].ToString(),
                Date = DateTime.Parse(row["date"].ToString()),
                UserId = Guid.Parse(row["userId"].ToString()),
                Customer = LoadSelectedCustomer(Guid.Parse(row["customerId"].ToString())),
                Location = LoadSelectedLocation(Guid.Parse(row["locationId"].ToString())),
            }).ToList();

            Meetings = new ObservableCollection<Meeting>(meetings);
            
        }

        _logger.LogInformation("Pobrano spotkania z bazy danych!");
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
    
    partial void OnSelectedDateChanged(DateTime value)
    {
        LoadMeetings();
    }
    
    //Metoda do przejścia do ekranu edycji
    [RelayCommand]
    private void UpdateSelectedMeeting(Meeting meeting)
    {
        var updateViewModel = new MeetingUpdateViewModel(meeting.MeetingId, _meetingRepository, _customerRepository, _locationRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = updateViewModel;
        _logger.LogInformation("Przejście do widoku aktualizacji spotkania o ID {0}!", meeting.MeetingId);
    }
    
    //Metoda do przejścia do widoku dodawania klienta
    [RelayCommand]
    private void GoToAddScreen()
    {
        var addViewModel = new MeetingAddViewModel(_meetingRepository, _customerRepository, _locationRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = addViewModel;
        _logger.LogInformation("Przejście do widoków dodawania klientów! ");
        
    }
    
    //Metoda do usuwania klienta z bazy danych
    [RelayCommand]
    private async void DeleteMeeting(Meeting meeting)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie spotkania",
                "Czy na pewno chcesz usunąć to spotkanie?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime)
                .MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _meetingRepository.DeleteMeeting(meeting.MeetingId);
                LoadMeetings();
            }
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