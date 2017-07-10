using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace WooFractal.Objects
{
    public class MaterialSelection
    {
        public Material[] _Defaults = new Material[18];

        public MaterialSelection()
        {
            Material temp = new Material();
            temp._DiffuseColour = new Colour(0.8, 0.3, 0.3);
            temp._Reflectivity = new Colour(0.3);
            temp._SpecularColour = new Colour(0.3);
            temp._DiElectric = 0.5f;
            temp._Roughness = 0.2f;
            _Defaults[0] = new Material(temp);
            temp._DiffuseColour = new Colour(0.8, 0.8, 0.3);
            _Defaults[1] = new Material(temp);
            temp._DiffuseColour = new Colour(0.3, 0.8, 0.3);
            _Defaults[2] = new Material(temp);
            temp._DiffuseColour = new Colour(0.3, 0.8, 0.8);
            _Defaults[3] = new Material(temp);
            temp._DiffuseColour = new Colour(0.3, 0.3, 0.8);
            _Defaults[4] = new Material(temp);
            temp._DiffuseColour = new Colour(0.8, 0.3, 0.8);
            _Defaults[5] = new Material(temp);

            temp._Reflectivity = new Colour(0.5);
            temp._SpecularColour = new Colour(0.5);
            temp._Roughness = 0.12f;

            temp._DiffuseColour = new Colour(0.7, 0.2, 0.2);
            _Defaults[6] = new Material(temp);
            temp._DiffuseColour = new Colour(0.7, 0.7, 0.2);
            _Defaults[7] = new Material(temp);
            temp._DiffuseColour = new Colour(0.2, 0.7, 0.2);
            _Defaults[8] = new Material(temp);
            temp._DiffuseColour = new Colour(0.2, 0.7, 0.7);
            _Defaults[9] = new Material(temp);
            temp._DiffuseColour = new Colour(0.2, 0.2, 0.7);
            _Defaults[10] = new Material(temp);
            temp._DiffuseColour = new Colour(0.7, 0.2, 0.7);
            _Defaults[11] = new Material(temp);

            temp._Roughness = 0.002f;

            temp._Reflectivity = new Colour(0.8);
            temp._SpecularColour = temp._Reflectivity;
            temp._DiffuseColour = new Colour(0.1, 0.1, 0.1);
            _Defaults[12] = new Material(temp);
            temp._Reflectivity = new Colour(0.5);
            temp._SpecularColour = temp._Reflectivity;
            temp._DiffuseColour = new Colour(0.3, 0.3, 0.3);
            _Defaults[13] = new Material(temp);
            temp._Reflectivity = new Colour(0.9, 0.6, 0.3);
            temp._SpecularColour = temp._Reflectivity;
            temp._DiffuseColour = new Colour(0.1, 0.1, 0.1);
            _Defaults[14] = new Material(temp);
            temp._Reflectivity = new Colour(0.9, 0.8, 0.3);
            temp._SpecularColour = temp._Reflectivity;
            temp._DiffuseColour = new Colour(0.1, 0.1, 0.1);
            _Defaults[15] = new Material(temp);
            temp._Reflectivity = new Colour(0.03, 0.03, 0.03);
            temp._SpecularColour = temp._Reflectivity;
            temp._Roughness = 0.3f;
            temp._DiffuseColour = new Colour(0.1, 0.1, 0.1);
            _Defaults[16] = new Material(temp);
            temp._DiffuseColour = new Colour(0.9, 0.9, 0.9);
            _Defaults[17] = new Material(temp);
        }

        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("MATERIALSELECTION");
            for (int i=0; i<18; i++)
                ret.Add(_Defaults[i].CreateElement(false));
            parent.Add(ret);
        }

        public void LoadXML(XmlReader reader)
        {
            int matIdx = 0;

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "MATERIAL")
                {
                    _Defaults[matIdx++].LoadXML(reader);
                }
            }
            reader.Read();
        }
    }
}
