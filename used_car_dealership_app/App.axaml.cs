using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using used_car_dealership_app.Services;
using used_car_dealership_app.ViewModels;
using used_car_dealership_app.Views;

namespace used_car_dealership_app;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
                var splashScreenVM = new SplashScreenViewModel();
                var splashScreen = new SplashScreenView {
                    DataContext = splashScreenVM
                };
                
                desktop.MainWindow = splashScreen;
                splashScreen.Show();

                try {
                    splashScreenVM.StartupMessage = "Uruchamianie potrzebnych usług...";
                    await Task.Delay(3000, splashScreenVM.CancellationToken);
                    splashScreenVM.StartupMessage = "Ładowanie zasobów...";
                    await Task.Delay(1500, splashScreenVM.CancellationToken);
                    splashScreenVM.StartupMessage = "Konfigurowanie...";
                    await Task.Delay(2000, splashScreenVM.CancellationToken);
                    splashScreenVM.StartupMessage = "Finalizacja...";
                    await Task.Delay(2500, splashScreenVM.CancellationToken);
                }
                catch (TaskCanceledException) {
                    splashScreen.Close();
                    return;
                }
                
                var mainWin = new MainWindow {
                    DataContext = new MainWindowViewModel(),
                };
                desktop.MainWindow = mainWin;
                mainWin.Show();
                
                splashScreen.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
} 