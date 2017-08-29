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
    public class KIFSIteration : WooFractalIteration
    {
        public Vector3 _PreRotation = new Vector3(0,0,0);
        public Vector3 _PostRotation = new Vector3(0, 0, 0);
        public double _Scale = 1.9;
        public Vector3 _Offset = new Vector3(1, 1, 1);
        public EFractalType _FractalType = EFractalType.Cube;

        public override void CreateElement(XElement parent)
        {
            XElement ret = new XElement("KIFSFRACTAL",
                new XAttribute("preRotation", _PreRotation),
                new XAttribute("postRotation", _PostRotation),
                new XAttribute("scale", _Scale),
                new XAttribute("offset", _Offset),
                new XAttribute("fractalType", _FractalType),
                new XAttribute("repeats", _Repeats));
            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadVector3(reader, "preRotation", ref _PreRotation);
            XMLHelpers.ReadVector3(reader, "postRotation", ref _PostRotation);
            XMLHelpers.ReadDouble(reader, "scale", ref _Scale);
            XMLHelpers.ReadVector3(reader, "offset", ref _Offset);
            XMLHelpers.ReadFractalType(reader, "fractalType", ref _FractalType);
            XMLHelpers.ReadInt(reader, "repeats", ref _Repeats);
            reader.Read();
        }

        public KIFSIteration()
        {
        }

        public KIFSIteration(EFractalType type, Vector3 preRotation, Vector3 PostRotation, double scale, Vector3 offset, int repeats)
        {
            _FractalType = type;
            _PreRotation = preRotation;
            _PostRotation = PostRotation;
            _Scale = scale;
            _Offset = offset;
            _Repeats = repeats;
        }

        public override UserControl GetControl()
        {
            return new FractalControl(this);
        }

        private int _Iteration = -1;
        public override void CompileDeclerations(ref string frag, int iteration)
        {
            frag += @"
uniform float fracScale" + iteration + ";";
            frag += @"
uniform vec3 fracOffset" + iteration + ";";
            frag += @"
uniform mat3 fracPreRot" + iteration + ";";
            frag += @"
uniform mat3 fracPostRot" + iteration + ";";
            _Iteration = iteration;
        }

        public override void SetDeclarations(ref ShaderVariables shaderVars)
        {
            Matrix3 preRot = new Matrix3();
            preRot.MakeFromRPY(_PreRotation.x, _PreRotation.y, _PreRotation.z);
            mat3 glPreRot = preRot.GetGLMat3();
            Matrix3 postRot = new Matrix3();
            postRot.MakeFromRPY(_PostRotation.x, _PostRotation.y, _PostRotation.z);
            mat3 glPostRot = postRot.GetGLMat3();
            
            shaderVars.Add("fracScale" + _Iteration, (float)_Scale);
            shaderVars.Add("fracOffset" + _Iteration, _Offset);
            shaderVars.Add("fracPreRot" + _Iteration, glPreRot);
            shaderVars.Add("fracPostRot" + _Iteration, glPostRot);
        }

        public override void Compile(ref string frag, int iteration)
        {
            Matrix3 preRot = new Matrix3();
            preRot.MakeFromRPY(_PreRotation.x, _PreRotation.y, _PreRotation.z);
            Matrix3 postRot = new Matrix3();
            postRot.MakeFromRPY(_PostRotation.x, _PostRotation.y, _PostRotation.z);
            switch (_FractalType)
            {
                case EFractalType.Cube:
                    frag += "Cuboid(pos, origPos, scale, fracScale" + iteration + ", fracOffset" + iteration + ", fracPreRot" + iteration + ", fracPostRot" + iteration + @");
            DEMode = 0;";
                    break;
                case EFractalType.Menger:
                    frag += "Menger(pos, origPos, scale, fracScale" + iteration + ", fracOffset" + iteration + ", fracPreRot" + iteration + ", fracPostRot" + iteration + @");
            DEMode = 0;";
                    break;
                case EFractalType.Tetra:
                    frag += "Tetra(pos, origPos, scale, fracScale" + iteration + ", fracOffset" + iteration + ", fracPreRot" + iteration + ", fracPostRot" + iteration + @");
            DEMode = 0;";
                    break;
            }
        }
    };
}
