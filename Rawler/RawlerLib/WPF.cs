using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RawlerLib.Wpf
{
    internal class GenericCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (CanExecutePredicate != null)
            {
                return CanExecutePredicate(parameter);
            }
            else
            {
                return true;
            }
        }

        public event EventHandler CanExecuteChanged;
        public event Predicate<object> CanExecutePredicate;
        public event EventHandler<Event.Args<object>> ExecuteEvent;


        public void Execute(object parameter)
        {
            if (ExecuteEvent != null)
            {
                ExecuteEvent(this, new Event.Args<object>(parameter));
            }
        }
    }

    
    
}
