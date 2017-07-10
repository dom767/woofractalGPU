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
            vectorEditor1.Set("Rotation pre transform", _Parent._PreRotation, new Vector3(-Math.PI), new Vector3(Math.PI), VectorEditorFlags.Rotation, this);
            vectorEditor2.Set("Rotation post transform", _Parent._PostRotation, new Vector3(-Math.PI), new Vector3(Math.PI), VectorEditorFlags.Rotation, this);
            vectorEditor3.Set("Offset", _Parent._Offset, new Vector3(0), new Vector3(4), VectorEditorFlags.None, this);
            floatEditor1.Set("Scale", _Parent._Scale, 0, 4, FloatEditorFlags.None, this);

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
            _Parent._PreRotation = vectorEditor1.GetSliderValue();
            _Parent._PostRotation = vectorEditor2.GetSliderValue();
            _Parent._Offset = vectorEditor3.GetSliderValue();
            _Parent._Scale = floatEditor1.GetSliderValue();

            int repeats;
            if (int.TryParse(textBox1.Text, out repeats)
                && repeats>=1 && repeats<10)
            {
                _Parent._Repeats = repeats;
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetCameraDirty();
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
