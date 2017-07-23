using System;
using System.Collections.Generic;
using GlmNet;
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
        public int _DistanceMinimumMode = 1;
        public double _DistanceIterations = 100;
        public double _StepSize = 0.7;
        public double _DistanceExtents = 1;
        public int _FractalIterationCount = 15;
        public int _ColourIterationCount = 15;
        public int _DEMode = 2;
        public int _Background = 1;
        public int _Lighting = 0;
        public double _SunHeight = 50; // degrees
        public double _SunDirection = 225; // degrees
        public bool _Headlight = false;
        public double _HeadLightStrength = 1.0;
        public double _FogStrength = 0.0;
        public Colour _FogColour = new Colour(0.7, 0.8, 0.9);
        public int _FogSamples = 1;

        public RenderOptions GetClone()
        {
            return (RenderOptions)this.MemberwiseClone();
        }

        public RenderOptions()
        {
            PopulateScenes();
        }

        public vec3 GetSunVec3()
        {
            return new vec3((float)(Math.Sin(Math.PI*_SunHeight/180) * Math.Cos(Math.PI*_SunDirection/180)),
                (float)(Math.Cos(Math.PI * _SunHeight / 180)),
                (float)(Math.Sin(Math.PI * _SunHeight / 180) * Math.Sin(Math.PI * _SunDirection / 180)));
        }

        public UserControl GetControl()
        {
            return new RenderControls(this);
        }

        public UserControl GetEnvironmentControl()
        {
            return new EnvironmentControls(this);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("RENDEROPTIONS",
                new XAttribute("distanceMinimum", _DistanceMinimum),
                new XAttribute("distanceMinimumMode", _DistanceMinimumMode),
                new XAttribute("distanceIterations", _DistanceIterations),
                new XAttribute("stepSize", _StepSize),
                new XAttribute("distanceExtents", _DistanceExtents),
                new XAttribute("fractalIterationCount", _FractalIterationCount),
                new XAttribute("colourIterationCount", _ColourIterationCount),
                new XAttribute("deMode", _DEMode),
                new XAttribute("background", _Background),
                new XAttribute("lighting", _Lighting),
                new XAttribute("sunHeight", _SunHeight),
                new XAttribute("sunDirection", _SunDirection),
                new XAttribute("headlightEnabled", _Headlight),
                new XAttribute("headlightStrength", _HeadLightStrength),
                new XAttribute("fogStrength", _FogStrength),
                new XAttribute("fogColour", _FogColour),
                new XAttribute("fogSamples", _FogSamples));
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
            XMLHelpers.ReadInt(reader, "distanceMinimumMode", ref _DistanceMinimumMode);
            XMLHelpers.ReadDouble(reader, "distanceIterations", ref _DistanceIterations);
            XMLHelpers.ReadDouble(reader, "stepSize", ref _StepSize);
            XMLHelpers.ReadDouble(reader, "distanceExtents", ref _DistanceExtents);
            XMLHelpers.ReadInt(reader, "fractalIterationCount", ref _FractalIterationCount);
            XMLHelpers.ReadInt(reader, "colourIterationCount", ref _ColourIterationCount);
            XMLHelpers.ReadInt(reader, "deMode", ref _DEMode);
            XMLHelpers.ReadInt(reader, "background", ref _Background);
            XMLHelpers.ReadInt(reader, "lighting", ref _Lighting);
            XMLHelpers.ReadDouble(reader, "sunHeight", ref _SunHeight);
            XMLHelpers.ReadDouble(reader, "sunDirection", ref _SunDirection);
            XMLHelpers.ReadBool(reader, "headlightEnabled", ref _Headlight);
            XMLHelpers.ReadDouble(reader, "headlightStrength", ref _HeadLightStrength);
            XMLHelpers.ReadDouble(reader, "fogStrength", ref _FogStrength);
            XMLHelpers.ReadColour(reader, "fogColour", ref _FogColour);
            XMLHelpers.ReadInt(reader, "fogSamples", ref _FogSamples);
        }
    }
}
