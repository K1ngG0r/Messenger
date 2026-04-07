using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Models;
using Client.ViewModels.Patterns;
using Client.ViewModels.Patterns.Services;
using Microsoft.Win32;

namespace Client.ViewModels
{
    public class SettingsPageViewModel : ViewModel
    {
        private Mediator _mediator;
        private ChatService _chatService;
        private string name;
        private string username;
        private string avatarPath;
        private AvatarImageViewModel avatar;
        private bool canSubmit;
        public AvatarImageViewModel Avatar
        {
            get => avatar;
            set
            {
                avatar = value;
                OnPropertyChanged();
            }
        }
            
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnSettingsChanged();
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnSettingsChanged();
            }
        }
        public string AvatarPath
        {
            get => avatarPath;
            set
            {
                avatarPath = value;
                Avatar = new AvatarImageViewModel(avatarPath);
                OnSettingsChanged();
            }
        }
        public Command NavigateToMainPageCommand { get; }
        public Command LogoutCommand { get; }
        public Command SubmitCommand { get; }
        public Command SelectImageCommand { get; }
        public bool CanSubmit
        {
            get => canSubmit;
            set
            {
                canSubmit = value;
                OnPropertyChanged();
            }
        }
        public SettingsPageViewModel(Mediator mediator, ChatService chatService)
        {
            _chatService = chatService;
            avatarPath = string.Empty;
            name = _chatService.CurrentUser.Name;
            username = _chatService.CurrentUser.Username;
            NavigateToMainPageCommand = new Command(OnNavigateToMainPage);
            LogoutCommand = new Command(OnLogout);
            SubmitCommand = new Command(OnSubmit);
            SelectImageCommand = new Command(OnSelectImageCommand);
            _mediator = mediator;
        }
        private void OnSettingsChanged([CallerMemberName]string name="")
        {
            OnPropertyChanged(name);
            if (CheckSettingsHasChanged())
            {
                CanSubmit = true;
                return;
            }
            CanSubmit = false;
        }
        private bool CheckSettingsHasChanged()
        {
            if (AvatarPath != string.Empty)
                return true;
            if (Name != _chatService.CurrentUser.Name)
                return true;
            if (Username != _chatService.CurrentUser.Username)
                return true;
            return false;
        }
        private void OnNavigateToMainPage()
        {
            _mediator.Send(new NavigateToMainPageMessage());
        }
        private void OnLogout()
        {
            _mediator.Send(new LogoutRequestedMessage());
        }
        private void OnSelectImageCommand()
        {
            OpenFileDialog fd = new OpenFileDialog();
            if(fd.ShowDialog() ?? false)
            {
                var extensionSplit = fd.FileName.Split(".");
                if (extensionSplit.Length == 0)
                    return;
                string extension = extensionSplit[extensionSplit.Length - 1];
                if (!(extension == "png"))
                {
                    MessageBox.Show("Неверный формат изображения");
                    return;
                }
                AvatarPath = fd.FileName;
            }
        }
        private void OnSubmit()
        {
            if (!CheckSettingsHasChanged())
                return;
            //обновление настроек
        }
    }
}
