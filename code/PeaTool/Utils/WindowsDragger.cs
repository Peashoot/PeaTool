using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PeaTool.Utils
{
    public class WindowsDragger
    {
        /// <summary>
        /// 要拖动的窗体
        /// </summary>
        private readonly Window _window;
        /// <summary>
        /// 有效的拖动元素
        /// </summary>
        private readonly FrameworkElement _element;
        /// <summary>
        /// 上一次的相对位置
        /// </summary>
        private Point oldPos;
        /// <summary>
        /// 鼠标按下时的相对位置
        /// </summary>
        private Point _dragDelta;
        /// <summary>
        /// 是否启用拖动
        /// </summary>
        public bool Enabled;
        public WindowsDragger(Window window, FrameworkElement element = null, bool enabled = true)
        {
            Enabled = enabled;
            _window = window;
            _element = element ?? window;
            _element.MouseLeftButtonDown += MouseLeftButtonDown;
            _element.MouseLeftButtonUp += MouseLeftButtonUp;
            _element.MouseMove += MouseMove;
        }
        /// <summary>
        /// 鼠标按下，开始拖动
        /// </summary>
        public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Enabled)
            {
                _dragDelta = e.GetPosition(_window);
                oldPos = _dragDelta;
                Mouse.Capture(_element);
            }
        }
        /// <summary>
        /// 鼠标移动，进行拖动
        /// </summary>
        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (Equals(_element, Mouse.Captured))
            {
                // 窗体历史位置不变，相对位置改变时需要移动窗体
                var relativePos = e.GetPosition(_window);
                if (oldPos.X == relativePos.X && oldPos.Y == relativePos.Y) return;
                oldPos = relativePos;
                var pos = _window.PointToScreen(relativePos);
                var verifiedPos = CoerceWindowBound(pos - _dragDelta);
                _window.Left = verifiedPos.X;
                _window.Top = verifiedPos.Y;
            }
        }
        /// <summary>
        /// 鼠标抬起，结束拖动
        /// </summary>
        public void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Equals(_element, Mouse.Captured))
                Mouse.Capture(null);
        }
        /// <summary>
        /// 计算应该停放的位置
        /// </summary>
        private Vector CoerceWindowBound(Vector newPoint)
        {
            // Snap to the current desktop border
            var wa = SystemParameters.WorkArea;
            if (newPoint.X < wa.Top) newPoint.X = wa.Top;
            if (newPoint.Y < wa.Left) newPoint.Y = wa.Left;
            if (_window.Width + newPoint.X > wa.Right) newPoint.X = wa.Right - _window.Width;
            if (_window.Height + newPoint.Y > wa.Bottom) newPoint.Y = wa.Bottom - _window.Height;
            return newPoint;
        }
    }
}
