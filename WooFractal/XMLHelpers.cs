using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    class XMLHelpers
    {
        public static void ReadFloat(XmlReader reader, string name, ref float param)
        {
            string attr = reader.GetAttribute(name);
            if (attr != null)
                float.TryParse(attr, out param);
        }

        public static void ReadDouble(XmlReader reader, string name, ref double param)
        {
            string attr = reader.GetAttribute(name);
            if (attr != null)
                double.TryParse(attr, out param);
        }

        public static void ReadInt(XmlReader reader, string name, ref int param)
        {
            string attr = reader.GetAttribute(name);
            if (attr != null)
                int.TryParse(attr, out param);
        }

        public static void ReadBool(XmlReader reader, string name, ref bool param)
        {
            string attr = reader.GetAttribute(name);
            if (attr != null)
                bool.TryParse(attr, out param);
        }

        public static void ReadFractalType(XmlReader reader, string name, ref EFractalType fractalType)
        {
            string attr = reader.GetAttribute(name);
            if (attr != null)
            {
                fractalType = (EFractalType)Enum.Parse(typeof(EFractalType), attr);
            }
        }

        public static void ReadBlendType(XmlReader reader, string name, ref EBlendType blendType)
        {
            string attr = reader.GetAttribute(name);
            if (attr != null)
            {
                blendType = (EBlendType)Enum.Parse(typeof(EBlendType), attr);
            }
        }

        public static void ReadColour(XmlReader reader, ref Colour colour)
        {
            string attr = reader.GetAttribute("colour");
            if (attr != null)
            {
                string[] rgb = attr.Split(',');
                colour._Red = float.Parse(rgb[0]);
                colour._Green = float.Parse(rgb[1]);
                colour._Blue = float.Parse(rgb[2]);
            }
        }

        public static void ReadVector3(XmlReader reader, string name, ref Vector3 vector)
        {
            string attr = reader.GetAttribute(name);
            if (attr != null)
            {
                string[] xyz = attr.Split(',');
                vector.x = double.Parse(xyz[0]);
                vector.y = double.Parse(xyz[1]);
                vector.z = double.Parse(xyz[2]);
            }
        }
    }
}
