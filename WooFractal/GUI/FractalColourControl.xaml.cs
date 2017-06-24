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
using WooFractal.Objects;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for FractalColourControl.xaml
    /// </summary>
    public partial class FractalColourControl : UserControl, IGUIUpdateable
    {
        FractalGradient _FractalColours = new FractalGradient();
        MaterialSelection _MaterialSelection;

        public FractalColourControl(FractalGradient fractalColours, MaterialSelection materialSelection)
        {
            InitializeComponent();

            _FractalColours = fractalColours;
            _MaterialSelection = materialSelection;

            SetupGUI();
        }

        public void SetupGUI()
        {
            comboBox2.SelectedIndex = _FractalColours.GetOrbitTypeIndex();
            floatEditor1.Set("Multiplier", _FractalColours._Multiplier, 0, 10, FloatEditorFlags.None, this);
            floatEditor2.Set("Offset", _FractalColours._Offset, 0, 1, FloatEditorFlags.None, this);
            floatEditor3.Set("Power", _FractalColours._Power, -2, 2, FloatEditorFlags.None, this);

            materialSelector1.Set(_MaterialSelection, _FractalColours._StartColour, this);
            materialSelector2.Set(_MaterialSelection, _FractalColours._EndColour, this);
        }

        public void GUIUpdate()
        {
            _FractalColours.SetOrbitTypeIndex(comboBox2.SelectedIndex);
            _FractalColours._Multiplier = floatEditor1.GetSliderValue();
            _FractalColours._Offset = floatEditor2.GetSliderValue();
            _FractalColours._Power = floatEditor3.GetSliderValue();

            _FractalColours._StartColour = materialSelector1.GetSelectedMaterial();
            _FractalColours._EndColour = materialSelector2.GetSelectedMaterial();

            _MaterialSelection = materialSelector1.GetMaterialSelection();

            //           _FractalColours._OrbitColoursX = orbitColourControl1.GetOrbitColours();
 //           _FractalColours._OrbitColoursY = orbitColourControl2.GetOrbitColours();
 //           _FractalColours._OrbitColoursZ = orbitColourControl3.GetOrbitColours();
 //           _FractalColours._OrbitColoursDist = orbitColourControl4.GetOrbitColours();

 //           _FractalColours._XOrbitEnabled = checkBox1.IsChecked.HasValue ? checkBox1.IsChecked.Value : false;
 //           _FractalColours._YOrbitEnabled = checkBox2.IsChecked.HasValue ? checkBox2.IsChecked.Value : false;
 //           _FractalColours._ZOrbitEnabled = checkBox3.IsChecked.HasValue ? checkBox3.IsChecked.Value : false;
 //           _FractalColours._DistOrbitEnabled = checkBox4.IsChecked.HasValue ? checkBox4.IsChecked.Value : false;

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void checkBox1_Modified(object sender, RoutedEventArgs e)
        {
            GUIUpdate();
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _FractalColours.SetOrbitTypeIndex(comboBox2.SelectedIndex);
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }
    }
}
