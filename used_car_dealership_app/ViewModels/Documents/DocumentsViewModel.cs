namespace used_car_dealership_app.ViewModels;

public class DocumentsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    public DocumentsViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
}