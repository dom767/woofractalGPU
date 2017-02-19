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
    /// Interaction logic for FractalColourControl.xaml
    /// </summary>
    public partial class FractalColourControl : UserControl, IGUIUpdateable
    {
        FractalColours _FractalColours;

        public FractalColourControl(FractalColours fractalColours)
        {
            InitializeComponent();

            _FractalColours = fractalColours;
            orbitColourControl1.SetOrbitColours(_FractalColours._OrbitColoursX, this);
            orbitColourControl2.SetOrbitColours(_FractalColours._OrbitColoursY, this);
            orbitColourControl3.SetOrbitColours(_FractalColours._OrbitColoursZ, this);
            orbitColourControl4.SetOrbitColours(_FractalColours._OrbitColoursDist, this);
            checkBox1.IsChecked = _FractalColours._XOrbitEnabled;
            checkBox2.IsChecked = _FractalColours._YOrbitEnabled;
            checkBox3.IsChecked = _FractalColours._ZOrbitEnabled;
            checkBox4.IsChecked = _FractalColours._DistOrbitEnabled;
        }

        public void GUIUpdate()
        {
            _FractalColours._OrbitColoursX = orbitColourControl1.GetOrbitColours();
            _FractalColours._OrbitColoursY = orbitColourControl2.GetOrbitColours();
            _FractalColours._OrbitColoursZ = orbitColourControl3.GetOrbitColours();
            _FractalColours._OrbitColoursDist = orbitColourControl4.GetOrbitColours();

            _FractalColours._XOrbitEnabled = checkBox1.IsChecked.HasValue ? checkBox1.IsChecked.Value : false;
            _FractalColours._YOrbitEnabled = checkBox2.IsChecked.HasValue ? checkBox2.IsChecked.Value : false;
            _FractalColours._ZOrbitEnabled = checkBox3.IsChecked.HasValue ? checkBox3.IsChecked.Value : false;
            _FractalColours._DistOrbitEnabled = checkBox4.IsChecked.HasValue ? checkBox4.IsChecked.Value : false;

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void checkBox1_Modified(object sender, RoutedEventArgs e)
        {
            GUIUpdate();
        }
    }
}
