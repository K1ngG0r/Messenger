using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;

namespace Client.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public MainPageViewModel MainPageViewModel { get; set; }
        public ChatPageViewModel ChatPageViewModel { get; set; }
        public SettingsPageViewModel SettingsPageViewModel { get; set; }
        public MainWindowViewModel(MainPageViewModel mainPageViewModel,
            ChatPageViewModel chatPageViewModel,
            SettingsPageViewModel settingsPageViewModel)
        {
            MainPageViewModel = mainPageViewModel;
            ChatPageViewModel = chatPageViewModel;
            SettingsPageViewModel = settingsPageViewModel;
        }
    }
}
