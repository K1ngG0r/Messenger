using Client.Connection;
using Client.Data;
using Client.Models;
using Client.ViewModels.Patterns;
using Client.ViewModels;
using System.Configuration;
using System.Data;
using System.Net;
using System.Windows;
using Client.Views;

namespace Client
{
    public partial class App : Application
    {
        private LoginWindow? loginWindow;
        //private MainWindowViewModel? mainWindowViewModel;
        private MainWindow? mainWindow;
        private Mediator _mediator = null!;
        private ChatService _chatService = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            
            //IPEndPoint serverIP = IPEndPoint.Parse("127.0.0.1:9000");
            IPEndPoint serverIP = IPEndPoint.Parse("26.107.253.47:9000");
            var clientConnection = new ClientConnection(serverIP, new WpfPresentationService());
            var context = new AppDBContext();
            context.Database.EnsureCreated();
            _chatService = new ChatService(context, clientConnection);
            _mediator = new Mediator();
            _mediator.Register<LogoutRequestedMessage>(HandleLogoutRequestedMessage);
            _mediator.Register<LoginRequestedMessage>(HandleLoginRequestedMessage);

            var previousLoginSettings = CacheManager.TryGetPreviousLoginSettings();

            if (previousLoginSettings is null)
            {
                ShowLoginWindow();
                return;
            }
            var username = previousLoginSettings.Value.Item1;
            var password = previousLoginSettings.Value.Item2;
            if(_chatService.TryLogin(username, password))
            {
                ShowMainWindow();
                return;
            }
            CacheManager.ClearPreviousLoginSettings();
            ShowLoginWindow();
        }
        private void ShowLoginWindow()
        {
            LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel(_chatService, _mediator);
            loginWindow = new LoginWindow(loginWindowViewModel);
            loginWindow.Show();
        }
        private void ShowMainWindow()
        {
            if (mainWindow != null)
                return;
            var mainWindowViewModel = new MainWindowViewModel(_mediator,
                _chatService);
            mainWindow = new MainWindow(mainWindowViewModel);
            mainWindow.Show();
        }
        private void HandleLogoutRequestedMessage(object? obj)
        {
            mainWindow?.Close();
            //mainWindowViewModel?.Dispose();
            mainWindow = null;
            //mainWindowViewModel = null;
            _chatService.OnLogout();
            CacheManager.ClearPreviousLoginSettings();
            ShowLoginWindow();
        }
        private void HandleLoginRequestedMessage(object? obj)
        {
            loginWindow?.Close();
            ShowMainWindow();
        }
    }
}
