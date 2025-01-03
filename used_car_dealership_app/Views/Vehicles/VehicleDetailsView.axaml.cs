using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace used_car_dealership_app.Views.Vehicles;

public partial class VehicleDetailsView : UserControl
{
    private int _currentIndex = 0;
    
    public VehicleDetailsView()
    {
        InitializeComponent();
        CarImages.SelectedIndex = _currentIndex;
    }
    
    public void Next(object? source, RoutedEventArgs args)
    {
        if (_currentIndex < CarImages.Items.Count() - 1)
        {
            _currentIndex++;
            CarImages.SelectedIndex = _currentIndex;
        }
    }

    public void Previous(object? source, RoutedEventArgs args) 
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            CarImages.SelectedIndex = _currentIndex;
        }
    }

}