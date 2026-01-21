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
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
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
                SwitchToLoginWindow();
                return;
            }
            var username = previousLoginSettings.Value.Item1;
            var password = previousLoginSettings.Value.Item2;
            if (_chatService.TryLogin(username, password))
            {
                SwitchToMainWindow();
                return;
            }
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            CacheManager.ClearPreviousLoginSettings();
            SwitchToLoginWindow();
        }
        private void OnWindowClosed(object? o, EventArgs e)
        {
            if (this.MainWindow == null)
                return;
            Shutdown();
        }
        private void SwitchToLoginWindow()
        {
            this.MainWindow = null;
            mainWindow?.Close();
            mainWindow = null;
            //mainWindowViewModel = null;
            _chatService.OnLogout();
            CacheManager.ClearPreviousLoginSettings();
            //mainWindowViewModel?.Dispose();
            if (loginWindow != null)
                return;
            LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel(_chatService, _mediator);
            loginWindow = new LoginWindow(loginWindowViewModel);
            loginWindow.Closed += OnWindowClosed;
            this.MainWindow = loginWindow;
            loginWindow.Show();
        }
        private void SwitchToMainWindow()
        {
            this.MainWindow = null;
            loginWindow?.Close();
            loginWindow = null;
            if (mainWindow != null)
                return;
            var mainWindowViewModel = new MainWindowViewModel(_mediator,
                _chatService);
            mainWindow = new MainWindow(mainWindowViewModel);
            mainWindow.Closed += OnWindowClosed;
            this.MainWindow = mainWindow;
            mainWindow.Show();
        }
        private void HandleLogoutRequestedMessage(object? obj)
        {
            SwitchToLoginWindow();
        }
        private void HandleLoginRequestedMessage(object? obj)
        {
            SwitchToMainWindow();
        }
    }
}
