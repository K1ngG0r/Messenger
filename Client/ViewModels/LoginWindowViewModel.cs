using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Models;
using Client.ViewModels.Patterns;
using Client.Views;

namespace Client.ViewModels
{
    public class LoginWindowViewModel : ViewModel
    {
        private ChatService _chatService;
        private Mediator _mediator;
        private string errorMessage = string.Empty;
        public Command TryLoginCommand { get; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? SessionKey { get; set; }
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }
        public LoginWindowViewModel(ChatService chatService, Mediator mediator)
        {
            _chatService = chatService;
            _mediator = mediator;
            TryLoginCommand = new Command(OnTryLogin);
        }
        private void OnTryLogin()
        {
            ErrorMessage = string.Empty;
            if (Login == string.Empty || Password == string.Empty)
            {
                ErrorMessage = "Пустое поле";
                return;
            }
            if (_chatService.TryLogin(Login, Password))
            {
                _mediator.Send(new LoginRequestedMessage());
            }
            else
            {
                ErrorMessage = "Не удалось войти";
            }
        }
    }
}
