using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlmNet;

namespace WooFractal
{
    public class Utils
    {
        public static string mat4ToString(mat4 mat)
        {
            return "" + mat[0, 0] + ", " + mat[0, 1] + ", " + mat[0, 2] + ", " + mat[0, 3] + ", "
                + mat[1, 0] + ", " + mat[1, 1] + ", " + mat[1, 2] + ", " + mat[1, 3] + ", "
                + mat[2, 0] + ", " + mat[2, 1] + ", " + mat[2, 2] + ", " + mat[2, 3] + ", "
                + mat[3, 0] + ", " + mat[3, 1] + ", " + mat[3, 2] + ", " + mat[3, 3];
        }

        public static string Matrix3ToString(Matrix3 mat)
        {
            return "" + mat._Component[0][0] + ", " + mat._Component[0][1] + ", " + mat._Component[0][2] + ", " +
                mat._Component[1][0] + ", " + mat._Component[1][1] + ", " + mat._Component[1][2] + ", " +
                mat._Component[2][0] + ", " + mat._Component[2][1] + ", " + mat._Component[2][2];
        }

        public static string Vector3ToString(Vector3 vec)
        {
            return vec.x + ", " + vec.y + ", " + vec.z;
        }
    }
}
