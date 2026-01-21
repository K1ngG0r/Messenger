using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Models;
using Client.ViewModels.Patterns;

namespace Client.ViewModels
{
    public class SettingsPageViewModel : ViewModel
    {
        private Mediator _mediator;
        private ChatService _chatService;
        public string Name { get; set; }
        public string Usernameame { get; set; }
        //public string Avatar { get; set; }
        public Command NavigateToMainPageCommand { get; }
        public Command LogoutCommand { get; }
        public Command SubmitCommand { get; }
        public SettingsPageViewModel(Mediator mediator, ChatService chatService)
        {
            _chatService = chatService;
            Name = _chatService.CurrentUser!.Name;
            Usernameame = _chatService.CurrentUser!.Username;
            NavigateToMainPageCommand = new Command(OnNavigateToMainPage);
            LogoutCommand = new Command(OnLogout);
            SubmitCommand = new Command(OnSubmit);
            _mediator = mediator;
        }
        private void OnNavigateToMainPage()
        {
            _mediator.Send(new NavigateToMainPageMessage());
        }
        private void OnLogout()
        {
            _mediator.Send(new LogoutRequestedMessage());
        }
        private void OnSubmit()
        {
            MessageBox.Show("В разработке");
        }
    }
}
