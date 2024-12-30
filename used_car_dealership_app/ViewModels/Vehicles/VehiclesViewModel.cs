namespace used_car_dealership_app.ViewModels.Vehicles;

public class VehiclesViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    public VehiclesViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
}