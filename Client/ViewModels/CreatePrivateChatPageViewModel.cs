using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Client.Models;
using Client.ViewModels.Patterns;

namespace Client.ViewModels
{
    public class CreatePrivateChatPageViewModel : ViewModel
    {
        private Mediator _mediator;
        private ChatService _chatService;
        private string contactUsername = string.Empty;
        private UserViewModel? foundUser;
        public UserViewModel? FoundUser
        {
            get => foundUser;
            set
            {
                foundUser = value;
                OnPropertyChanged();
            }
        }

        public string ContactUsername
        {
            get => contactUsername;
            set
            {
                contactUsername = value;
                OnPropertyChanged();
                var user = _chatService.TryLoadUserByUsername(contactUsername);
                if (user is null)
                    FoundUser = null;
                else
                    FoundUser = new UserViewModel(user);
            }
        }
        public Command SubmitCommand { get; }
        public Command NavigateToMainPageCommand { get; }
        public CreatePrivateChatPageViewModel(Mediator mediator, ChatService chatService)
        {
            _chatService = chatService;
            _mediator = mediator;
            SubmitCommand = new Command(OnSubmit);
            NavigateToMainPageCommand = new Command(OnNavigateToMainPage);
        }
        private void OnNavigateToMainPage()
        {
            _mediator.Send(new NavigateToMainPageMessage());
        }
        private void OnSubmit()
        {
            if (ContactUsername == string.Empty)
                return;
            _mediator.Send(new OpenPrivateChatMessage(ContactUsername));
            _mediator.Send(new NavigateToMainPageMessage());
        }
    }
}
