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
    /// Interaction logic for EnvironmentControls.xaml
    /// </summary>
    public partial class EnvironmentControls : UserControl, IGUIUpdateable
    {

        RenderOptions _Parent;
        public EnvironmentControls(RenderOptions parent)
        {
            DataContext = this;
            InitializeComponent();

            _Parent = parent;

            RenderSliders();

            PopulateCombos();
        }

        private void PopulateCombos()
        {
            // background
            foreach (Script script in _Parent._Backgrounds)
            {
                comboBox1.Items.Add(script._Name);
            }

            // lighting environments
            foreach (Script script in _Parent._LightingEnvironments)
            {
                comboBox2.Items.Add(script._Name);
            }
        }

        public void RenderSliders()
        {
            floatEditor1.Set("Sun Height", _Parent._SunHeight, 0, 90, FloatEditorFlags.None, this);
            floatEditor2.Set("Sun Direction", _Parent._SunDirection, 0, 360, FloatEditorFlags.None, this);
            floatEditor3.Set("Headlight", _Parent._HeadLightStrength, 0, 2, FloatEditorFlags.None, this);
            floatEditor4.Set("Fog Strength", _Parent._FogStrength, 0, 2, FloatEditorFlags.None, this);
            floatEditor5.Set("Fog Shadow Samples", _Parent._FogSamples, 0, 8, FloatEditorFlags.Integer, this);
            colourSelector1.Set(_Parent._FogColour, this);
            checkBox1.IsChecked = _Parent._Headlight;
        }

        public void GUIUpdate()
        {
            RenderOptions old = (RenderOptions) _Parent.GetClone();
            _Parent._SunHeight = floatEditor1.GetSliderValue();
            _Parent._SunDirection = floatEditor2.GetSliderValue();
            _Parent._HeadLightStrength = floatEditor3.GetSliderValue();
            _Parent._Headlight = checkBox1.IsChecked.HasValue ? checkBox1.IsChecked.Value : false;
            _Parent._FogStrength = floatEditor4.GetSliderValue();
            _Parent._FogSamples = (int)(floatEditor5.GetSliderValue()+0.5);
            _Parent._FogColour = colourSelector1.GetColour();

            if (_Parent._HeadLightStrength != old._HeadLightStrength
                || _Parent._Headlight != old._Headlight)
                ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
            else
                ((MainWindow)System.Windows.Application.Current.MainWindow).SetCameraDirty();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Parent._Background = comboBox1.SelectedIndex;
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Parent._Lighting = comboBox2.SelectedIndex;
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void checkBox1_Modified(object sender, RoutedEventArgs e)
        {
            GUIUpdate();
        }
    }
}
