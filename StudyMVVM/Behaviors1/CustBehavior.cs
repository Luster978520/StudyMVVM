using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StudyMVVM.Behaviors1
{

    /// <summary>
    /// 自定义行为（实现拖拽功能）
    /// </summary>
    public class CustBehavior : Behavior<UIElement>
    {

        /// <summary>
        /// 添加功能
        /// </summary>
        protected override void OnAttached() { 
                base.OnAttached();

            //鼠标左键按下去，鼠标移动，鼠标左键释放
            this.AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButton;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            this.AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;           
        }
        private bool IsMoving = false; //是否移动
        private Point startPoint;//起始坐标
                                 //   private Point 
        private Canvas canvas;

        /// <summary>
        /// 鼠标左键松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMoving)
            {
                //释放鼠标不能移动
                IsMoving = false;
                Mouse.Capture(null);
            }
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            //1是否点击左键，并且没有松开
            if (IsMoving)
            {
                //2移动
                //现在的坐标  //获取鼠标在画布中的坐标
                Point nowPoint = e.GetPosition(canvas);

                //AssociatedObject.SetCurrentValue(Canvas.LeftProperty,nowPoint.X);
                //AssociatedObject.SetCurrentValue(Canvas.TopProperty,nowPoint.Y);
                AssociatedObject.SetValue(Canvas.TopProperty,nowPoint.Y-startPoint.Y);
                AssociatedObject.SetValue(Canvas.LeftProperty, nowPoint.X - startPoint.X);

            }
        }

        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AssociatedObject_MouseLeftButton(object sender, MouseButtonEventArgs e)
        {
            IsMoving = true;

            //记录起始坐标
            startPoint=e.GetPosition(AssociatedObject);

            //移动元素捕获鼠标
            Mouse.Capture(AssociatedObject);

            //找画布
            if (canvas==null)
            {
                canvas = VisualTreeHelper.GetParent(AssociatedObject) as Canvas;
            }
        }

        /// <summary>
        /// 移除功能
        /// </summary>
        protected override void OnDetaching() { 
             base.OnDetaching();
            //鼠标左键按下去，鼠标移动，鼠标左键释放
            this.AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButton;
            this.AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            this.AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
        }

    }
}
