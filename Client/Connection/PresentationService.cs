using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Connection
{
    public interface IPresentationService
    {
        public void DisplayMessage(string message);
    }
    public class NullPresentationService : IPresentationService
    {
        public void DisplayMessage(string message)
        {

        }
    }
    public class WpfPresentationService : IPresentationService
    {
        public void DisplayMessage(string message)
        {
            Task.Run(() => MessageBox.Show(message));
        }
    }
}
