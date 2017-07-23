using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;
using WooFractal.Objects;

namespace WooFractal
{
    public class GradientSegment
    {
        public Material _StartColour = new Material();
        public Material _EndColour = new Material();
        public float _StartX = 0.0f;
        public float _EndX = 1.0f;

        public GradientSegment()
        {
            _StartColour._DiElectric = 0.5f;
            _StartColour._Reflectivity = new Colour(0.5);
            _StartColour._SpecularColour = new Colour(0.5);
            _StartColour._Roughness = 0.12f;
            _StartColour._DiffuseColour = new Colour(0.7, 0.2, 0.2);

            _EndColour._Reflectivity = new Colour(0.3);
            _EndColour._SpecularColour = new Colour(0.3);
            _EndColour._DiElectric = 0.5f;
            _EndColour._Roughness = 0.2f;
            _EndColour._DiffuseColour = new Colour(0.3, 0.8, 0.3);
        }

        public float Width()
        {
            return _EndX - _StartX;
        }

        public Material GetMaterial(float left)
        {
            float gradx = (left - _StartX) / Width();
            if (gradx > 1.0)
            {
                gradx = 1.0f;
            }
            Material mat = new Material();
            mat._DiElectric = _StartColour._DiElectric * (1 - gradx) + _EndColour._DiElectric * gradx;
            mat._Roughness = _StartColour._Roughness * (1 - gradx) + _EndColour._Roughness * gradx;
            mat._DiffuseColour = _StartColour._DiffuseColour * (1 - gradx) + _EndColour._DiffuseColour * gradx;
            mat._SpecularColour = _StartColour._SpecularColour * (1 - gradx) + _EndColour._SpecularColour * gradx;
            mat._Reflectivity = _StartColour._Reflectivity * (1 - gradx) + _EndColour._Reflectivity * gradx;
            return mat;
        }
        
        public void CreateElement(XElement parent)
        {
            XElement ret;

            ret = new XElement("GRADIENTSEGMENT",
                _StartColour.CreateElement("STARTCOLOUR", false),
                _EndColour.CreateElement("ENDCOLOUR", false),
                new XAttribute("startX", _StartX),
                new XAttribute("endX", _EndX));

            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadFloat(reader, "startX", ref _StartX);
            XMLHelpers.ReadFloat(reader, "endX", ref _EndX);

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "STARTCOLOUR")
                {
                    _StartColour.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ENDCOLOUR")
                {
                    _EndColour.LoadXML(reader);
                }
            }
            reader.Read();
        }
    }
    public class FractalGradient
    {
        public enum EOrbitType
        {
            x,
            y,
            z,
            w
        };

        public double _Multiplier = 1.0;
        public double _Offset = 0.0;
        public double _Power = 0.0;
        public EBlendType _BlendType = EBlendType.Linear;
        public List<GradientSegment> _GradientSegments = new List<GradientSegment>();
        public EOrbitType _OrbitType = EOrbitType.x;

        public FractalGradient()
        {
            _GradientSegments.Add(new GradientSegment());
        }

        public string GetOrbitTypeString()
        {
            switch (_OrbitType)
            {
                case EOrbitType.x:
                    return "x";
                case EOrbitType.y:
                    return "y";
                case EOrbitType.z:
                    return "z";
                case EOrbitType.w:
                    return "w";
                default:
                    return "x";
            }
        }

        public int GetOrbitTypeIndex()
        {
            switch (_OrbitType)
            {
                case EOrbitType.x:
                    return 0;
                case EOrbitType.y:
                    return 1;
                case EOrbitType.z:
                    return 2;
                case EOrbitType.w:
                    return 3;
                default:
                    return 0;
            }
        }

        public void SetOrbitTypeIndex(int index)
        {
            switch (index)
            {
                case 0:
                    _OrbitType = EOrbitType.x;
                    return;
                case 1:
                    _OrbitType = EOrbitType.y;
                    return;
                case 2:
                    _OrbitType = EOrbitType.z;
                    return;
                case 3:
                    _OrbitType = EOrbitType.w;
                    return;
                default:
                    _OrbitType = EOrbitType.x;
                    return;
            }
        }

        public FractalColourControl GetControl(MaterialSelection materialSelection)
        {
            return new FractalColourControl(this, materialSelection);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret;

            ret = new XElement("FRACTALCOLOURS",
                new XAttribute("orbitType", _OrbitType),
                new XAttribute("blendType", _BlendType),
                new XAttribute("multiplier", _Multiplier),
                new XAttribute("offset", _Offset),
                new XAttribute("power", _Power));

            for (int i = 0; i < _GradientSegments.Count(); i++)
            {
                _GradientSegments[i].CreateElement(ret);
            }

            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            _GradientSegments.Clear();
            XMLHelpers.ReadOrbitType(reader, "orbitType", ref _OrbitType);
            XMLHelpers.ReadBlendType(reader, "blendType", ref _BlendType);
            XMLHelpers.ReadDouble(reader, "multiplier", ref _Multiplier);
            XMLHelpers.ReadDouble(reader, "offset", ref _Offset);
            XMLHelpers.ReadDouble(reader, "power", ref _Power);

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "GRADIENTSEGMENT")
                {
                    GradientSegment seg = new GradientSegment();
                    seg.LoadXML(reader);
                    _GradientSegments.Add(seg);
                }
            }

            // backwards compatability
            if (_GradientSegments.Count() == 0)
            {
                _GradientSegments.Add(new GradientSegment());
            }

            reader.Read();
        }
    }
}
