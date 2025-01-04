﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Notification;
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

//KLASA WIDOKU GŁÓWNEGO OKNA APLIKACJI
public partial class MainWindowViewModel : ViewModelBase
{
    //WŁAŚCIWOŚCI DLA STRONY, CZY PANEL JEST OTWARTY, WYBRANEGO ELEMENTU LISTY, ZALOGOWANEGO UŻYTKOWNIKA I JEGO PEŁNEGO IMIENIA
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
    
    
    //WŁAŚCIWOŚĆ DLA ELEMENTÓW LISTY
    public ObservableCollection<ListItemTemplate> Items { get; set; } = new();
    
    
    //KONSTRUKTOR
    public MainWindowViewModel(User loggedUser)
    {
        _currentPage = new VehiclesViewModel(this);
        _loggedUser = loggedUser;
        _loggedUserFullName = $"{loggedUser.Name} {loggedUser.Surname}";
        
        InitializePaneItems();
    }
    
    //METODY
    //Metoda do zmiany strony
    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;

        var instance = Activator.CreateInstance(value.ModelType, this);
        if (instance is null) return;
        CurrentPage = (ViewModelBase)instance;
    }

    //Metoda do inicjalizacji elementów listy
    private void InitializePaneItems()
    {
        Items.Add(new ListItemTemplate(typeof(VehiclesViewModel), "Pojazdy", "VehicleCarRegular"));
        Items.Add(new ListItemTemplate(typeof(LocationsViewModel), "Lokalizacje", "LocationRegular"));
        Items.Add(new ListItemTemplate(typeof(Clients.ClientsViewModel), "Klienci", "PersonBoardRegular"));
        Items.Add(new ListItemTemplate(typeof(DocumentsViewModel), "Dokumenty", "DocumentRegular"));
        Items.Add(new ListItemTemplate(typeof(Calendar.CalendarViewModel), "Kalendarz", "CalendarRegular"));

        if (_loggedUser.Type == UserType.ADMINISTRATOR) { Items.Add(new ListItemTemplate(typeof(UsersViewModel), "Użytkownicy", "PeopleSettingsRegular")); }
    }
    
    
    //KOMENDY
    //Komenda do otwierania i zamykania panelu
    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }
    
    //Komenda do otwierania profilu
    [RelayCommand]
    private void OpenProfile()
    {
        CurrentPage = new ProfileViewModel(LoggedUser.UserId, new UserRepository(), this);
    }
    
    //Komenda do wylogowywania
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


//KLASA SZABLONU ELEMENTU LISTY
public class ListItemTemplate {
    public ListItemTemplate(Type type, String name, String iconKey)
    {
        ModelType = type;
        Label = name;
        
        Application.Current!.TryFindResource(iconKey, out var res);
        Icon = (StreamGeometry)res;
    }
    
    public String Label { get; }
    public Type ModelType { get; }
    public StreamGeometry Icon { get; }
}