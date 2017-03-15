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
    public class Script
    {
        public Script(string name, string desc)
        {
            _Name = name;
            _Description = desc;
        }
        public string _Name;
        public string _Description;
    };

    public class RenderOptions
    {
        public double _DistanceMinimum = 2;
        public double _DistanceIterations = 40;
        public double _StepSize = 0.7;
        public double _DistanceExtents = 1;
        public int _FractalIterationCount = 15;
        public int _ColourIterationCount = 15;
        public int _DEMode = 2;
        public int _Background = 0;
        public int _Lighting = 0;
        public bool _Headlight = false;
        public double _HeadLightStrength = 1.0;

        public RenderOptions()
        {
            PopulateScenes();
        }

        public UserControl GetControl()
        {
            return new RenderControls(this);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("RENDEROPTIONS",
                new XAttribute("distanceMinimum", _DistanceMinimum),
                new XAttribute("distanceIterations", _DistanceIterations),
                new XAttribute("stepSize", _StepSize),
                new XAttribute("distanceExtents", _DistanceExtents),
                new XAttribute("fractalIterationCount", _FractalIterationCount),
                new XAttribute("colourIterationCount", _ColourIterationCount),
                new XAttribute("deMode", _DEMode),
                new XAttribute("background", _Background),
                new XAttribute("lighting", _Lighting),
                new XAttribute("headlightEnabled", _Headlight),
                new XAttribute("headlightStrength", _HeadLightStrength));
            parent.Add(ret);
        }

        public List<Script> _Backgrounds = new List<Script>();
        public List<Script> _LightingEnvironments = new List<Script>();

        private void PopulateScenesType(string sceneType, ref List<Script> scriptsList)
        {
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\" + sceneType;
            if (System.IO.Directory.Exists(store))
            {
                var scripts = System.IO.Directory.EnumerateFiles(store, "*.frag");
                foreach (string scriptName in scripts)
                {
                    string shortName = scriptName.Substring(0, scriptName.LastIndexOf('.'));
                    int slashIdx = shortName.LastIndexOf('\\');
                    shortName = shortName.Substring(slashIdx + 1, shortName.Length - (slashIdx + 1));

                    string sceneDesc = "";
                    if (System.IO.File.Exists(scriptName))
                    {
                        StreamReader sr = new StreamReader(scriptName);
                        sceneDesc = sr.ReadToEnd();
                        sr.Close();
                    }

                    scriptsList.Add(new Script(shortName, sceneDesc));
                }
            }
        }

        private void PopulateScenes()
        {
            PopulateScenesType("backgrounds", ref _Backgrounds);
//            PopulateScenesType("lighting", ref _LightingEnvironments);
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadDouble(reader, "distanceMinimum", ref _DistanceMinimum);
            XMLHelpers.ReadDouble(reader, "distanceIterations", ref _DistanceIterations);
            XMLHelpers.ReadDouble(reader, "stepSize", ref _StepSize);
            XMLHelpers.ReadDouble(reader, "distanceExtents", ref _DistanceExtents);
            XMLHelpers.ReadInt(reader, "fractalIterationCount", ref _FractalIterationCount);
            XMLHelpers.ReadInt(reader, "colourIterationCount", ref _ColourIterationCount);
            XMLHelpers.ReadInt(reader, "deMode", ref _DEMode);
            XMLHelpers.ReadInt(reader, "background", ref _Background);
            XMLHelpers.ReadInt(reader, "lighting", ref _Lighting);
            XMLHelpers.ReadBool(reader, "headlightEnabled", ref _Headlight);
            XMLHelpers.ReadDouble(reader, "headlightStrength", ref _HeadLightStrength);
        }
    }
}
