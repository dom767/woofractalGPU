using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal
{
    public class FractalColours
    {
        public bool _XOrbitEnabled = true;
        public bool _YOrbitEnabled = true;
        public bool _ZOrbitEnabled = true;
        public bool _DistOrbitEnabled = false;
        public OrbitColours _OrbitColoursX = new OrbitColours();
        public OrbitColours _OrbitColoursY = new OrbitColours();
        public OrbitColours _OrbitColoursZ = new OrbitColours();
        public OrbitColours _OrbitColoursDist = new OrbitColours();

        public UserControl GetControl()
        {
            return new FractalColourControl(this);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("FRACTALCOLOURS",
                new XAttribute("xOrbitEnabled", _XOrbitEnabled),
                new XAttribute("yOrbitEnabled", _YOrbitEnabled),
                new XAttribute("zOrbitEnabled", _ZOrbitEnabled),
                new XAttribute("distOrbitEnabled", _DistOrbitEnabled),
                _OrbitColoursX.CreateElement("ORBITCOLOURSX"),
                _OrbitColoursY.CreateElement("ORBITCOLOURSY"),
                _OrbitColoursZ.CreateElement("ORBITCOLOURSZ"),
                _OrbitColoursDist.CreateElement("ORBITCOLOURSDIST"));

            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadBool(reader, "xOrbitEnabled", ref _XOrbitEnabled);
            XMLHelpers.ReadBool(reader, "yOrbitEnabled", ref _YOrbitEnabled);
            XMLHelpers.ReadBool(reader, "zOrbitEnabled", ref _ZOrbitEnabled);
            XMLHelpers.ReadBool(reader, "distOrbitEnabled", ref _DistOrbitEnabled);

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ORBITCOLOURSX")
                {
                    _OrbitColoursX.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ORBITCOLOURSY")
                {
                    _OrbitColoursY.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ORBITCOLOURSZ")
                {
                    _OrbitColoursZ.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ORBITCOLOURSDIST")
                {
                    _OrbitColoursDist.LoadXML(reader);
                }
            }
            reader.Read();
        }

        public void Compile(ref string frag)
        {
            Colour totalDiffuse = new Colour(0, 0, 0);

            if (_XOrbitEnabled)
                totalDiffuse += Colour.max(_OrbitColoursX._StartColour._DiffuseColour, _OrbitColoursX._EndColour._DiffuseColour);
            if (_YOrbitEnabled)
                totalDiffuse += Colour.max(_OrbitColoursY._StartColour._DiffuseColour, _OrbitColoursY._EndColour._DiffuseColour);
            if (_ZOrbitEnabled)
                totalDiffuse += Colour.max(_OrbitColoursZ._StartColour._DiffuseColour, _OrbitColoursZ._EndColour._DiffuseColour);
            double max = totalDiffuse.GetMaxComponent();
            if (max < 0.001) max = 0.001;

            int divisor = (_XOrbitEnabled ? 1 : 0) + (_YOrbitEnabled ? 1 : 0) + (_ZOrbitEnabled ? 1 : 0) + (_DistOrbitEnabled ? 1 : 0);

            string roundstartX = (_OrbitColoursX._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendX = (_OrbitColoursX._BlendType == EBlendType.Chop) ? ")" : "";
            string roundstartY = (_OrbitColoursY._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendY = (_OrbitColoursY._BlendType == EBlendType.Chop) ? ")" : "";
            string roundstartZ = (_OrbitColoursZ._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendZ = (_OrbitColoursZ._BlendType == EBlendType.Chop) ? ")" : "";
            string roundstartDist = (_OrbitColoursDist._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendDist = (_OrbitColoursDist._BlendType == EBlendType.Chop) ? ")" : "";

            frag += @"void OrbitToColour(in vec4 orbitTrap, inout material mat) {";
            frag += "vec4 trappos = vec4(" + roundstartX + "pow(mod((orbitTrap.x*" + _OrbitColoursX._Multiplier.ToString() + ")+" + _OrbitColoursX._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursX._Power).ToString() + ")" + roundendX + ",";
            frag += roundstartY + "pow(mod((orbitTrap.y*" + _OrbitColoursY._Multiplier.ToString() + ")+" + _OrbitColoursY._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursY._Power).ToString() + ")" + roundendY + ",";
            frag += roundstartZ + "pow(mod((orbitTrap.z*" + _OrbitColoursZ._Multiplier.ToString() + ")+" + _OrbitColoursZ._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursZ._Power).ToString() + ")" + roundendZ + ",";
            frag += roundstartDist + "pow(mod((orbitTrap.w*" + _OrbitColoursDist._Multiplier.ToString() + ")+" + _OrbitColoursDist._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursDist._Power).ToString() + ")" + roundendDist + ");\r\n";
            frag += "mat.diff=vec3(0,0,0);\r\n";
            if (_XOrbitEnabled) frag += "mat.diff+=mix(vec3(" + _OrbitColoursX._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursX._EndColour._DiffuseColour.ToString() + "), trappos.x);\r\n";
            if (_YOrbitEnabled) frag += "mat.diff+=mix(vec3(" + _OrbitColoursY._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursY._EndColour._DiffuseColour.ToString() + "), trappos.y);\r\n";
            if (_ZOrbitEnabled) frag += "mat.diff+=mix(vec3(" + _OrbitColoursZ._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursZ._EndColour._DiffuseColour.ToString() + "), trappos.z);\r\n";
            if (_DistOrbitEnabled) frag += "mat.diff+=mix(vec3(" + _OrbitColoursDist._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursDist._EndColour._DiffuseColour.ToString() + "), trappos.w);\r\n";
            frag += "mat.diff/=" + divisor.ToString() + @";";
            frag += "mat.spec=vec3(0,0,0);\r\n";
            if (_XOrbitEnabled) frag += "mat.spec+=mix(vec3(" + _OrbitColoursX._StartColour._SpecularColour.ToString() + "), vec3(" + _OrbitColoursX._EndColour._SpecularColour.ToString() + "), trappos.x);\r\n";
            if (_YOrbitEnabled) frag += "mat.spec+=mix(vec3(" + _OrbitColoursY._StartColour._SpecularColour.ToString() + "), vec3(" + _OrbitColoursY._EndColour._SpecularColour.ToString() + "), trappos.y);\r\n";
            if (_ZOrbitEnabled) frag += "mat.spec+=mix(vec3(" + _OrbitColoursZ._StartColour._SpecularColour.ToString() + "), vec3(" + _OrbitColoursZ._EndColour._SpecularColour.ToString() + "), trappos.z);\r\n";
            if (_DistOrbitEnabled) frag += "mat.spec+=mix(vec3(" + _OrbitColoursDist._StartColour._SpecularColour.ToString() + "), vec3(" + _OrbitColoursDist._EndColour._SpecularColour.ToString() + "), trappos.w);\r\n";
            frag += "mat.spec/=" + divisor.ToString() + @";";
            frag += "mat.refl=vec3(0,0,0);\r\n";
            if (_XOrbitEnabled) frag += "mat.refl+=mix(vec3(" + _OrbitColoursX._StartColour._Reflectivity.ToString() + "), vec3(" + _OrbitColoursX._EndColour._Reflectivity.ToString() + "), trappos.x);\r\n";
            if (_YOrbitEnabled) frag += "mat.refl+=mix(vec3(" + _OrbitColoursY._StartColour._Reflectivity.ToString() + "), vec3(" + _OrbitColoursY._EndColour._Reflectivity.ToString() + "), trappos.y);\r\n";
            if (_ZOrbitEnabled) frag += "mat.refl+=mix(vec3(" + _OrbitColoursZ._StartColour._Reflectivity.ToString() + "), vec3(" + _OrbitColoursZ._EndColour._Reflectivity.ToString() + "), trappos.z);\r\n";
            if (_DistOrbitEnabled) frag += "mat.refl+=mix(vec3(" + _OrbitColoursDist._StartColour._Reflectivity.ToString() + "), vec3(" + _OrbitColoursDist._EndColour._Reflectivity.ToString() + "), trappos.w);\r\n";
            frag += "mat.refl/=" + divisor.ToString() + @";";
            frag += "mat.emi=vec3(0,0,0);\r\n";
            if (_XOrbitEnabled) frag += "mat.emi+=mix(vec3(" + _OrbitColoursX._StartColour._EmissiveColour.ToString() + "), vec3(" + _OrbitColoursX._EndColour._EmissiveColour.ToString() + "), trappos.x);\r\n";
            if (_YOrbitEnabled) frag += "mat.emi+=mix(vec3(" + _OrbitColoursY._StartColour._EmissiveColour.ToString() + "), vec3(" + _OrbitColoursY._EndColour._EmissiveColour.ToString() + "), trappos.y);\r\n";
            if (_ZOrbitEnabled) frag += "mat.emi+=mix(vec3(" + _OrbitColoursZ._StartColour._EmissiveColour.ToString() + "), vec3(" + _OrbitColoursZ._EndColour._EmissiveColour.ToString() + "), trappos.z);\r\n";
            if (_DistOrbitEnabled) frag += "mat.emi+=mix(vec3(" + _OrbitColoursDist._StartColour._EmissiveColour.ToString() + "), vec3(" + _OrbitColoursDist._EndColour._EmissiveColour.ToString() + "), trappos.w);\r\n";
            frag += "mat.emi/=" + divisor.ToString() + @";";
            frag += "mat.roughness=0;\r\n";
            if (_XOrbitEnabled) frag += "mat.roughness+=mix(" + _OrbitColoursX._StartColour._Roughness.ToString() + ", " + _OrbitColoursX._EndColour._Roughness.ToString() + ", trappos.x);\r\n";
            if (_YOrbitEnabled) frag += "mat.roughness+=mix(" + _OrbitColoursY._StartColour._Roughness.ToString() + ", " + _OrbitColoursY._EndColour._Roughness.ToString() + ", trappos.y);\r\n";
            if (_ZOrbitEnabled) frag += "mat.roughness+=mix(" + _OrbitColoursZ._StartColour._Roughness.ToString() + ", " + _OrbitColoursZ._EndColour._Roughness.ToString() + ", trappos.z);\r\n";
            if (_DistOrbitEnabled) frag += "mat.roughness+=mix(" + _OrbitColoursDist._StartColour._Roughness.ToString() + ", " + _OrbitColoursDist._EndColour._Roughness.ToString() + ", trappos.w);\r\n";
            frag += "mat.roughness/=" + divisor.ToString() + @";";
  
            frag += "}";
        }

        public string GenerateScript(bool complexMaterials)
        {
            string script = "";

            Colour totalDiffuse = new Colour(0, 0, 0);

            if (_XOrbitEnabled)
                totalDiffuse += Colour.max(_OrbitColoursX._StartColour._DiffuseColour, _OrbitColoursX._EndColour._DiffuseColour);
            if (_YOrbitEnabled)
                totalDiffuse += Colour.max(_OrbitColoursY._StartColour._DiffuseColour, _OrbitColoursY._EndColour._DiffuseColour);
            if (_ZOrbitEnabled)
                totalDiffuse += Colour.max(_OrbitColoursZ._StartColour._DiffuseColour, _OrbitColoursZ._EndColour._DiffuseColour);
            double max = totalDiffuse.GetMaxComponent();
            if (max < 0.001) max = 0.001;

            int divisor = (_XOrbitEnabled ? 1 : 0) + (_YOrbitEnabled ? 1 : 0) + (_ZOrbitEnabled ? 1 : 0) + (_DistOrbitEnabled ? 1 : 0);

            string roundstartX = (_OrbitColoursX._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendX = (_OrbitColoursX._BlendType == EBlendType.Chop) ? ")" : "";
            string roundstartY = (_OrbitColoursY._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendY = (_OrbitColoursY._BlendType == EBlendType.Chop) ? ")" : "";
            string roundstartZ = (_OrbitColoursZ._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendZ = (_OrbitColoursZ._BlendType == EBlendType.Chop) ? ")" : "";
            string roundstartDist = (_OrbitColoursDist._BlendType == EBlendType.Chop) ? "round(" : "";
            string roundendDist = (_OrbitColoursDist._BlendType == EBlendType.Chop) ? ")" : "";

            script += "shader fracColours {\r\n";
            if (_DistOrbitEnabled) script += "distance = sqrt(diff.x*diff.x + diff.y*diff.y + diff.z*diff.z)\r\n";
            if (_DistOrbitEnabled) script += "distance = " + roundstartDist + "pow(mod((distance*" + _OrbitColoursDist._Multiplier.ToString() + ")+" + _OrbitColoursDist._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursDist._Power).ToString() + ")" + roundendDist + "\r\n";
            script += "trappos = vec(" + roundstartX + "pow(mod((diff.x*" + _OrbitColoursX._Multiplier.ToString() + ")+" + _OrbitColoursX._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursX._Power).ToString() + ")" + roundendX + ",";
            script += roundstartY + "pow(mod((diff.y*" + _OrbitColoursY._Multiplier.ToString() + ")+" + _OrbitColoursY._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursY._Power).ToString() + ")" + roundendY + ",";
            script += roundstartZ + "pow(mod((diff.z*" + _OrbitColoursZ._Multiplier.ToString() + ")+" + _OrbitColoursZ._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursZ._Power).ToString() + ")" + roundendZ + ")\r\n";
            script += "diff=vec(0,0,0)\r\n";
            if (_XOrbitEnabled) script += "diff+=lerp(vec(" + _OrbitColoursX._StartColour._DiffuseColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._DiffuseColour.ToString() + "), trappos.x)\r\n";
            if (_YOrbitEnabled) script += "diff+=lerp(vec(" + _OrbitColoursY._StartColour._DiffuseColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._DiffuseColour.ToString() + "), trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "diff+=lerp(vec(" + _OrbitColoursZ._StartColour._DiffuseColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._DiffuseColour.ToString() + "), trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "diff+=lerp(vec(" + _OrbitColoursDist._StartColour._DiffuseColour.ToString() + "), vec(" + _OrbitColoursDist._EndColour._DiffuseColour.ToString() + "), distance)\r\n";
            script += "diff/=" + divisor.ToString() + "\r\n";
            script += "spec=vec(0,0,0)\r\n";
            if (_XOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursX._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._SpecularColour.ToString() + "), trappos.x)\r\n";
            if (_YOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursY._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._SpecularColour.ToString() + "), trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursZ._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._SpecularColour.ToString() + "), trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursDist._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursDist._EndColour._SpecularColour.ToString() + "), distance)\r\n";
            script += "spec/=" + divisor.ToString() + "\r\n";
//            if (complexMaterials)
            {
                script += "refl=vec(0,0,0)\r\n";
                if (_XOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursX._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursX._EndColour._Reflectivity.ToString() + "), trappos.x)\r\n";
                if (_YOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursY._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursY._EndColour._Reflectivity.ToString() + "), trappos.y)\r\n";
                if (_ZOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursZ._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursZ._EndColour._Reflectivity.ToString() + "), trappos.z)\r\n";
                if (_DistOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursDist._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursDist._EndColour._Reflectivity.ToString() + "), distance)\r\n";
                script += "refl/=" + divisor.ToString() + "\r\n";
            }
            script += "emi=vec(0,0,0)\r\n";
            if (_XOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursX._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._EmissiveColour.ToString() + "), trappos.x)\r\n";
            if (_YOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursY._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._EmissiveColour.ToString() + "), trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursZ._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._EmissiveColour.ToString() + "), trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursDist._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursDist._EndColour._EmissiveColour.ToString() + "), distance)\r\n";
            script += "emi/=" + divisor.ToString() + "\r\n";
            script += "gloss=0\r\n";
            if (_XOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursX._StartColour._Roughness.ToString() + ", " + _OrbitColoursX._EndColour._Roughness.ToString() + ", trappos.x)\r\n";
            if (_YOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursY._StartColour._Roughness.ToString() + ", " + _OrbitColoursY._EndColour._Roughness.ToString() + ", trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursZ._StartColour._Roughness.ToString() + ", " + _OrbitColoursZ._EndColour._Roughness.ToString() + ", trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursDist._StartColour._Roughness.ToString() + ", " + _OrbitColoursDist._EndColour._Roughness.ToString() + ", distance)\r\n";
            script += "gloss/=" + divisor.ToString() + "\r\n";
            script += "}\r\n";
            return script;
        }
    }
}
