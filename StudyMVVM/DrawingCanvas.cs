using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StudyMVVM.CustCanvans
{

    /// <summary>
    /// 自定义控件（Canvas控件）
    /// </summary>
    public class DrawingCanvas : Canvas
    {
        /// <summary>
        /// 存放小方格
        /// </summary>
        private List<Visual> visuals = new List<Visual>();
        //  protected override int VisualChildrenCount => visuals.Count;

        /// <summary>
        /// canvas 子元素的个数
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }


        /// <summary>
        /// 根据索引获取某个子元素
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index < visuals.Count)
            {
                return visuals[index];
            }
            return null;
        }

        /// <summary>
        /// 添加子元素
        /// </summary>
        /// <param name="visual">可视化元素</param>
        public void AddVisual(Visual visual) {
            visuals.Add(visual); //添加到几何中

            base.AddLogicalChild(visual); //添加到逻辑树里面
            base.AddVisualChild(visual); //添加到视觉树中里面
        }

        /// <summary>
        /// 根据坐标找到可视化对象（小方格）
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public DrawingVisual GetVisual(Point point) { 
            var hitTestResult =VisualTreeHelper.HitTest(this,point);
            return hitTestResult.VisualHit as DrawingVisual;
        }

        public void DeletedVisual(Visual visual) {        
            visuals.Remove(visual);
            base.RemoveLogicalChild(visual);
            base.RemoveVisualChild(visual);
        
        }

        }
}
