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

        public Colour(double val)
        {
            _Red = val;
            _Green = val;
            _Blue = val;
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
                arg1._Blue + arg2._Blue);
        }
        public static Colour operator *(Colour arg1, float arg2)
        {
            return new Colour(arg1._Red * arg2,
                arg1._Green * arg2,
                arg1._Blue * arg2);
        }

        public XElement CreateElement(string name)
        {
            return new XElement(name,
                new XAttribute("type", 0),
                new XAttribute("colour", ToString()));
        }
    }
    public class HSLColour
    {
        public HSLColour(double h, double s, double l)
        {
            _H = h;
            _S = s;
            _L = l;
        }
        public HSLColour(Colour rhs)
        {
            double r=rhs._Red, g=rhs._Green, b=rhs._Blue;
            double h=0, s=0, l=0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            v = Math.Max(r,g);
            v = Math.Max(v,b);
            m = Math.Min(r,g);
            m = Math.Min(m,b);

            if ((l = (m + v) / 2.0) > 0.0)
            {
                if ((s = vm = v - m) > 0.0)
                {
                    s /= (l <= 0.5) ? (v + m) :
                        (2.0 - v - m);
                    r2 = (v - r) / vm;
                    g2 = (v - g) / vm;
                    b2 = (v - b) / vm;

                    if (r == v)
                        h = (g == m ? 5.0 + b2 : 1.0 - g2);
                    else if (g == v)
                        h = (b == m ? 1.0 + r2 : 3.0 - b2);
                    else
                        h = (r == m ? 3.0 + g2 : 5.0 - r2);

                    h /= 6;
                }
           }

            _H = h;
            _S = s;
            _L = l;
        }

        public Colour GetColour()
        {
            double v;
            double h = _H, s = _S, l = _L;
            double r=0, g=0, b=0;

            v = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);
            if (v >= 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant%6)
                {
                    case 0: r = v; g = mid1; b = m; break;
                    case 1: r = mid2; g = v; b = m; break;
                    case 2: r = m; g = v; b = mid1; break;
                    case 3: r = m; g = mid2; b = v; break;
                    case 4: r = mid1; g = m; b = v; break;
                    case 5: r = v; g = m; b = mid2; break;
                }
            }

            return new Colour(r, g, b);
        }

        public double _H, _S, _L;
    }
}
