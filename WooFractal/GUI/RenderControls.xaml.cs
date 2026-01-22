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
    /// Interaction logic for RenderControls.xaml
    /// </summary>
    public partial class RenderControls : UserControl, IGUIUpdateable
    {

        RenderOptions _Parent;
        public RenderControls(RenderOptions parent)
        {
            DataContext = this;
            InitializeComponent();

            _Parent = parent;

            RenderSliders();

            PopulateCombos();
        }

        private void PopulateCombos()
        {
            // DE modes
            comboBox3.Items.Add("Standard Mandelbulb");
            comboBox3.Items.Add("KIFS / Mandelbox");
            comboBox3.Items.Add("Last iteration");
            comboBox3.Items.Add("4-point Buddhi/Makin");

            // minimum distance mode
            comboBox4.Items.Add("Fixed");
            comboBox4.Items.Add("Screenspace");

            comboBox3.SelectedIndex = _Parent._DEMode;
            comboBox4.SelectedIndex = _Parent._DistanceMinimumMode;
        }

        public void RenderSliders()
        {
            floatEditor3.Set("Minimum Distance", _Parent._DistanceMinimum, 0, 5, FloatEditorFlags.None, this);
            floatEditor1.Set("Distance Iterations", _Parent._DistanceIterations, 1, 4096, FloatEditorFlags.Integer, this);
            floatEditor2.Set("Step Size", _Parent._StepSize, 0.01, 1.0, FloatEditorFlags.None, this);
            floatEditor4.Set("Distance Extents", _Parent._DistanceExtents, 0.5, 10.0, FloatEditorFlags.None, this);
            floatEditor5.Set("Fractal Iterations", _Parent._FractalIterationCount, 1, 250, FloatEditorFlags.Integer, this);
            floatEditor6.Set("Colour Iterations", _Parent._ColourIterationCount, 1, 250, FloatEditorFlags.Integer, this);
        }

        public void GUIUpdate()
        {
            RenderOptions old = (RenderOptions) _Parent.GetClone();
            _Parent._DistanceMinimum = floatEditor3.GetSliderValue();
            _Parent._DistanceIterations = floatEditor1.GetSliderValue();
            _Parent._StepSize = floatEditor2.GetSliderValue();
            _Parent._DistanceExtents = floatEditor4.GetSliderValue();
            _Parent._FractalIterationCount = (int)(floatEditor5.GetSliderValue() + 0.5);
            _Parent._ColourIterationCount = (int)(floatEditor6.GetSliderValue() + 0.5);

            if (_Parent._DistanceMinimum != old._DistanceMinimum
                || _Parent._DistanceIterations != old._DistanceIterations
                || _Parent._StepSize != old._StepSize
                || _Parent._FractalIterationCount != old._FractalIterationCount
                || _Parent._ColourIterationCount != old._ColourIterationCount)
                ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
            else
                ((MainWindow)System.Windows.Application.Current.MainWindow).SetCameraDirty();
        }

        private void comboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Parent._DEMode = comboBox3.SelectedIndex;
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void checkBox1_Modified(object sender, RoutedEventArgs e)
        {
            GUIUpdate();
        }

        private void comboBox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Parent._DistanceMinimumMode = comboBox4.SelectedIndex;
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }
    }
}
