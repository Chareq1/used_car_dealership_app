using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace used_car_dealership_app.ViewModels;

public partial class SplashScreenViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _startupMessage = "Uruhcamianie aplikacji...";

    public void Cancel()
    {
        StartupMessage = "Cancelling...";
        _cts.Cancel();
    }

    private readonly CancellationTokenSource _cts = new();

    public CancellationToken CancellationToken => _cts.Token;
}
