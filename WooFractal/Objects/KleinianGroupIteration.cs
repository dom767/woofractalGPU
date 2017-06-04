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
        public double _Scale = 1;
        public Vector3 _CSize = new Vector3(1, 1, 1);
        public Vector3 _Julia = new Vector3(0, 0, 0);

        public KleinianGroupIteration()
        {
        }

        public KleinianGroupIteration(double scale, Vector3 size, Vector3 julia, int repeats)
        {
            _CSize = size;
            _Scale = scale;
            _Julia = julia;
            _Repeats = repeats;
        }

        public override UserControl GetControl()
        {
            return new KleinianGroupControl(this);
        }

        public override void Compile(ref string frag)
        {
            frag += "Kleinian(pos, origPos, scale, float(" + _Scale + "), vec3(" + Utils.Vector3ToString(_CSize) + "), vec3(" + Utils.Vector3ToString(_Julia) + @"));
            DEMode = 3;";
        }

        public override string GetFractalString()
        {
            string repstring = "";

            return repstring;
        }

        public override void CreateElement(XElement parent)
        {
            XElement ret = new XElement("KLEINIANGROUP",
                new XAttribute("scale", _Scale),
                new XAttribute("csize", _CSize),
                new XAttribute("julia", _Julia),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
            return;
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadDouble(reader, "scale", ref _Scale);
            XMLHelpers.ReadVector3(reader, "csize", ref _CSize);
            XMLHelpers.ReadVector3(reader, "julia", ref _Julia);
            XMLHelpers.ReadInt(reader, "repeats", ref _Repeats);
            reader.Read();
        }
    }
}
