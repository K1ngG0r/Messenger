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
        private CurrentUserService _userService;
        public Command NavigateToMainPageCommand { get; set; }
        public SettingsPageViewModel(Mediator mediator, CurrentUserService userService)
        {
            NavigateToMainPageCommand = new Command(NavigateToMainPage);
            _mediator = mediator;
            _userService = userService;
        }
        private void NavigateToMainPage()
        {
            _mediator.Send(new NavigateToMainPage());
        }
    }
}
