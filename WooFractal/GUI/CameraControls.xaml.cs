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
    /// Interaction logic for CameraControls.xaml
    /// </summary>
    public partial class CameraControls : UserControl, IGUIUpdateable
    {
        Camera _Parent;
        public CameraControls(Camera camera)
        {
            InitializeComponent();

            _Parent = camera;

            CreateGUI();
        }

        public void CreateGUI()
        {
            wooSlider2.Set(_Parent._ApertureSize, 0.0001, 0.1, this);
            wooSlider3.Set(_Parent._FOV, 1, 360, this);
            wooSlider4.Set(_Parent._Spherical, 0, 1, this);
            wooSlider5.Set(_Parent._Stereographic, 0, 1, this);
        }

        public void GUIUpdate()
        {
            _Parent._ApertureSize = wooSlider2.GetSliderValue();
            _Parent._FOV = wooSlider3.GetSliderValue();
            _Parent._Spherical = wooSlider4.GetSliderValue();
            _Parent._Stereographic = wooSlider5.GetSliderValue();

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }
    }
}
