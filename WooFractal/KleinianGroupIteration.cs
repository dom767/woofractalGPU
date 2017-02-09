using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public class KleinianGroupIteration : WooFractalIteration
    {
        public Vector3 _Rotation = new Vector3(0, 0, 0);
        public double _Scale = 1;
        public double _Multiplier = 0.5;

        public KleinianGroupIteration()
        {
        }

        public KleinianGroupIteration(Vector3 rotation, double scale, double multiplier, int repeats)
        {
            _Rotation = rotation;
            _Scale = scale;
            _Multiplier = multiplier;
            _Repeats = repeats;
        }

        public override UserControl GetControl()
        {
            return new KleinianGroupControl(this);
        }

        public override void Compile(ref string frag)
        {
            Matrix3 rot = new Matrix3();
            rot.MakeFromRPY(_Rotation.x, _Rotation.y, _Rotation.z);
            frag += "Kleinian(pos, origPos, scale, " + _Scale + ", mat3(" + Utils.Matrix3ToString(rot) + "), " + _Multiplier + @");
            DEMode = 3;";
        }

        public override string GetFractalString()
        {
            string fracstring = "fractal_kleiniangroup(vec(" + _Rotation.ToString() + "), " + _Scale.ToString() + ", "+_Multiplier.ToString()+")\r\n";

            string repstring = "";
            for (int i = 0; i < _Repeats; i++)
            {
                repstring += fracstring;
            }

            return repstring;
        }

        public override void CreateElement(XElement parent)
        {
            XElement ret = new XElement("KLEINIANGROUP",
                new XAttribute("rotation", _Rotation),
                new XAttribute("scale", _Scale),
                new XAttribute("multiplier", _Multiplier),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
            return;
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadVector3(reader, "rotation", ref _Rotation);
            XMLHelpers.ReadDouble(reader, "scale", ref _Scale);
            XMLHelpers.ReadDouble(reader, "multiplier", ref _Multiplier);
            XMLHelpers.ReadInt(reader, "repeats", ref _Repeats);
            reader.Read();
        }
    }
}
