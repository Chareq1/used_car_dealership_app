using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace used_car_dealership_app.ViewModels;

//KLASA WIDOKU SPLASHSCREEN
public partial class SplashScreenViewModel : ViewModelBase
{
    //WŁAŚCIWOŚĆ DLA WIADOMOŚCI STARTOWEJ
    [ObservableProperty]
    private String _startupMessage = "Uruhcamianie aplikacji...";
    
    //WŁAŚCIWOŚĆ DLA TOKENA ANULOWANIA
    public CancellationToken CancellationToken => _cts.Token;
    
    //WŁAŚCIWOŚĆ DLA ŹRÓDŁA TOKENU ANULOWANIA
    private readonly CancellationTokenSource _cts = new();

    
    //METODY
    //Metoda do anulowania
    public void Cancel()
    {
        StartupMessage = "Cancelling...";
        _cts.Cancel();
    }
}
