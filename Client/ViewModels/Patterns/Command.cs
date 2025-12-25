using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.ViewModels.Patterns
{
    public class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private Action Implemention { get; set; }
        public Command(Action implemention)
        {
            Implemention = implemention;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Implemention?.Invoke();
        }
    }
}
