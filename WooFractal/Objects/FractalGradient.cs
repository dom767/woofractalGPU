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
        public Material _StartColour = new Material();
        public Material _EndColour = new Material();
        public EOrbitType _OrbitType = EOrbitType.x;

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

        public UserControl GetControl(MaterialSelection materialSelection)
        {
            return new FractalColourControl(this, materialSelection);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret;

            ret = new XElement("FRACTALCOLOURS",
                new XAttribute("orbitType", _OrbitType),
                new XAttribute("blendType", _BlendType),
                _StartColour.CreateElement("STARTCOLOUR", false),
                _EndColour.CreateElement("ENDCOLOUR", false),
                new XAttribute("multiplier", _Multiplier),
                new XAttribute("offset", _Offset),
                new XAttribute("power", _Power));

            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadOrbitType(reader, "orbitType", ref _OrbitType);
            XMLHelpers.ReadBlendType(reader, "blendType", ref _BlendType);
            XMLHelpers.ReadDouble(reader, "multiplier", ref _Multiplier);
            XMLHelpers.ReadDouble(reader, "offset", ref _Offset);
            XMLHelpers.ReadDouble(reader, "power", ref _Power);

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
}
