using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudyMVVM.Views
{
    /// <summary>
    /// PalyFrm.xaml 的交互逻辑
    /// </summary>
    public partial class PalyFrm : Window
    {
        public PalyFrm()
        {
            InitializeComponent();
        }

        private void startStoryboard_CurrentGlobalSpeedInvalidated(object sender, EventArgs e)
        {

        }

        private void startStoryboard_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            Debug.WriteLine(sender.GetType().FullName);
            if (sender is Clock clock)
            {
                if (clock.CurrentProgress == null)
                {
                    ProgressBar1.Value = 0;
                    txtPlaybackStatus.Text = "停止";

                }
                else
                {
                    ProgressBar1.Value = clock.CurrentProgress.Value;
                    txtPlaybackStatus.Text = clock.CurrentProgress.Value.ToString("P1");
                    if (clock.CurrentState== ClockState.Filling)
                    {
                        txtPlaybackStatus.Text = "执行完成" + clock.CurrentProgress.Value.ToString("P1");
                    }
                    if (clock.CurrentState == ClockState.Stopped)
                    {
                        txtPlaybackStatus.Text = "重新开始";
                    }
                   

                }
                //switch (clock.CurrentState)
                //{
                //    case ClockState.Active:
                //        txtPlaybackStatus.Text = "正在执行";

                //        break;
                //    case ClockState.Filling:
                //        txtPlaybackStatus.Text = "执行完成";
                //        break;
                //    case ClockState.Stopped:  
                //        txtPlaybackStatus.Text = "重新开始";
                //        break;
                //    default:
                //        txtPlaybackStatus.Text = "错误";
                //        break;
                //}
               
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void sldPlaybackSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fadeStoryboarBegin.Storyboard.SetSpeedRatio(this,e.NewValue);
        }
    }
}
