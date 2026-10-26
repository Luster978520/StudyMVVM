using StudyMVVM.Model;
using StudyMVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudyMVVM.Views
{
    /// <summary>
    /// LogicView.xaml 的交互逻辑
    /// </summary>
    public partial class LogicView : Window
    {

        LogicViewModel logicViewModel = new LogicViewModel();
        public LogicView()
        {
            InitializeComponent();

            this.DataContext = logicViewModel;  //上下文一定要是视图
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            shapFrm shapFrm=new shapFrm();
            shapFrm.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            paintVisualFrm paintVisualFrm = new paintVisualFrm();
            paintVisualFrm.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            effectFrm effectFrm = new effectFrm();
            effectFrm.ShowDialog();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PalyFrm palyFrm = new PalyFrm();
            palyFrm.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            RenderAndRotateFrm renderAndRotateFrm = new RenderAndRotateFrm();
            renderAndRotateFrm.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

            controlTemplateFrm controlTemplate=new controlTemplateFrm();
            controlTemplate.ShowDialog();
        }

        private void OpenTemplateLab_Click(object sender, RoutedEventArgs e)
        {
            var window = new TemplateLabWindow { Owner = this };
            window.Show();
        }

        //    private void Button_Click(object sender, RoutedEventArgs e)
        //    {
        //        string account= logicViewModel.Account;
        //        string pwd= logicViewModel.Pwd;
        //    }

        //    private void Button_Click_1(object sender, RoutedEventArgs e)
        //    {
        //        string account= logicViewModel.Account;
        //        string pwd= logicViewModel.Pwd;
        //        if (account=="admin"&&pwd=="123")
        //        {
        //            MessageBox.Show($"Account:{account},Pwd:{pwd}");
        //            logicViewModel.Account = "";
        //            logicViewModel.Pwd = "";
        //        }

        //        logicViewModel.Account = "";
        //        logicViewModel.Pwd = "";
        //    }

        //    private void Button_Click_2(object sender, RoutedEventArgs e)
        //    {

        //    }
        //}
    }
}
