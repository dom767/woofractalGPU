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
    /// <summary>
    /// Interaction logic for FractalControl.xaml
    /// </summary>
    /// 
    public partial class FractalControl : UserControl, IGUIUpdateable
    {

        KIFSIteration _Parent;

        public FractalControl(KIFSIteration parent)
        {
            InitializeComponent();

            _Parent = parent;

            RenderSwatches();
        }

        private void RenderSingle(double value, double range, bool signed, Rectangle parent, ref Rectangle rect)
        {
            double nvalue = value / range;

            if (signed)
            {
                double controlMid = parent.Margin.Left + parent.Width * 0.5;
                double controlWidth = parent.Width * 0.5;

                if (nvalue < 0)
                {
                    Thickness marg = rect.Margin;
                    marg.Left = controlMid + nvalue * controlWidth;
                    rect.Margin = marg;
                    rect.Width = -nvalue * controlWidth;
                }
                else
                {
                    Thickness marg = rect.Margin;
                    marg.Left = controlMid;
                       rect.Margin = marg;
                    rect.Width = nvalue * controlWidth;
                }
            }
            else
            {
                rect.Width = nvalue * parent.Width;
            }
        }

        private void RenderSwatches()
        {
            wooSlider1.Set(_Parent._PreRotation.x, -Math.PI, Math.PI, this);
            wooSlider2.Set(_Parent._PreRotation.y, -Math.PI, Math.PI, this);
            wooSlider3.Set(_Parent._PreRotation.z, -Math.PI, Math.PI, this);

            wooSlider4.Set(_Parent._PostRotation.x, -Math.PI, Math.PI, this);
            wooSlider5.Set(_Parent._PostRotation.y, -Math.PI, Math.PI, this);
            wooSlider6.Set(_Parent._PostRotation.z, -Math.PI, Math.PI, this);

            wooSlider7.Set(_Parent._Offset.x, 0, 3, this);
            wooSlider8.Set(_Parent._Offset.y, 0, 3, this);
            wooSlider9.Set(_Parent._Offset.z, 0, 3, this);

            wooSlider10.Set(_Parent._Scale, 0, 4, this);

            SetName(_Parent._FractalType);

            textBox1.Text = _Parent._Repeats.ToString();
        }

        private void SetName(EFractalType fractalType)
        {
            switch (fractalType)
            {
                case EFractalType.Tetra:
                    label5.Content = "T E T R A H E D R O N";
                    break;
                case EFractalType.Menger:
                    label5.Content = "M E N G E R";
                    break;
                case EFractalType.Cube:
                    label5.Content = "C U B E";
                    break;
            }
        }

        private void SetValue(object sender, Point pos)
        {

            if ((sender as Rectangle).Name.Equals("scale1"))
                _Parent._Scale = 4 * (pos.X - 156) / 156.0;

            RenderSwatches();

            ((MainWindow)System.Windows.Application.Current.MainWindow).Compile();
        }

        public void GUIUpdate()
        {
            _Parent._PreRotation.x = wooSlider1.GetSliderValue();
            _Parent._PreRotation.y = wooSlider2.GetSliderValue();
            _Parent._PreRotation.z = wooSlider3.GetSliderValue();
            
            _Parent._PostRotation.x = wooSlider4.GetSliderValue();
            _Parent._PostRotation.y = wooSlider5.GetSliderValue();
            _Parent._PostRotation.z = wooSlider6.GetSliderValue();

            _Parent._Offset.x = wooSlider7.GetSliderValue();
            _Parent._Offset.y = wooSlider8.GetSliderValue();
            _Parent._Offset.z = wooSlider9.GetSliderValue();

            _Parent._Scale = wooSlider10.GetSliderValue();

            int repeats;
            if (int.TryParse(textBox1.Text, out repeats)
                && repeats>=1 && repeats<10)
            {
                _Parent._Repeats = repeats;
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).RemoveIteration(_Parent);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).PromoteIteration(_Parent);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DemoteIteration(_Parent);
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            GUIUpdate();
        }
    }
}
