using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyMVVM.Model
{
    public class LogicModel:INotifyPropertyChanged
    {

        private string account;

        public string Account
        {
            get { return account; }
            set {

                account = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(Account));
                }

            //   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Account)));
            }
        }

       
       

        public event PropertyChangedEventHandler PropertyChanged;
        private string pwd;

        public string Pwd
        {
            get { return pwd; }
            set {
                pwd = value; 
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(Pwd));
            }
        }

    }
}
