using DocumentFormat.OpenXml.Office.Y2022.FeaturePropertyBag;
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

namespace StudyMVVM
{
    /// <summary>
    /// shapFrm.xaml 的交互逻辑
    /// </summary>
    public partial class shapFrm : Window
    {// 在窗口类定义一个标记
        private bool _isWindowLoaded = false;
        public shapFrm()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                _isWindowLoaded = true; //窗口加载完毕，标记置true
            };

            Border border = new Border();
            SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Pink);
            border.Background = solidColorBrush;
            ellipse.Fill = solidColorBrush;

           border.BorderBrush = solidColorBrush;
            Rectangle rec = new Rectangle();
            rec.Stroke = solidColorBrush;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 窗口还没加载完成，直接退出，不执行
            if (!_isWindowLoaded)
                return;
            var selectedItem = cbbCombined.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            // 取出CombinedGeometry
            var combinedGeometryTest = combinedGeometryPathTest.Data as CombinedGeometry;
            if (combinedGeometryTest == null)
                return; 

            string changeMode = selectedItem.Content.ToString(); //这里拿到纯净的 "Intersect"
            switch (changeMode)
            {
                case "Exclude":
                    combinedGeometryTest.GeometryCombineMode = GeometryCombineMode.Exclude;
                    break;
                case "Intersect":
                    combinedGeometryTest.GeometryCombineMode = GeometryCombineMode.Intersect;
                    break;
                case "Union":
                    combinedGeometryTest.GeometryCombineMode = GeometryCombineMode.Union;
                    break;
                case "Xor":
                    combinedGeometryTest.GeometryCombineMode = GeometryCombineMode.Xor;
                    break;
                default:
                    break;
            }
           
        }
    }
}
