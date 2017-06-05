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
            floatEditor1.Set("Aperture Size", _Parent._ApertureSize, 0.0001, 0.1, FloatEditorFlags.None, this);
            floatEditor2.Set("Field Of View", _Parent._FOV, 1, 360, FloatEditorFlags.None, this);
            floatEditor3.Set("Spherical Projection", _Parent._Spherical, 0, 1, FloatEditorFlags.None, this);
            floatEditor4.Set("Stereographic", _Parent._Stereographic, 0, 1, FloatEditorFlags.None, this);
            floatEditor5.Set("Exposure", _Parent._Exposure, 0.1, 10, FloatEditorFlags.None, this);
        }

        public void GUIUpdate()
        {
            _Parent._ApertureSize = floatEditor1.GetSliderValue();
            _Parent._FOV = floatEditor2.GetSliderValue();
            _Parent._Spherical = floatEditor3.GetSliderValue();
            _Parent._Stereographic = floatEditor4.GetSliderValue();
            _Parent._Exposure = floatEditor5.GetSliderValue();

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetCameraDirty();
        }
    }
}
