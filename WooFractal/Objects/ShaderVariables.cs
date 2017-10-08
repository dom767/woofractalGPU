using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;

namespace WooFractal.Objects
{
    enum SVType
    {
        Float = 1,
        Vec3 = 2,
        Mat3 = 3,
        Mat4 = 4,
        FloatArray = 5,
        Vec3Array = 6
    };

    public class ShaderVariable
    {
        public ShaderVariable(string name, float value)
        {
            _Name = name;
            _FloatValue = value;
            _Type = SVType.Float;
        }
        public ShaderVariable(string name, vec3 value)
        {
            _Name = name;
            _Vec3Value = value;
            _Type = SVType.Vec3;
        }
        public ShaderVariable(string name, mat3 value)
        {
            _Name = name;
            _Mat3Value = value;
            _Type = SVType.Mat3;
        }
        public ShaderVariable(string name, mat4 value)
        {
            _Name = name;
            _Mat4Value = value;
            _Type = SVType.Mat4;
        }
        public ShaderVariable(string name, float[] values)
        {
            _Name = name;
            _FloatArrayValues = values;
            _Type = SVType.FloatArray;
        }
        public ShaderVariable(string name, vec3[] values)
        {
            _Name = name;
            _Vec3ArrayValues = values;
            _Type = SVType.Vec3Array;
        }
        public void Set(OpenGL gl, ShaderProgram shader)
        {
            switch (_Type)
            {
                case SVType.Float:
                    shader.SetUniform1(gl, _Name, _FloatValue);
                    break;
                case SVType.Vec3:
                    shader.SetUniform3(gl, _Name, _Vec3Value.x, _Vec3Value.y, _Vec3Value.z);
                    break;
                case SVType.Mat3:
                    shader.SetUniformMatrix3(gl, _Name, _Mat3Value.to_array());
                    break;
                case SVType.Mat4:
                    shader.SetUniformMatrix4(gl, _Name, _Mat4Value.to_array());
                    break;
                case SVType.FloatArray:
                    int rte1 = shader.GetUniformLocation(gl, _Name);
                    gl.Uniform1(rte1, _FloatArrayValues.Count(), _FloatArrayValues);
                    break;
                case SVType.Vec3Array:
                    int rte2 = shader.GetUniformLocation(gl, _Name);
                    float[] tempFloats = new float[_Vec3ArrayValues.Count() * 3];
                    for (int i = 0; i < _Vec3ArrayValues.Count(); i++)
                    {
                        tempFloats[i * 3 + 0] = _Vec3ArrayValues[i].x;
                        tempFloats[i * 3 + 1] = _Vec3ArrayValues[i].y;
                        tempFloats[i * 3 + 2] = _Vec3ArrayValues[i].z;
                    }
                    gl.Uniform3(rte2, _Vec3ArrayValues.Count(), tempFloats);
                    break;
            }
        }

        private string GetValueString()
        {
            switch (_Type)
            {
                case SVType.Float:
                    return _FloatValue.ToString();
                case SVType.Vec3:
                    {
                        string ret = "vec3(" + _Vec3Value.x + ", " + _Vec3Value.y + ", " + _Vec3Value.z + ")";
                        return ret;
                    }
                case SVType.Mat3:
                    {
                        string ret = "mat3(";
                        float[] vals = _Mat3Value.to_array();
                        for (int i = 0; i < 8; i++)
                        {
                            ret += vals[i] + ", ";
                        }
                        ret += vals[8] + ")";
                        return ret;
                    }
                case SVType.Mat4:
                    {
                        string ret = "mat4(";
                        float[] vals = _Mat4Value.to_array();
                        for (int i = 0; i < 15; i++)
                        {
                            ret += vals[i] + ", ";
                        }
                        ret += vals[15] + ")";
                        return ret;
                    }
                case SVType.FloatArray:
                    {
                        string ret = "{";
                        for (int i = 0; i < _FloatArrayValues.Count() - 1; i++)
                        {
                            ret += _FloatArrayValues[i] + ", ";
                        }
                        ret += _FloatArrayValues[_FloatArrayValues.Count()-1] + "}";
                        return ret;
                    }
                case SVType.Vec3Array:
                    {
                        string ret = "vec3[](";
                        for (int i = 0; i < _Vec3ArrayValues.Count() - 1; i++)
                        {
                            ret += "vec3("+_Vec3ArrayValues[i].x+", "+ _Vec3ArrayValues[i].y+", "+_Vec3ArrayValues[i].z + "), ";
                        }
                        ret += "vec3("+_Vec3ArrayValues[_Vec3ArrayValues.Count() - 1].x+", "+ _Vec3ArrayValues[_Vec3ArrayValues.Count() - 1].y+", "+_Vec3ArrayValues[_Vec3ArrayValues.Count() - 1].z + "))";
                        return ret;
                    }
            }
            return "";
        }

        public void BurnVariable(ref string program)
        {
            string ret = "";
            string valueString = GetValueString();
            string[] lines = program.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0; i<lines.Count(); i++)
            {
                if (lines[i].Contains(_Name) && lines[i].Contains("uniform"))
                {
                    ret += lines[i].Replace("uniform ", "").Replace(";", "= " + valueString + ";") + Environment.NewLine;
                }
                else
                {
                    ret += lines[i] + Environment.NewLine;
                }
            }
            program = ret;
        }
 
        string _Name;
        SVType _Type;
        float _FloatValue; // when you can't be arsed to OO it...
        vec3 _Vec3Value;
        mat3 _Mat3Value;
        mat4 _Mat4Value;
        float[] _FloatArrayValues;
        vec3[] _Vec3ArrayValues;
    };

    public class ShaderVariables
    {
        List<ShaderVariable> _ShaderVariable = new List<ShaderVariable>();

        public void Add(string name, float value)
        {
            _ShaderVariable.Add(new ShaderVariable(name, value));
        }
        public void Add(string name, vec3 value)
        {
            _ShaderVariable.Add(new ShaderVariable(name, value));
        }
        public void Add(string name, Vector3 vec)
        {
            vec3 value = new vec3((float)vec.x, (float)vec.y, (float)vec.z);
            _ShaderVariable.Add(new ShaderVariable(name, value));
        }
        public void Add(string name, Colour col)
        {
            vec3 value = new vec3((float)col._Red, (float)col._Green, (float)col._Blue);
            _ShaderVariable.Add(new ShaderVariable(name, value));
        }
        public void Add(string name, mat3 value)
        {
            _ShaderVariable.Add(new ShaderVariable(name, value));
        }
        public void Add(string name, mat4 value)
        {
            _ShaderVariable.Add(new ShaderVariable(name, value));
        }
        public void Add(string name, float[] values)
        {
            _ShaderVariable.Add(new ShaderVariable(name, values));
        }
        public void Add(string name, vec3[] values)
        {
            _ShaderVariable.Add(new ShaderVariable(name, values));
        }

        public void Set(OpenGL gl, ShaderProgram shader)
        {
            for (int i = 0; i < _ShaderVariable.Count(); i++)
            {
                _ShaderVariable[i].Set(gl, shader);
            }
        }

        public void BurnVariables(ref string program)
        {
            for (int i = 0; i < _ShaderVariable.Count(); i++)
            {
                _ShaderVariable[i].BurnVariable(ref program);
            }
        }
    };
}
