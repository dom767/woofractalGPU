using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooFractal
{
    [Serializable]
    public class Vector3
    {
        public Vector3() { }
        public Vector3(double ix, double iy, double iz) { x = ix; y = iy; z = iz; }
        public Vector3(Vector3 rhs) { x = rhs.x; y = rhs.y; z = rhs.z; }
        public Vector3(Colour rhs) { x = rhs._Red; y = rhs._Green; z = rhs._Blue; }

        public void LoadString(string vec)
        {
            string[] xyz = vec.Split(',');
            x = double.Parse(xyz[0]);
            y = double.Parse(xyz[1]);
            z = double.Parse(xyz[2]);
        }

        public override string ToString()
        {
            return x.ToString(CultureInfo.InvariantCulture) + ", " + y.ToString(CultureInfo.InvariantCulture) + ", " + z.ToString(CultureInfo.InvariantCulture);
        }

        public void Mul(Matrix3 mat)
        {
            double nx,ny,nz;
            nx = mat._Component[0][0] * x + mat._Component[1][0] * y + mat._Component[2][0] * z;
            ny = mat._Component[0][1] * x + mat._Component[1][1] * y + mat._Component[2][1] * z;
            nz = mat._Component[0][2] * x + mat._Component[1][2] * y + mat._Component[2][2] * z;
            x = nx; y = ny; z = nz;
        }

        public void Mul(double arg)
        {
            x *= arg;
            y *= arg;
            z *= arg;
        }

        public double Dot(Vector3 arg)
        {
            return x * arg.x + y * arg.y + z * arg.z;
        }

        public void Add(Vector3 arg)
        {
            x += arg.x;
            y += arg.y;
            z += arg.z;
        }

        public static Vector3 operator*(Vector3 arg1, double arg2)
        {
            Vector3 ret = new Vector3();
            ret.x = arg1.x * arg2;
            ret.y = arg1.y * arg2;
            ret.z = arg1.z * arg2;
            return ret;
        }

        public double MagnitudeSquared()
        {
            return x * x + y * y + z * z;
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public static Vector3 operator -(Vector3 arg1, Vector3 arg2)
        {
            Vector3 ret = new Vector3();
            ret.x = arg1.x - arg2.x;
            ret.y = arg1.y - arg2.y;
            ret.z = arg1.z - arg2.z;
            return ret;
        }

        public static Vector3 operator +(Vector3 arg1, Vector3 arg2)
        {
            Vector3 ret = new Vector3();
            ret.x = arg1.x + arg2.x;
            ret.y = arg1.y + arg2.y;
            ret.z = arg1.z + arg2.z;
            return ret;
        }

        public Vector3 Cross(Vector3 arg)
        {
            Vector3 ret = new Vector3();
            ret.x = y * arg.z - z * arg.y;
            ret.y = z * arg.x - x * arg.z;
            ret.z = z * arg.y - y * arg.x;
            return ret;
        }

        public void Normalise()
        {
            double mag = Math.Sqrt(x * x + y * y + z * z);
            x /= mag;
            y /= mag;
            z /= mag;
        }

        public Vector3 Clone()
        {
            Vector3 ret = new Vector3(this);
            return ret;
        }
        
        public double x, y, z;
    }
}
