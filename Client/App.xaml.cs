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
using Client.ViewModels.Patterns.Services;

namespace Client
{
    public partial class App : Application
    {
        private LoginWindow? loginWindow;
        private MainWindow? mainWindow;
        private Mediator _mediator = null!;
        private ChatService _chatService = null!;
        private bool ShutdownOnClose = false;
        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            //IPEndPoint serverIP = IPEndPoint.Parse("127.0.0.1:9000");
            IPEndPoint serverIP = IPEndPoint.Parse("90.188.16.58:4232");
            var clientConnection = new ClientConnection(serverIP, new WpfPresentationService());
            var context = new AppDBContext();
            context.Database.EnsureCreated();
            _mediator = new Mediator();
            _chatService = new ChatService(context, clientConnection);
            _mediator.Register<LogoutRequestedMessage>(HandleLogoutRequestedMessage);
            _mediator.Register<LoginRequestedMessage>(HandleLoginRequestedMessage);


#if !UserLogin
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
#else
            _chatService.CurrentUserSettings = new CurrentUserSettings("test username", "test name",
                CacheManager.GetUserAvatarPathByUsername("test"));
            SwitchToMainWindow();
#endif
        }
        private void OnWindowClosed(object? o, EventArgs e)
        {
            if (!ShutdownOnClose)
                return;
            Shutdown();
        }
        private void SwitchToLoginWindow()
        {
            ShutdownOnClose = false;
            mainWindow?.Close();
            mainWindow = null;
            _chatService.OnLogout();
            CacheManager.ClearPreviousLoginSettings();
            if (loginWindow != null)
                return;
            LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel(_chatService, _mediator);
            loginWindow = new LoginWindow(loginWindowViewModel);
            ShutdownOnClose = true;
            loginWindow.Closed += OnWindowClosed;
            loginWindow.Show();
        }
        private void SwitchToMainWindow()
        {
            ShutdownOnClose = false;
            loginWindow?.Close();
            loginWindow = null;
            if (mainWindow != null)
                return;
            _chatService.OnLogin();
            var mainWindowViewModel = new MainWindowViewModel(_mediator,
                _chatService);
            _chatService.OnLogin();
            mainWindow = new MainWindow(mainWindowViewModel);
            ShutdownOnClose = true;
            mainWindow.Closed += OnWindowClosed;
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
