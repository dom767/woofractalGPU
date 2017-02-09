using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooFractal
{
    [Serializable]
    public class Matrix3
    {
        public double[][] _Component;

        public Matrix3()
        {
            _Component = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                _Component[i] = new double[3];
            }
        }

        public Matrix3(Matrix3 rhs)
        {
            _Component = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                _Component[i] = new double[3];
                _Component[i][0] = rhs._Component[i][0];
                _Component[i][1] = rhs._Component[i][1];
                _Component[i][2] = rhs._Component[i][2];
            }
        }
        public Matrix3 Clone()
        {
            return new Matrix3(this);
        }

        public void MakeFromRPY(double roll, double pitch, double yaw)
        {
            double cp = Math.Cos(pitch);
            double sp = Math.Sin(pitch);
            double sr = Math.Sin(roll);
            double cr = Math.Cos(roll);
            double sy = Math.Sin(yaw);
            double cy = Math.Cos(yaw);

            _Component[0][0] = cp * cy;
            _Component[0][1] = (sr * sp * cy) - (cr * sy);
            _Component[0][2] = (cr * sp * cy) + (sr * sy);
            _Component[1][0] = cp * sy;
            _Component[1][1] = (sr * sp * sy) + (cr * cy);
            _Component[1][2] = (cr * sp * sy) - (sr * cy);
            _Component[2][0] = -sp;
            _Component[2][1] = sr * cp;
            _Component[2][2] = cr * cp;
        }

        public void Mul(Matrix3 arg)
        {
            double [][] res;
            res = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                res[i] = new double[3];
            }

            res[0][0] = _Component[0][0] * arg._Component[0][0] + _Component[0][1] * arg._Component[1][0] + _Component[0][2] * arg._Component[2][0];
            res[0][1] = _Component[0][0] * arg._Component[0][1] + _Component[0][1] * arg._Component[1][1] + _Component[0][2] * arg._Component[2][1];
            res[0][2] = _Component[0][0] * arg._Component[0][2] + _Component[0][1] * arg._Component[1][2] + _Component[0][2] * arg._Component[2][2];

            res[1][0] = _Component[1][0] * arg._Component[0][0] + _Component[1][1] * arg._Component[1][0] + _Component[1][2] * arg._Component[2][0];
            res[1][1] = _Component[1][0] * arg._Component[0][1] + _Component[1][1] * arg._Component[1][1] + _Component[1][2] * arg._Component[2][1];
            res[1][2] = _Component[1][0] * arg._Component[0][2] + _Component[1][1] * arg._Component[1][2] + _Component[1][2] * arg._Component[2][2];

            res[2][0] = _Component[2][0] * arg._Component[0][0] + _Component[2][1] * arg._Component[1][0] + _Component[2][2] * arg._Component[2][0];
            res[2][1] = _Component[2][0] * arg._Component[0][1] + _Component[2][1] * arg._Component[1][1] + _Component[2][2] * arg._Component[2][1];
            res[2][2] = _Component[2][0] * arg._Component[0][2] + _Component[2][1] * arg._Component[1][2] + _Component[2][2] * arg._Component[2][2];

            _Component = res;
        }
        public void MakeIdentity()
        {
            _Component[0][0] = _Component[1][1] = _Component[2][2] = 1.0;
            _Component[0][1] = _Component[0][2] = 0.0;
            _Component[1][0] = _Component[1][2] = 0.0;
            _Component[2][0] = _Component[2][1] = 0.0;
        }
        public override string ToString()
        {
            string outputString = "";
            for (int y =0; y<3; y++)
            {
                for (int x=0; x<3; x++)
                {
                    outputString += (_Component[y][x]).ToString(CultureInfo.InvariantCulture);
                    if (x!=2 || y!=2)
                        outputString += ", ";
                }
            }
 	        return outputString;
        }
    }
}
