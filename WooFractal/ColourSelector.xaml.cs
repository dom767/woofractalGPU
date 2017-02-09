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
    /// Interaction logic for ColourSelector.xaml
    /// </summary>
    public partial class ColourSelector : UserControl, IGUIUpdateable
    {
        public ColourSelector()
        {
            InitializeComponent();
        }

        IGUIUpdateable _GUIUpdateTarget;
        Colour _Colour;
        double _Maximum;

        public void Set(Colour colour, double max, IGUIUpdateable guiUpdateTarget)
        {
            _GUIUpdateTarget = guiUpdateTarget;
            _Colour = colour;
            _Maximum = max;

            RenderSliders();

            UpdateSwatch();
        }

        private void RenderSliders()
        {
            wooSlider1.Set(_Colour._Red, 0, _Maximum, this);
            wooSlider2.Set(_Colour._Green, 0, _Maximum, this);
            wooSlider3.Set(_Colour._Blue, 0, _Maximum, this);
        }

        public void GUIUpdate()
        {
            _Colour._Red = wooSlider1.GetSliderValue();
            _Colour._Green = wooSlider2.GetSliderValue();
            _Colour._Blue = wooSlider3.GetSliderValue();

            _GUIUpdateTarget.GUIUpdate();

            UpdateSwatch();
        }

        private void UpdateSwatch()
        {
        }

        public Colour GetColour()
        {
            return _Colour;
        }
    }
}
