using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;
using SharpGL.Shaders;
using SharpGL;
using GlmNet;
using WooFractal.Objects;

namespace WooFractal
{
    public class MandelboxIteration : WooFractalIteration
    {
        public Vector3 _Rotation = new Vector3(0, 0, 0);
        public Vector3 _Scale = new Vector3(1, 1, 1);
        public double _MinRadius = 0.5;

        public MandelboxIteration()
        {
        }

        public MandelboxIteration(Vector3 rotation, Vector3 scale, double minRadius, int repeats)
        {
            _Rotation = rotation;
            _Scale = scale;
            _Repeats = repeats;
            _MinRadius = minRadius;
        }

        public override UserControl GetControl()
        {
            return new MandelboxControl(this);
        }

        private int _Iteration = -1;
        public override void CompileDeclerations(ref string frag, int iteration)
        {
            frag += @"
uniform vec3 fracScale" + iteration + ";";
            frag += @"
uniform mat3 fracRot" + iteration + ";";
            frag += @"
uniform float fracMinRad" + iteration + ";";
            _Iteration = iteration;
        }

        public override void SetDeclarations(ref ShaderVariables shaderVars)
        {
            Matrix3 rot = new Matrix3();
            rot.MakeFromRPY(_Rotation.x, _Rotation.y, _Rotation.z);
            mat3 glRot = rot.GetGLMat3();

            shaderVars.Add("fracScale" + _Iteration, _Scale);
            shaderVars.Add("fracRot" + _Iteration, glRot);
            shaderVars.Add("fracMinRad" + _Iteration, (float)_MinRadius);
        }

        public override void Compile(ref string frag, int iteration)
        {
            Matrix3 rot = new Matrix3();
            rot.MakeFromRPY(_Rotation.x, _Rotation.y, _Rotation.z);
            frag += "Box(pos, origPos, scale, fracScale" + iteration + ", fracRot" + iteration + ", fracMinRad" + iteration + @");
            DEMode = 1;";
        }

        public override void CreateElement(XElement parent)
        {
            XElement ret = new XElement("BOXFRACTAL",
                new XAttribute("rotation", _Rotation),
                new XAttribute("scale", _Scale),
                new XAttribute("minradius", _MinRadius),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
            return;
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadVector3(reader, "rotation", ref _Rotation);
            XMLHelpers.ReadVector3(reader, "scale", ref _Scale);
            XMLHelpers.ReadDouble(reader, "minradius", ref _MinRadius);
            XMLHelpers.ReadInt(reader, "repeats", ref _Repeats);
            reader.Read();
        }

    }
}
