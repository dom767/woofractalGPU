using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace WooFractal
{
    public class Scene
    {
        public Camera _Camera;
        public FractalSettings _FractalSettings;

        public Scene()
        {
            _Camera = new Camera();
            _FractalSettings = new FractalSettings();
        }

        public void LoadXML(XmlReader reader)
        {
            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "FRACTAL")
                {
                    _FractalSettings = new FractalSettings();
                    _FractalSettings.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "CAMERA")
                {
                    _Camera = new Camera();
                    _Camera.LoadXML(reader);
                }
            }
            reader.Read(); // finish off reading the scene
        }

        public XElement CreateElement()
        {
            XElement ret = new XElement("SCENE");
            _FractalSettings.CreateElement(ret);
            ret.Add(_Camera.CreateElement());
            return ret;
        }
    }
}
