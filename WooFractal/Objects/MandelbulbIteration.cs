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

namespace WooFractal
{
    public class MandelbulbIteration : WooFractalIteration
    {
        public Vector3 _Rotation = new Vector3(0,0,0);
        public double _Scale = 1;
        public bool _JuliaMode = false;
        public Vector3 _Julia = new Vector3(0, 0, 0);
 
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

        private int _Iteration = -1;
        public override void CompileDeclerations(ref string frag, int iteration)
        {
            frag += "uniform float fracScale" + iteration + ";";
            frag += "uniform mat3 fracRot" + iteration + ";";
            frag += "uniform bool fracJMode" + iteration + ";";
            frag += "uniform vec3 fracJulia" + iteration + ";";
            _Iteration = iteration;
        }

        public override void SetDeclarations(ShaderProgram shader, OpenGL gl)
        {
            Matrix3 rot = new Matrix3();
            rot.MakeFromRPY(_Rotation.x, _Rotation.y, _Rotation.z);
            mat3 glRot = rot.GetGLMat3();

            shader.SetUniform1(gl, "fracScale" + _Iteration, (float)_Scale);
            shader.SetUniformMatrix3(gl, "fracRot" + _Iteration, glRot.to_array());
            shader.SetUniform1(gl, "fracJMode" + _Iteration, _JuliaMode ? 1 : 0);
            shader.SetUniform3(gl, "fracJulia" + _Iteration, (float)_Julia.x, (float)_Julia.y, (float)_Julia.z);
        }

        public override void Compile(ref string frag, int iteration)
        {
            Matrix3 rot = new Matrix3();
            rot.MakeFromRPY(_Rotation.x, _Rotation.y, _Rotation.z);
            frag += "Bulb(r, pos, origPos, scale, fracScale" + iteration + ", fracRot" + iteration + @", fracJMode" + iteration + ", fracJulia" + iteration + @");
            DEMode = 2;";
        }

        public override void CreateElement(XElement parent)
        {
            XElement ret = new XElement("BULBFRACTAL",
                new XAttribute("rotation", _Rotation),
                new XAttribute("scale", _Scale),
                new XAttribute("juliaMode", _JuliaMode),
                new XAttribute("julia", _Julia),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
            return;
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadVector3(reader, "rotation", ref _Rotation);
            XMLHelpers.ReadDouble(reader, "scale", ref _Scale);
            XMLHelpers.ReadBool(reader, "juliaMode", ref _JuliaMode);
            XMLHelpers.ReadVector3(reader, "julia", ref _Julia);
            XMLHelpers.ReadInt(reader, "repeats", ref _Repeats);
            reader.Read();
        }
    }
}
