using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
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
           // Create the splash screen
                var splashScreenVM = new SplashScreenViewModel();
                var splashScreen = new SplashScreenView {
                    DataContext = splashScreenVM
                };
                
                desktop.MainWindow = splashScreen;
                splashScreen.Show();

                try {
                    // Initialize the device. We can interact with splashScreenVM.CancellationToken
                    // to determine if the user wants to abort the connection process.
                    splashScreenVM.StartupMessage = "Searching for devices...";
                    await Task.Delay(1000, splashScreenVM.CancellationToken);
                    splashScreenVM.StartupMessage = "Connecting to device #1...";
                    await Task.Delay(2000, splashScreenVM.CancellationToken);
                    splashScreenVM.StartupMessage = "Configuring device...";
                    await Task.Delay(2000, splashScreenVM.CancellationToken);
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
                
                // Get rid of the splash screen
                splashScreen.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
} 