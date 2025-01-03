using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using used_car_dealership_app.Models;
using used_car_dealership_app.Repositories;
using used_car_dealership_app.Services;
using used_car_dealership_app.ViewModels.Documents;
using used_car_dealership_app.ViewModels.Locations;
using used_car_dealership_app.ViewModels.Users;
using used_car_dealership_app.ViewModels.Vehicles;
using used_car_dealership_app.Views;

namespace used_car_dealership_app.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen = true;
    
    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty] 
    private ListItemTemplate? _selectedListItem;
    
    [ObservableProperty]
    private User _loggedUser;
    
    [ObservableProperty] 
    private String _loggedUserFullName;
    
    public MainWindowViewModel(User loggedUser)
    {
        _currentPage = new VehiclesViewModel(this);
        _loggedUser = loggedUser;
        _loggedUserFullName = $"{loggedUser.Name} {loggedUser.Surname}";
        InitializePaneItems();
    }
    
    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;

        var instance = Activator.CreateInstance(value.ModelType, this);
        if (instance is null) return;
        CurrentPage = (ViewModelBase)instance;
    }

    public ObservableCollection<ListItemTemplate> Items { get; set; } = new();

    private void InitializePaneItems()
    {
        Items.Add(new ListItemTemplate(typeof(VehiclesViewModel), "Pojazdy", "VehicleCarRegular"));
        Items.Add(new ListItemTemplate(typeof(LocationsViewModel), "Lokalizacje", "LocationRegular"));
        Items.Add(new ListItemTemplate(typeof(Clients.ClientsViewModel), "Klienci", "PersonBoardRegular"));
        Items.Add(new ListItemTemplate(typeof(DocumentsViewModel), "Dokumenty", "DocumentRegular"));
        Items.Add(new ListItemTemplate(typeof(Calendar.CalendarViewModel), "Kalendarz", "CalendarRegular"));

        if (_loggedUser.Type == UserType.ADMINISTRATOR) { Items.Add(new ListItemTemplate(typeof(UsersViewModel), "Użytkownicy", "PeopleSettingsRegular")); }
    }
    
    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
    
    [RelayCommand]
    private void OpenProfile()
    {
        CurrentPage = new ProfileViewModel(LoggedUser.UserId, new UserRepository(), this);
    }
    
    [RelayCommand]
    private async Task Logout()
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard("Wylogowywanie", "Czy chcesz wylogować się z aplikacji?", ButtonEnum.YesNo, Icon.Warning);
        var mainWindow = (MainWindow)((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow;
        var result = await messageBoxStandardWindow.ShowAsPopupAsync(mainWindow);

        if (result == ButtonResult.Yes)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWin = (MainWindow)desktop.MainWindow;
                var loginWindow = new LoginWindow()
                {
                    DataContext = new LoginViewModel()
                };
            
                desktop.MainWindow = loginWindow;
                
                loginWindow.Show();
                
                mainWin.Close();
            }
        }
    }
}

public class ListItemTemplate {
    public ListItemTemplate(Type type, String name, string iconKey)
    {
        ModelType = type;
        Label = name;
        
        Application.Current!.TryFindResource(iconKey, out var res);
        Icon = (StreamGeometry)res;
    }
    
    public string Label { get; }
    public Type ModelType { get; }
    public StreamGeometry Icon { get; }
}