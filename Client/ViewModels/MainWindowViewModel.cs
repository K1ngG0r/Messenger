using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Client.ViewModels.Patterns;

namespace Client.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private Mediator _mediator;
        private ViewModel activePageViewModel;
        public ViewModel ActivePageViewModel
        {
            get => activePageViewModel;
            set
            {
                activePageViewModel = value;
                OnPropertyChanged();
            }
        }
        public MainPageViewModel MainPageViewModel { get; }
        public SettingsPageViewModel SettingsPageViewModel { get; }
        public ChatPageViewModel ChatPageViewModel { get; }
        public MainWindowViewModel(MainPageViewModel mainPageViewModel,
            ChatPageViewModel chatPageViewModel,
            SettingsPageViewModel settingsPageViewModel,
            Mediator mediator)
        {
            MainPageViewModel = mainPageViewModel;
            ChatPageViewModel = chatPageViewModel;
            SettingsPageViewModel = settingsPageViewModel;
            activePageViewModel = MainPageViewModel;
            _mediator = mediator;
            _mediator.Register<NavigateToSettingsPage>(OnNavigateToSettingsPage);
            _mediator.Register<NavigateToMainPage>(OnNavigateToMainPage);
        }
        private void OnNavigateToSettingsPage(object? parameters)
        {
            ActivePageViewModel = SettingsPageViewModel;
        }
        private void OnNavigateToMainPage(object? parameters)
        {
            ActivePageViewModel = MainPageViewModel;
        }
    }
}
