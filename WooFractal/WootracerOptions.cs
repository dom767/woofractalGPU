using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WooFractal
{
    public class WootracerOptions
    {
        WootracerControls _Controls;
        public UserControl GetControl()
        {
            _Controls = new WootracerControls(this);
            return _Controls;
        }

        public void UpdateGUI()
        {
            _Controls.CreateGUI();
        }

        public double _Exposure = 1;
        public bool _AutoExposure = true;
        public bool _ShadowsEnabled = true;
        public bool _DoFEnabled = true;
        public bool _ReflectionsEnabled = false;
        public bool _Headlight = true;
        public bool _Colours = true;
        public bool _Progressive = false;
    }
}
