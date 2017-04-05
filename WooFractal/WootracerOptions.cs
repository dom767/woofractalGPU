using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace WooFractal
{
    public class RaytracerOptions
    {
        RaytracerControls _Controls;
        public UserControl GetControl()
        {
            _Controls = new RaytracerControls(this);
            return _Controls;
        }

        public void UpdateGUI()
        {
            _Controls.CreateGUI();
        }

        public XElement CreateElement()
        {
            return new XElement("OPTIONS",
                new XAttribute("exposure", _Exposure),
                new XAttribute("autoExposure", _AutoExposure),
                new XAttribute("shadowsEnabled", _ShadowsEnabled),
                new XAttribute("dofEnabled", _DoFEnabled),
                new XAttribute("reflections", _Reflections),
                new XAttribute("headlight", _Headlight),
                new XAttribute("colours", _Colours),
                new XAttribute("progressive", _Progressive));
        }
        
        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadDouble(reader, "exposure", ref _Exposure);
            XMLHelpers.ReadBool(reader, "autoExposure", ref _AutoExposure);
            XMLHelpers.ReadBool(reader, "shadowsEnabled", ref _ShadowsEnabled);
            XMLHelpers.ReadBool(reader, "dofEnabled", ref _DoFEnabled);
            XMLHelpers.ReadInt(reader, "reflections", ref _Reflections);
            XMLHelpers.ReadBool(reader, "headlight", ref _Headlight);
            XMLHelpers.ReadBool(reader, "colours", ref _Colours);
            XMLHelpers.ReadBool(reader, "progressive", ref _Progressive);
            reader.Read();
        }

        public int GetRaysPerPixel()
        {
            return (1 + (_ShadowsEnabled?2:0)) * (1+_Reflections);
        }

        public double _Exposure = 1;
        public int _Resolution = 1;
        public bool _AutoExposure = true;
        public bool _ShadowsEnabled = true;
        public bool _DoFEnabled = true;
        public int _Reflections = 0;
        public bool _Headlight = true;
        public bool _Colours = true;
        public bool _Progressive = false;
        public int _MaxIterations = 2;
    }
}
