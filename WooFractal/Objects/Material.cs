﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace WooFractal
{
    [Serializable]
    public class Material
    {
        public Material()
        {
            _Opacity = 1.0f;
            _RefractiveIndex = 1.0f;
            _Density = 1.0f;
            _TintDensity = 0.1f;
            _Roughness = 1.0f;
            _DiffuseColour = new Colour(1.0f, 0.5f, 0.0f);
            _SpecularColour = new Colour(0.4f, 0.4f, 0.4f);
            _EmissiveColour = new Colour(0.0f, 0.0f, 0.0f);
            _Reflectivity = new Colour(0.3f, 0.3f, 0.3f);
            _AbsorptionColour = new Colour(0.0f, 0.0f, 0.0f);
            _MaterialFunction = "";
        }

        public Material(Material rhs)
        {
            _Opacity = rhs._Opacity;
            _RefractiveIndex = rhs._RefractiveIndex;
            _Density = rhs._Density;
            _TintDensity = rhs._TintDensity;
            _Roughness = rhs._Roughness;
            _DiffuseColour = rhs._DiffuseColour.Clone();
            _SpecularColour = rhs._SpecularColour.Clone();
            _EmissiveColour = rhs._EmissiveColour.Clone();
            _Reflectivity = rhs._Reflectivity.Clone();
            _AbsorptionColour = rhs._AbsorptionColour.Clone();
            _MaterialFunction = rhs._MaterialFunction;
        }

        public XElement CreateElement(bool preview)
        {
            return CreateElement("MATERIAL", preview);
        }

        public XElement CreateElement(string name, bool preview)
        {
            XElement mat;
            if (preview)
            {
                Colour black = new Colour(0, 0, 0);
                mat = new XElement(name,
                    new XAttribute("opacity", 1),
                    new XAttribute("density", 1),
                    new XAttribute("tintdensity", 0.1),
                    new XAttribute("roughness", 1),
                    new XAttribute("refractiveIndex", 1),
                    _DiffuseColour.CreateElement("DIFFUSECOLOUR"),
                    black.CreateElement("SPECULARCOLOUR"),
                    _EmissiveColour.CreateElement("EMISSIVECOLOUR"),
                    black.CreateElement("REFLECTIVITYCOLOUR"),
                    _AbsorptionColour.CreateElement("ABSORPTIONCOLOUR"));
            }
            else
            {
                mat = new XElement(name,
                    new XAttribute("opacity", _Opacity),
                    new XAttribute("density", _Density),
                    new XAttribute("tintdensity", _TintDensity),
                    new XAttribute("roughness", _Roughness),
                    new XAttribute("refractiveIndex", _RefractiveIndex),
                    _DiffuseColour.CreateElement("DIFFUSECOLOUR"),
                    _SpecularColour.CreateElement("SPECULARCOLOUR"),
                    _EmissiveColour.CreateElement("EMISSIVECOLOUR"),
                    _Reflectivity.CreateElement("REFLECTIVITYCOLOUR"),
                    _AbsorptionColour.CreateElement("ABSORPTIONCOLOUR"));
            }

            if (_MaterialFunction.Length > 0) mat.Add(new XAttribute("materialFunction", _MaterialFunction));

            return mat;
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadFloat(reader, "opacity", ref _Opacity);
            XMLHelpers.ReadFloat(reader, "density", ref _Density);
            XMLHelpers.ReadFloat(reader, "tintdensity", ref _TintDensity);
            XMLHelpers.ReadFloat(reader, "roughness", ref _Roughness);
            XMLHelpers.ReadFloat(reader, "refractiveIndex", ref _RefractiveIndex);

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "DIFFUSECOLOUR")
                {
                    XMLHelpers.ReadColour(reader, ref _DiffuseColour);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "SPECULARCOLOUR")
                {
                    XMLHelpers.ReadColour(reader, ref _SpecularColour);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "EMISSIVECOLOUR")
                {
                    XMLHelpers.ReadColour(reader, ref _EmissiveColour);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "REFLECTIVITYCOLOUR")
                {
                    XMLHelpers.ReadColour(reader, ref _Reflectivity);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "ABSORPTIONCOLOUR")
                {
                    XMLHelpers.ReadColour(reader, ref _AbsorptionColour);
                }
            }
            reader.Read();
        }

        public void RenderThumb(Image image)
        {
            int width = (int)image.Width;
            int height = (int)image.Height;

            byte[] pixels = new byte[4 * height * width];
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            float lightPos = 0.7f;
            int p = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float floatx = (float)x / (float)(width - 1);
                    float floaty = 1.0f - (float)y / (float)(width - 1); // deliberate width to keep aspect ratio 1:1

                    float dx = lightPos - floatx;
                    float dy = lightPos - floaty;
                    float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));

                    float highlight = (float)Math.Exp((float)(-dist) / Math.Sqrt((double)_Roughness));// (float)Math.Pow((double)dist, (double)(material._Roughness));
                    Colour pixelColour = _DiffuseColour + _SpecularColour * highlight;
                    pixelColour.Clamp(0, 1);
                    pixels[p * 4] = (byte)(pixelColour._Blue * 255.99f);
                    pixels[p * 4 + 1] = (byte)(pixelColour._Green * 255.99f);
                    pixels[p * 4 + 2] = (byte)(pixelColour._Red * 255.99f);
                    pixels[p * 4 + 3] = 255;
                    p++;
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);

            writeableBitmap.WritePixels(rect, pixels, width * 4, (int)0);

            image.Source = writeableBitmap;
        }
        
        public string _MaterialFunction;
        public Colour _DiffuseColour;
        public Colour _SpecularColour;
        public Colour _EmissiveColour;
        public Colour _Reflectivity;
        public Colour _AbsorptionColour;
        public float _Opacity;
        public float _RefractiveIndex;
        public float _Density;
        public float _TintDensity;
        public float _Roughness;
    }
}
