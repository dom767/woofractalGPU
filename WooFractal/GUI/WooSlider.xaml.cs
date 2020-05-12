using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WooFractal
{
    public interface IGUIUpdateable
    {
        void GUIUpdate();
    }
    /// <summary>
    /// Interaction logic for WooSlider.xaml
    /// </summary>
    public partial class WooSlider : UserControl
    {
        public WooSlider()
        {
            InitializeComponent();
        }

        double _Value;
        double _Min;
        double _Max;
        IGUIUpdateable _GUIUpdateTarget;

        public void Set(double value, double min, double max, IGUIUpdateable guiTarget)
        {
            _Min = min;
            _Max = max;
            SetValue(value);
            _GUIUpdateTarget = guiTarget;
        }

        public double GetSliderValue()
        {
            if (_Value > -0.000000000001 && _Value < 0.00000000000001)
                return 0.0;
            else
                return _Value;
        }

        public void SetSliderValue(double value, bool updateGUI)
        {
            SetValue(value);
            ValueUpdated(updateGUI);
        }

        bool _ValueDrag;
        Point _LastPos;

        private void rectangle1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_ValueDrag)
            {
                System.Console.WriteLine("wspmlbup");
                Mouse.Capture(null);
                Point _NewPos = e.GetPosition(this);
                double delta = _NewPos.X - _LastPos.X;
                SetValue(_Value + (_Max - _Min) * delta / this.ActualWidth);
                ValueUpdated(true);
                _ValueDrag = false;
            }
        }

        public void ValueUpdated(bool updateGUI)
        {
            double range = _Max - _Min;

            grid.Width = this.ActualWidth;
            rectangle1.Width = this.ActualWidth;

            if (_Min<0 && _Max>0)
            {
                double controlMid = this.ActualWidth * (-_Min / range);
                double controlWidth = this.ActualWidth;

                if (_Value < 0)
                {
                    Thickness marg = rectangle2.Margin;
                    marg.Left = controlMid + _Value * controlWidth / range;
                    rectangle2.Margin = marg;
                    rectangle2.Width = -_Value * controlWidth / range;
                }
                else
                {
                    Thickness marg = rectangle2.Margin;
                    marg.Left = controlMid;
                    rectangle2.Margin = marg;
                    rectangle2.Width = _Value * controlWidth / range;
                }
            }
            else
            {
                Thickness marg = rectangle2.Margin;
                marg.Left = 0;
                rectangle2.Margin = marg;
                rectangle2.Width = (_Value-_Min) * this.ActualWidth / range;
            }

            if (updateGUI)
            {
                if (_GUIUpdateTarget != null)
                    _GUIUpdateTarget.GUIUpdate();
            }
        }

        private void rectangle1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as System.Windows.IInputElement);
            _ValueDrag = true;
            _LastPos = e.GetPosition(this);
            double newValue = _Min + ((_Max - _Min) * _LastPos.X / this.ActualWidth);
            double delta = newValue - _Value;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                delta *= 0.02;
            }
            SetValue(_Value + delta);

            ValueUpdated(true);
        }

        private void SetValue(double value)
        {
            _Value = value;
            if (_Value < _Min) _Value = _Min;
            if (_Value > _Max) _Value = _Max;
        }

        private void rectangle1_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_ValueDrag)
            {
                Point _NewPos = e.GetPosition(this);
                double delta = _NewPos.X - _LastPos.X;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    delta *= 0.02;
                }
                SetValue(_Value + (_Max - _Min) * delta / this.ActualWidth);
                _LastPos = _NewPos;
                ValueUpdated(true);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ValueUpdated(false);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ValueUpdated(false);
        }
    }
}
