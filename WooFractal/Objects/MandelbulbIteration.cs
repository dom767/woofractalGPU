using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public class MandelbulbIteration : WooFractalIteration
    {
        public Vector3 _Rotation = new Vector3(0,0,0);
        public double _Scale = 1;
 
        public MandelbulbIteration()
        {
        }

        public MandelbulbIteration(Vector3 rotation, double scale, int repeats)
        {
            _Rotation = rotation;
            _Scale = scale;
            _Repeats = repeats;
        }
        public override UserControl GetControl()
        {
            return new MandelbulbControl(this);
        }

        public override void Compile(ref string frag)
        {
            Matrix3 rot = new Matrix3();
            rot.MakeFromRPY(_Rotation.x, _Rotation.y, _Rotation.z);
            frag += "Bulb(r, pos, origPos, scale, float("+_Scale+"), mat3("+Utils.Matrix3ToString(rot)+@"));
            DEMode = 2;";
        }

        public override string GetFractalString()
        {
            string fracstring = "fractal_mandelbulb(vec(" + _Rotation.ToString() + "), "+  _Scale.ToString() + ")\r\n";

            string repstring = "";
            for (int i = 0; i < _Repeats; i++)
            {
                repstring += fracstring;
            }

            return repstring;
        }

        public override void CreateElement(XElement parent)
        {
            XElement ret = new XElement("BULBFRACTAL",
                new XAttribute("rotation", _Rotation),
                new XAttribute("scale", _Scale),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
            return;
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadVector3(reader, "rotation", ref _Rotation);
            XMLHelpers.ReadDouble(reader, "scale", ref _Scale);
            XMLHelpers.ReadInt(reader, "repeats", ref _Repeats);
            reader.Read();
        }
    }
}
