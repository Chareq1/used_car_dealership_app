using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Notification;
using used_car_dealership_app.Services.Interfaces;
using used_car_dealership_app.ViewModels;

namespace used_car_dealership_app.Services;

public class NotificationService: INotificationService
{
    private readonly INotificationMessageManager _manager;

        public NotificationService(MainWindowViewModel mainWindowViewModel)
        {
            _manager = mainWindowViewModel.Manager;
        }
        

        public void ShowError(String title, String message)
        {
            _manager.CreateMessage()
                .Accent("#FF3B3B")
                .Background("#FF3B3B")
                .Animates(true)
                .Foreground("White")
                .HasHeader(title)
                .HasBadge("BŁĄD")
                .HasMessage(message)
                .Dismiss().WithButton("\u2715", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                .WithOverlay(new ProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 3,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false
                })
                .Queue();
        }
        
        public void ShowInfo(String title, String message)
        {
            _manager.CreateMessage()
                .Accent("#6BA3BE")
                .Background("#6BA3BE")
                .Animates(true)
                .Foreground("White")
                .HasHeader(title)
                .HasBadge("INFO")
                .HasMessage(message)
                .Dismiss().WithButton("\u2715", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                .WithOverlay(new ProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 3,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false
                })
                .Queue();
        }
        
        
        public void ShowWarning(String title, String message)
        {
            _manager.CreateMessage()
                .Accent("#E0A030")
                .Background("#E0A030")
                .Animates(true)
                .Foreground("White")
                .HasHeader(title)
                .HasBadge("WARN")
                .HasMessage(message)
                .Dismiss().WithButton("\u2715", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                .WithOverlay(new ProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 3,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false
                })
                .Queue();
        }
        
        public void ShowSuccess(String title, String message)
        {
            _manager.CreateMessage()
                .Accent("#32A852")
                .Background("#32A852")
                .Animates(true)
                .Foreground("White")
                .HasHeader(title)
                .HasBadge("SUKCES")
                .HasMessage(message)
                .Dismiss().WithButton("\u2715", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                .WithOverlay(new ProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 3,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false
                })
                .Queue();
        }
        
}