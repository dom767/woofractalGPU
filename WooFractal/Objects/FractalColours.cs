﻿using System;   
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

            frag += "vec3 OrbitToColour(in vec4 orbitTrap) {\r\n";
            frag += "vec4 trappos = vec4(" + roundstartX + "pow(mod((orbitTrap.x*" + _OrbitColoursX._Multiplier.ToString() + ")+" + _OrbitColoursX._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursX._Power).ToString() + ")" + roundendX + ",";
            frag += roundstartY + "pow(mod((orbitTrap.y*" + _OrbitColoursY._Multiplier.ToString() + ")+" + _OrbitColoursY._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursY._Power).ToString() + ")" + roundendY + ",";
            frag += roundstartZ + "pow(mod((orbitTrap.z*" + _OrbitColoursZ._Multiplier.ToString() + ")+" + _OrbitColoursZ._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursZ._Power).ToString() + ")" + roundendZ + ",";
            frag += roundstartDist + "pow(mod((orbitTrap.w*" + _OrbitColoursDist._Multiplier.ToString() + ")+" + _OrbitColoursDist._Offset.ToString() + ",1.0)," + Math.Pow(10, _OrbitColoursDist._Power).ToString() + ")" + roundendDist + ");\r\n";
            frag += "vec3 diff=vec3(0,0,0);\r\n";
            if (_XOrbitEnabled) frag += "diff+=mix(vec3(" + _OrbitColoursX._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursX._EndColour._DiffuseColour.ToString() + "), trappos.x);\r\n";
            if (_YOrbitEnabled) frag += "diff+=mix(vec3(" + _OrbitColoursY._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursY._EndColour._DiffuseColour.ToString() + "), trappos.y);\r\n";
            if (_ZOrbitEnabled) frag += "diff+=mix(vec3(" + _OrbitColoursZ._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursZ._EndColour._DiffuseColour.ToString() + "), trappos.z);\r\n";
            if (_DistOrbitEnabled) frag += "diff+=mix(vec3(" + _OrbitColoursDist._StartColour._DiffuseColour.ToString() + "), vec3(" + _OrbitColoursDist._EndColour._DiffuseColour.ToString() + "), trappos.w);\r\n";
            frag += "diff/=" + divisor.ToString() + @";
return diff;}
";
/*            script += "spec=vec(0,0,0)\r\n";
            if (_XOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursX._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._SpecularColour.ToString() + "), trappos.x)\r\n";
            if (_YOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursY._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._SpecularColour.ToString() + "), trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursZ._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._SpecularColour.ToString() + "), trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "spec+=lerp(vec(" + _OrbitColoursDist._StartColour._SpecularColour.ToString() + "), vec(" + _OrbitColoursDist._EndColour._SpecularColour.ToString() + "), distance)\r\n";
            script += "spec/=" + divisor.ToString() + "\r\n";
            script += "refl=vec(0,0,0)\r\n";
            if (_XOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursX._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursX._EndColour._Reflectivity.ToString() + "), trappos.x)\r\n";
            if (_YOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursY._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursY._EndColour._Reflectivity.ToString() + "), trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursZ._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursZ._EndColour._Reflectivity.ToString() + "), trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "refl+=lerp(vec(" + _OrbitColoursDist._StartColour._Reflectivity.ToString() + "), vec(" + _OrbitColoursDist._EndColour._Reflectivity.ToString() + "), distance)\r\n";
            script += "refl/=" + divisor.ToString() + "\r\n";
            script += "emi=vec(0,0,0)\r\n";
            if (_XOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursX._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursX._EndColour._EmissiveColour.ToString() + "), trappos.x)\r\n";
            if (_YOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursY._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursY._EndColour._EmissiveColour.ToString() + "), trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursZ._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursZ._EndColour._EmissiveColour.ToString() + "), trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "emi+=lerp(vec(" + _OrbitColoursDist._StartColour._EmissiveColour.ToString() + "), vec(" + _OrbitColoursDist._EndColour._EmissiveColour.ToString() + "), distance)\r\n";
            script += "emi/=" + divisor.ToString() + "\r\n";
            script += "power=0\r\n";
            if (_XOrbitEnabled) script += "power+=lerp(" + _OrbitColoursX._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursX._EndColour._SpecularPower.ToString() + ", trappos.x)\r\n";
            if (_YOrbitEnabled) script += "power+=lerp(" + _OrbitColoursY._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursY._EndColour._SpecularPower.ToString() + ", trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "power+=lerp(" + _OrbitColoursZ._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursZ._EndColour._SpecularPower.ToString() + ", trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "power+=lerp(" + _OrbitColoursDist._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursDist._EndColour._SpecularPower.ToString() + ", distance)\r\n";
            script += "power/=" + divisor.ToString() + "\r\n";
            script += "gloss=0\r\n";
            if (_XOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursX._StartColour._Shininess.ToString() + ", " + _OrbitColoursX._EndColour._Shininess.ToString() + ", trappos.x)\r\n";
            if (_YOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursY._StartColour._Shininess.ToString() + ", " + _OrbitColoursY._EndColour._Shininess.ToString() + ", trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursZ._StartColour._Shininess.ToString() + ", " + _OrbitColoursZ._EndColour._Shininess.ToString() + ", trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursDist._StartColour._Shininess.ToString() + ", " + _OrbitColoursDist._EndColour._Shininess.ToString() + ", distance)\r\n";
            script += "gloss/=" + divisor.ToString() + "\r\n";
            script += "}\r\n";*/
//            return script;
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
            script += "power=0\r\n";
            if (_XOrbitEnabled) script += "power+=lerp(" + _OrbitColoursX._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursX._EndColour._SpecularPower.ToString() + ", trappos.x)\r\n";
            if (_YOrbitEnabled) script += "power+=lerp(" + _OrbitColoursY._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursY._EndColour._SpecularPower.ToString() + ", trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "power+=lerp(" + _OrbitColoursZ._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursZ._EndColour._SpecularPower.ToString() + ", trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "power+=lerp(" + _OrbitColoursDist._StartColour._SpecularPower.ToString() + ", " + _OrbitColoursDist._EndColour._SpecularPower.ToString() + ", distance)\r\n";
            script += "power/=" + divisor.ToString() + "\r\n";
            script += "gloss=0\r\n";
            if (_XOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursX._StartColour._Shininess.ToString() + ", " + _OrbitColoursX._EndColour._Shininess.ToString() + ", trappos.x)\r\n";
            if (_YOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursY._StartColour._Shininess.ToString() + ", " + _OrbitColoursY._EndColour._Shininess.ToString() + ", trappos.y)\r\n";
            if (_ZOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursZ._StartColour._Shininess.ToString() + ", " + _OrbitColoursZ._EndColour._Shininess.ToString() + ", trappos.z)\r\n";
            if (_DistOrbitEnabled) script += "gloss+=lerp(" + _OrbitColoursDist._StartColour._Shininess.ToString() + ", " + _OrbitColoursDist._EndColour._Shininess.ToString() + ", distance)\r\n";
            script += "gloss/=" + divisor.ToString() + "\r\n";
            script += "}\r\n";
            return script;
        }
    }
}