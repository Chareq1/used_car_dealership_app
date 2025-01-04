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
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels.Users;

//KLASA WIDOKU DO WYŚWIETLANIA LISTY UŻYTKOWNIKÓW
[CustomInfo("Widok listy użytkowników", 1.0f)]
public partial class UsersViewModel: ViewModelBase
{
    //POLE DLA USŁUGI NOTYFIKACJI
    private readonly NotificationService _notifications;
    
    
    //POLA DLA LOGGERA
    private static ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    private ILogger _logger = _loggerFactory.CreateLogger<UsersViewModel>();
    
    
    //POLA DLA WSZYSTKICH POTRZEBNYCH DANYCH
    private readonly UserRepository _userRepository;
    private ObservableCollection<User> _users;
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    
    //WŁAŚCIWOŚĆ DO SPRAWDZANIA CZY SĄ UŻYTKOWNICY
    [ObservableProperty]
    private bool _areThereUsers = false;
    
    
    //WŁAŚCIWOŚCI DO WYSZUKIWANA UŻYTKOWNIKÓW
    [ObservableProperty]
    private String _searchText;

    [ObservableProperty]
    private String _selectedSearchField;

    [ObservableProperty] 
    private List<String> _searchFields;
    
    
    //POLE DLA LISTY UŻYTKOWNIKÓW Z BAZY DANYCH
    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }
    
    
    //KONSTRUKTOR DLA WIDOKU
    public UsersViewModel()
    {
        _userRepository = new UserRepository();
        _searchFields = new List<String> { "Imię", "Nazwisko", "Telefon", "Email", "Typ użytkownika"};
        
        var attributes = typeof(UsersViewModel).GetCustomAttributes(typeof(CustomInfoAttribute), false);
        if (attributes.Length > 0)
        {
            var customInfo = (CustomInfoAttribute)attributes[0];
            _logger.LogWarning($"Opis: {customInfo.Description}, Wersja: v{customInfo.Version}");
        }
        
        LoadUsers();
    }
    
    
    //KONSTRUKTOR DLA WIDOKU Z MAINWINDOWVIEWMODEL
    public UsersViewModel(MainWindowViewModel mainWindowViewModel) : this()
    {
        _mainWindowViewModel = mainWindowViewModel;
        _notifications = new NotificationService(_mainWindowViewModel);
    }
    
    
    //METODY
    //Metoda do wczytywania lokalizacji z bazy danych
    private async Task LoadUsers()
    {
        var dataTable = await Task.Run(() => _userRepository.GetAllUsers());

        if (dataTable.Rows.Count == 0)
        {
            AreThereUsers = false;
            _notifications.ShowInfo("Użytkownicy", "Brak użytkowników!");
        }
        else
        {
            AreThereUsers = true;
            
            _notifications.ShowInfo("Użytkownicy", "Ilość użytkowników: " + dataTable.Rows.Count);
            
            var users = dataTable.AsEnumerable().Select(row => new User
            {
                UserId = Guid.Parse(row["userId"].ToString()),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString(),
                Type = Enum.TryParse(row["type"].ToString(), out UserType userType) ? userType : UserType.WORKER
            }).ToList();
            
            Users = new ObservableCollection<User>(users);
        }
        
        _logger.LogInformation("Pobrano użytkowników z bazy danych!");
    }
    
    //Metoda do wyszukiwania użytkowników w bazie danych
    [RelayCommand]
    private async Task SearchUsersAsync()
    {
        var selectedColumn = "";
            
        if (String.IsNullOrEmpty(SearchText) || String.IsNullOrEmpty(SelectedSearchField))
        {
            await LoadUsers();
            return;
        }

        switch (SelectedSearchField)
        {
            case "Imię":
                selectedColumn = "name";
                break;
            case "Nazwisko":
                selectedColumn = "surname";
                break;
            case "Telefon":
                selectedColumn = "phone";
                break;
            case "Email":
                selectedColumn = "email";
                break;
            case "Typ użytkownika":
                selectedColumn = "type";
                break;
        }

        var query = $"SELECT * FROM users WHERE \"{selectedColumn}\" LIKE @searchText";
        var parameters = new List<NpgsqlParameter>();
        
        
        if (SelectedSearchField == "Typ użytkownika")
        {
            if (Enum.TryParse(typeof(UserType), SearchText, true, out var userType))
            {
                parameters.Add(new NpgsqlParameter("@searchText", userType.ToString()));
                query  = $"SELECT * FROM users WHERE \"{selectedColumn}\" = @searchText::\"usertype\";";
            }
            else
            {
                AreThereUsers = false;
                _logger.LogWarning("Nieprawidłowa wartość dla typu użytkownika!");
                return;
            }
        }
        else
        {
            parameters.Add(new NpgsqlParameter("@searchText", $"%{SearchText}%"));
        }

        var dataTable = await Task.Run(() => _userRepository.ExecuteQuery(query, parameters));

        if (dataTable.Rows.Count == 0)
        {
            AreThereUsers = false;
            _notifications.ShowInfo("Użytkownicy", "Brak użytkowników o podanych informacjach!");
        }
        else
        {
            AreThereUsers = true;
            
            _notifications.ShowInfo("Użytkownicy", "Ilość znalezionych użytkowników: " + dataTable.Rows.Count);

            var users = dataTable.AsEnumerable().Select(row => new User
            {
                UserId = Guid.Parse(row["userId"].ToString()),
                Name = row["name"].ToString(),
                Surname = row["surname"].ToString(),
                Email = row["email"].ToString(),
                Phone = row["phone"].ToString(),
                Type = Enum.TryParse(row["type"].ToString(), out UserType userType) ? userType : UserType.WORKER
            }).ToList();

            Users = new ObservableCollection<User>(users);
        }

        _logger.LogInformation("Wyszukano użytkowników w bazie danych!");
    }
    
    //Metoda do pokazania szczegółów użytkownika (przejścia do innego widoku)
    [RelayCommand]
    private void ShowUserDetails(User user)
    {
        var detailsViewModel = new UserDetailsViewModel(user.UserId, _userRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = detailsViewModel;
        _logger.LogInformation("Przejście do widoku szczegółów użytkownika o ID {0}!", user.UserId);
    }
    
    //Metoda do przejścia do widoku dodawania użytkownika
    [RelayCommand]
    private void GoToAddScreen()
    {
        var addViewModel = new UserAddViewModel(_userRepository, _mainWindowViewModel);
        _mainWindowViewModel.CurrentPage = addViewModel;
        _logger.LogInformation("Przejście do widoku dodawania użytkownika! ");
    }
    
    //Metoda do usuwania użytkownika z bazy danych
    [RelayCommand]
    private async void DeleteUser(User user)
    {
        try
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Usunięcie użytkownika", "Czy na pewno chcesz usunąć tego użytkownika?", ButtonEnum.YesNo, Icon.Warning);
            var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
            var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

            if (result == ButtonResult.Yes)
            {
                _userRepository.DeleteUser(user.UserId);
                _notifications.ShowSuccess("Usuwanie użytkownika", "Operacja zakończona pomyślnie!");
                LoadUsers();
            }
        }
        catch (Exception ex)
        {
            _notifications.ShowError("Problem z usunięciem użytkownika", ex.Message);
            _logger.LogError(ex, "Błąd podczas usuwania użytkownika z bazy danych!");
        }
    }
}