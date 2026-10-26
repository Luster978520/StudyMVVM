using StudyMVVM.ViewModel;
using System;
using System.Windows;

namespace StudyMVVM.Views
{
    public partial class TemplateLabWindow : Window
    {
        public TemplateLabWindow()
        {
            InitializeComponent();
            DataContext = new TemplateLabViewModel();
            // Keep the complete workspace within the current desktop work area.
            Width = Math.Min(1440, SystemParameters.WorkArea.Width * 0.94);
            Height = Math.Min(1080, SystemParameters.WorkArea.Height * 0.94);
        }

        private void CopyCode_Click(object sender, RoutedEventArgs e)
        {
            var model = (TemplateLabViewModel)DataContext;
            try
            {
                Clipboard.SetText(model.IsStates ? model.StateCode : model.TemplateCode);
                model.Status = "XAML 已复制到剪贴板。";
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                model.Status = "剪贴板正在使用中，请稍后再试。";
            }
        }
    }
}
