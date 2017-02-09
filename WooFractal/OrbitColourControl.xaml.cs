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
    /// Interaction logic for OrbitColourControl.xaml
    /// </summary>
    public partial class OrbitColourControl : UserControl, IGUIUpdateable
    {
        OrbitColours _OrbitColours;
        IGUIUpdateable _GUIUpdateable;
        bool _Setup = false;

        public OrbitColourControl()
        {
            InitializeComponent();
        }

        public void SetOrbitColours(OrbitColours orbitColours, IGUIUpdateable guiUpdateable)
        {
            _OrbitColours = orbitColours;
            _GUIUpdateable = guiUpdateable;

            SetupGUI();
        }

        public OrbitColours GetOrbitColours()
        {
            return _OrbitColours;
        }

        public void SetupGUI()
        {
            materialControl1.SetMaterial(_OrbitColours._StartColour, this);
            materialControl2.SetMaterial(_OrbitColours._EndColour, this);
            wooSlider1.Set(_OrbitColours._Multiplier, 0, 10, this);
            switch (_OrbitColours._BlendType)
                {
                case EBlendType.Linear:
                    comboBox1.SelectedIndex = 0;
                    break;
                case EBlendType.Chop:
                    comboBox1.SelectedIndex = 1;
                    break;
            }
            wooSlider2.Set(_OrbitColours._Power, -2, 2, this);
            wooSlider3.Set(_OrbitColours._Offset, 0, 1, this);

            _Setup = true;
        }

        public void GUIUpdate()
        {
            _OrbitColours._StartColour = materialControl1.GetMaterial();
            _OrbitColours._EndColour = materialControl2.GetMaterial();
            _OrbitColours._Multiplier = wooSlider1.GetSliderValue();
            _OrbitColours._Power = wooSlider2.GetSliderValue();
            _OrbitColours._Offset = wooSlider3.GetSliderValue();
            _GUIUpdateable.GUIUpdate();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_Setup)
                return;
            if (_OrbitColours == null)
                return;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    _OrbitColours._BlendType = EBlendType.Linear;
                    break;
                case 1:
                    _OrbitColours._BlendType = EBlendType.Chop;
                    break;
            }
            GUIUpdate();
        }
    }
}
