using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using SharpGL.Shaders;
using SharpGL;
using System.Xml.Linq;
using System.Xml;
using WooFractal.GUI;
using WooFractal.Objects;

namespace WooFractal
{
    public class BoxfoldIteration : WooFractalIteration
    {
        public Vector3 _FoldRadius = new Vector3(1, 1, 1);
        
        public BoxfoldIteration()
        {
        }

        public BoxfoldIteration(Vector3 foldRadius, int repeats)
        {
            _FoldRadius = foldRadius;
            _Repeats = repeats;
        }
        public override UserControl GetControl()
        {
            return new BoxfoldControl(this);
        }
        
        private int _Iteration = -1;
        public override void SetIteration(int iteration)
        {
            _Iteration = iteration;
        }

        public override void SetDeclarations(ref ShaderVariables shaderVars)
        {
            shaderVars.SetValue("fracVec31", _Iteration, _FoldRadius);
        }

        public override void Compile(ref string frag, int iteration)
        {
            frag += "BoxFold(pos, origPos, scale, fracVec31[" + iteration + @"]);";
        }

        public override void CreateElement(XElement parent)
        {
            XElement ret = new XElement("BOXFOLD",
                new XAttribute("foldRadius", _FoldRadius),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
            return;
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadVector3(reader, "foldRadius", ref _FoldRadius);
            XMLHelpers.ReadInt(reader, "repeats", ref _Repeats);
            reader.Read();
        }

        public override int GetFractalType()
        {
            return 0;
        }
    }
}
