using StudyMVVM.Common;
using StudyMVVM.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace StudyMVVM.ViewModel
{
    public class LogicViewModel:INotifyPropertyChanged
    {
       public LogicModel logicModel=new	LogicModel();

        public event PropertyChangedEventHandler PropertyChanged;

       

       
        public string Account
		{
			get { return logicModel.Account; }
			set {
                logicModel.Account = value;

                OnPropertyChanged(Account);
            }
		}


		private string _pwd;

		public string Pwd
		{
			get { return logicModel.Pwd; }
			set {
                logicModel.Pwd = value;
                OnPropertyChanged(Pwd);
            }
		}




        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //登录的也为
        private void Login() {
            string account =Account;
            string pwd = this.Pwd;

            if (account == "admin" && pwd == "123")
            {
               
            }
            MessageBox.Show($"Account:{account},Pwd:{pwd}");
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Account = "";
            this.Pwd = "";
        }

        public LoginCmd LoginCommand
        {
            get
            {   LoginCmd cmd = new LoginCmd();
                cmd.DoAction = Login;
                return cmd;
            }//只读即可    点击后返回
        }
    }
}
