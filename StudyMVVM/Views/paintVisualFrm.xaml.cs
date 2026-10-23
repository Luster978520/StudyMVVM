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
    /// paintVisualFrm.xaml 的交互逻辑
    /// </summary>
    public partial class paintVisualFrm : Window
    {
        public paintVisualFrm()
        {
            InitializeComponent();
        }

        Point startPoint=new Point();
        private Vector clickOffset;
        private DrawingVisual selectedVisual;


        private DrawingVisual selectionSquare;//  绘制长方形
        private bool isMultiSecting=false;  //是否多选
        private Point selectionSquareTopLeft;   //矩形左上角坐标

        /// <summary>
        /// 鼠标左键单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawingSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mouseLeftPoint = e.GetPosition(drawingSurface);
            startPoint = mouseLeftPoint;             
            if (cmdMove.IsChecked==true)
            {
                
                //根据坐标获取小方格
                DrawingVisual visual = drawingSurface.GetVisual(mouseLeftPoint);
                //如果获取到了小方格
                if (visual!=null)
                {
                    isMove = true;
                    Point topLeftCorner = new Point(visual.ContentBounds.TopLeft.X + drawingPen.Thickness / 2, visual.ContentBounds.TopLeft.Y + drawingPen.Thickness / 2);
                    DrawSquare(visual,topLeftCorner,true);
                    clickOffset = topLeftCorner - mouseLeftPoint;
                }

                //设置能移动
                //计算鼠标的位置和方格左上角的偏移量
                //是否是第一次移动  如果不是清除之前移动的小方格的样式
                if (selectedVisual!=null &&selectedVisual!=visual)
                {
                    //清除小方格的样式
                    clearSelection();
                }
                selectedVisual = visual;

            }
            if (cmdAdd.IsChecked == true)
            {

                DrawingVisual visual = new DrawingVisual();

                this.DrawSquare(visual, mouseLeftPoint,false);

                //添加画布
                drawingSurface.AddVisual(visual);
            }
            if (cmdDelete.IsChecked == true)
            {
                //删除方格，根据鼠标的选中的坐标，找到对应的方格
                DrawingVisual visual = drawingSurface.GetVisual(mouseLeftPoint);
                if (visual!=null)
                {
                    drawingSurface.DeletedVisual(visual);
                }

                //如果找到的就删除

                //canvas提供两个方法 1、找方格 2、删除方格

            }
            if (CmdSelectmultp.IsChecked == true)
            {

            }
        }


        /// <summary>
        /// 通过重绘小方格清楚以前的样式
        /// </summary>
        private void clearSelection()
        {
            Point topLeftCorner = new Point(
                selectedVisual.ContentBounds.TopLeft.X + drawingPen.Thickness / 2,
                selectedVisual.ContentBounds.TopLeft.Y + drawingPen.Thickness / 2);
                 DrawSquare(selectedVisual,topLeftCorner,false);
                 selectedVisual = null;

        }

        /// <summary>
        /// 画出小方格
        /// </summary>
        /// <param name="visual">可视化</param>
        /// <param name="topLeftCorner">坐标</param>
        /// <param name="isSelected">画完以后，小方格是否标记为已选择的样式</param>
        ///
        private Brush drawingBrush = Brushes.Red;
        private Pen drawingPen = new Pen(Brushes.SteelBlue,3);
        private Size drawingSize = new Size(30,30);

        private Brush drawingBrushSelect;

        private void DrawSquare(DrawingVisual visual,Point topLeftCorner,bool isSelected) {           
            using (DrawingContext dc = visual.RenderOpen()) //using 自动释放
            {
                drawingBrushSelect = Brushes.LightGoldenrodYellow;
                if (isSelected)
                {
                    drawingBrushSelect = drawingBrush;
                }        
                //画东西
                dc.DrawRectangle(drawingBrushSelect, drawingPen, new Rect(topLeftCorner, drawingSize));               
            }
        }

        private bool isMove = false;

        private void drawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMove)
            {
                Point pointDragged = e.GetPosition(drawingSurface) + clickOffset;
                DrawSquare(selectedVisual, pointDragged, true);
            }
        }

        private void drawingSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMove=false;
        }
    }
}
