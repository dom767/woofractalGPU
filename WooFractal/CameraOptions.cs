﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace WooFractal
{
    public class CameraOptions
    {
        CameraControls _Controls;
        public UserControl GetControl()
        {
            _Controls = new CameraControls(this);
            return _Controls;
        }

        public void UpdateGUI()
        {
            _Controls.CreateGUI();
        }

        public double _FocusDepth = 1;
        public double _ApertureSize = 0.1;
        public double _FieldOfView = 40;
        public double _Spherical = 0;
        public double _Stereographic = 0;
        public Vector3 _CameraPosition = new Vector3(-0.6, 1.2, -0.6);
        public Vector3 _CameraTarget = new Vector3(0, 0.5, 0);
    }
}
