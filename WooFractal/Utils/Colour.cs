using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    [Serializable]
    public class Colour
    {
        public static Colour max(Colour col1, Colour col2)
        {
            return new Colour(col1._Red > col2._Red ? col1._Red : col2._Red,
                col1._Green > col2._Green ? col1._Green : col2._Green,
                col1._Blue > col2._Blue ? col1._Blue : col2._Blue);
        }
        public double _Red;
        public double _Green;
        public double _Blue;

        public Colour()
        {
        }

        public Colour(Colour rhs)
        {
            _Red = rhs._Red;
            _Green = rhs._Green;
            _Blue = rhs._Blue;
        }

        public Colour(double red, double green, double blue)
        {
            _Red = red;
            _Green = green;
            _Blue = blue;
        }
        
        public override string ToString()
        {
            return _Red.ToString(CultureInfo.InvariantCulture) + ", " + _Green.ToString(CultureInfo.InvariantCulture) + ", " + _Blue.ToString(CultureInfo.InvariantCulture);
        }

        public Colour Clone()
        {
            Colour ret = new Colour(_Red, _Green, _Blue);
            return ret;
        }
        // http://www.cs.rit.edu/~ncs/color/t_convert.html
       // public

        public void Clamp(double min, double max)
        {
            if (_Red < min) _Red = min;
            if (_Green < min) _Green = min;
            if (_Blue < min) _Blue = min;

            if (_Red > max) _Red = max;
            if (_Green > max) _Green = max;
            if (_Blue > max) _Blue = max;
        }

        public double GetMaxComponent()
        {
            return _Red > _Green ? (_Red > _Blue ? _Red : _Blue) : _Green > _Blue ? _Green : _Blue;
        }
        public static Colour operator +(Colour arg1, Colour arg2)
        {
            return new Colour(arg1._Red + arg2._Red,
                arg1._Green + arg2._Green,
                arg1._Blue + arg2._Red);
        }

        public XElement CreateElement(string name)
        {
            return new XElement(name,
                new XAttribute("type", 0),
                new XAttribute("colour", ToString()));
        }
    }
}
