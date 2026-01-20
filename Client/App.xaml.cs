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
            
            IPEndPoint serverIP = IPEndPoint.Parse("127.0.0.1:9000");
            var clientConnection = new ClientConnection(serverIP, new WpfPresentationService());
            var context = new AppDBContext();
            context.Database.EnsureCreated();
            _chatService = new ChatService(context, clientConnection);
            _mediator = new Mediator();
            _mediator.Register<LogoutRequestedMessage>(HandleLogoutRequestedMessage);
            _mediator.Register<LoginRequestedMessage>(HandleLoginRequestedMessage);

            var previousSessionKey = CacheManager.TryGetPreviousUserSessionKey();
            //если ключ не найден
            if (previousSessionKey is null ? true : previousSessionKey == string.Empty)
            {
                ShowLoginWindow();
                return;
            }
            //если есть ключ то пробуем сразу запустить приложение

        }
        private void ShowLoginWindow()
        {
            LoginWindowViewModel loginWindowViewModel = new LoginWindowViewModel(_chatService, _mediator);
            loginWindow = new LoginWindow(loginWindowViewModel);
        }
        private void ShowMainWindow()
        {
            try
            {
                var mainWindowViewModel = new MainWindowViewModel(_mediator,
                    _chatService);

                MessageBox.Show("init vm");
                var mainWindow = new MainWindow(mainWindowViewModel);
                MessageBox.Show("init mainwindow");
                mainWindow.Show();
                MessageBox.Show("wow!");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"error show window: {ex.Message}");
            }
        }
        private void HandleLogoutRequestedMessage(object? obj)
        {
            mainWindow?.Close();
            //mainWindowViewModel?.Dispose();
            mainWindow = null;
            //mainWindowViewModel = null;
            _chatService.OnLogout();
            ShowLoginWindow();
        }
        private void HandleLoginRequestedMessage(object? obj)
        {
            loginWindow?.Close();
            if (!(mainWindow is null))
                return;
            var mainWindowViewModel = new MainWindowViewModel(_mediator, _chatService);
            mainWindow = new MainWindow(mainWindowViewModel);
            mainWindow.Show();
        }
    }
}
