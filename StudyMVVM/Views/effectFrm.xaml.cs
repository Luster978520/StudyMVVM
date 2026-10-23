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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudyMVVM.Views
{
    /// <summary>
    /// effectFrm.xaml 的交互逻辑
    /// </summary>
    public partial class effectFrm : Window
    {
        public effectFrm()
        {
            InitializeComponent();
            TestEffectBtn.Effect = btnEffect();
        }

        private DropShadowEffect btnEffect() {
           
           var shadow = new DropShadowEffect
           {
               Color = Colors.Blue,
               BlurRadius = 20,
               ShadowDepth = 5,
               Direction = 315,
               Opacity = 0.8
           };
            return shadow;

        }

        private void BeginStoryboard_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation widthEAmination = new DoubleAnimation();  //动画 插值动画From To Duration   By上一次的基础上的增长值
            widthEAmination.From = 200;
            widthEAmination.To = 500;
            //widthEAmination.DecelerationRatio = 2.5;
            widthEAmination.SpeedRatio = 2.5;
            // widthEAmination.Duration = TimeSpan.FromSeconds(2);  //duration 有一个隐式转换功能
            //widthEAmination.RepeatBehavior = new RepeatBehavior(2); //重复次数
            widthEAmination.RepeatBehavior = new RepeatBehavior(TimeSpan.FromSeconds(5));
            widthEAmination.Completed += WidthEAmination_Completed;
            btnTestAnimation.BeginAnimation(Button.WidthProperty, widthEAmination);
            
        }

        /// <summary>
        /// 动画完成之后需要完成的业务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void WidthEAmination_Completed(object sender, EventArgs e)
        {
            btnTestAnimation.BeginAnimation(Button.WidthProperty,null);
        }
    }
}
