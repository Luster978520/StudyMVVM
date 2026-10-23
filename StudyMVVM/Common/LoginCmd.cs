using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudyMVVM.Common
{
    public class LoginCmd : ICommand
    {

        public Action DoAction { get; set; }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //if (DoAction != null)
            //{
            //    DoAction.Invoke();
            //}
            DoAction?.Invoke();
        }
    }
}
