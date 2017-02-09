using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public interface ColourFunction
    {
        XElement CreateElement(string name);
        ColourFunction Clone();
        Colour GetColour();
    }

    [Serializable]
    public class CFConstant : ColourFunction
    {
        public CFConstant()
        {
            _Colour = new Colour();
        }
        public CFConstant(double red, double green, double blue)
        {
            _Colour = new Colour(red, green, blue);
        }
        public ColourFunction Clone()
        {
            return new CFConstant(_Colour._Red, _Colour._Green, _Colour._Blue);
        }
        public XElement CreateElement(string name)
        {
            return new XElement(name,
                new XAttribute("type", 0),
                new XAttribute("colour", _Colour.ToString()));
        }
        public Colour GetColour()
        {
            return _Colour;
        }

        public Colour _Colour;
    }

    [Serializable]
    public class CFGrid : ColourFunction
    {
        public Colour _Colour1;
        public Colour _Colour2;
        public float _Scale;
        public float _Ratio;
        public bool _IsCheckerboard;

        public CFGrid()
        {
            _Colour1 = new Colour();
            _Colour2 = new Colour();
            _Scale = 1;
            _Ratio = 0.9f;
            _IsCheckerboard = true;
        }
        public ColourFunction Clone()
        {
            CFGrid clone = new CFGrid();
            clone._Colour1 = new Colour(_Colour1);
            clone._Colour2 = new Colour(_Colour2);
            clone._Scale = _Scale;
            clone._Ratio = _Ratio;
            clone._IsCheckerboard = _IsCheckerboard;
            return clone;
        }
        public XElement CreateElement(string name)
        {
            return new XElement(name,
                new XAttribute("type", 1),
                new XAttribute("colour1", _Colour1.ToString()),
                new XAttribute("colour2", _Colour2.ToString()),
                new XAttribute("scale", _Scale),
                new XAttribute("ratio", _Ratio),
                new XAttribute("checkerboard", _IsCheckerboard));
        }
        public Colour GetColour()
        {
            return _Colour1;
        }
    }
}
