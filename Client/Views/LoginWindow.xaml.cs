using Client.Connection;
using Client.Data;
using Client.Models;
using Client.ViewModels;
using Client.ViewModels.Patterns;
using Client.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginWindowViewModel loginWindowViewModel)
        {
            InitializeComponent();
            DataContext = loginWindowViewModel;
        }
    }
}