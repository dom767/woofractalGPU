using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;
using SharpGL.Shaders;
using SharpGL;

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

        private int _Iteration = -1;
        public override void CompileDeclerations(ref string frag, int iteration)
        {
            frag += "uniform float fracScale" + iteration + ";";
            frag += "uniform vec3 fracCSize" + iteration + ";";
            frag += "uniform vec3 fracJulia" + iteration + ";";
            _Iteration = iteration;
        }

        public override void SetDeclarations(ShaderProgram shader, OpenGL gl)
        {
            shader.SetUniform1(gl, "fracScale" + _Iteration, (float)_Scale);
            shader.SetUniform3(gl, "fracCSize" + _Iteration, (float)_CSize.x, (float)_CSize.y, (float)_CSize.z);
            shader.SetUniform3(gl, "fracJulia" + _Iteration, (float)_Julia.x, (float)_Julia.y, (float)_Julia.z);
        }

        public override void Compile(ref string frag, int iteration)
        {
            frag += "Kleinian(pos, origPos, scale, fracScale" + iteration + ", fracCSize" + iteration + ", fracJulia" + iteration + @");
            DEMode = 3;";
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
