using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for MaterialEditor.xaml
    /// </summary>
    public partial class MaterialEditor : Window, IGUIUpdateable
    {
        public Material _Material;
        public bool _OK;

        public MaterialEditor(Material material)
        {
            InitializeComponent();

            _Material = material;
            _OK = false;

            ConfigureGUI();

            RenderPreview();
        }

        public Material GetMaterial()
        {
            return _Material;
        }

        private void ConfigureGUI()
        {
            wooSlider1.Set(-Math.Log10(1.00000001 - _Material._Shininess), 0, 5.0, this);
            wooSlider2.Set(_Material._SpecularPower, 1, 80.0, this);
            colourSelector1.Set(_Material._DiffuseColour, 1, this);
            colourSelector2.Set(_Material._SpecularColour, 1, this);
            colourSelector3.Set(_Material._Reflectivity, 1, this);
            colourSelector4.Set(_Material._EmissiveColour, 1000, this);
        }

        public void GUIUpdate()
        {
            _Material._Shininess = (float)(1 - Math.Pow(10, -wooSlider1.GetSliderValue()));
            _Material._SpecularPower = (float)wooSlider2.GetSliderValue();
            _Material._DiffuseColour = colourSelector1.GetColour();
            _Material._SpecularColour = colourSelector2.GetColour();
            _Material._Reflectivity = colourSelector3.GetColour();
            _Material._EmissiveColour = colourSelector4.GetColour();

            RenderPreview();
        }

        public void RenderPreview()
        {
            _Camera = new Camera(new Vector3(0, 0.4, -0.75), new Vector3(0, 0.5, 0), 40, 0, 0);
            _Camera._AAEnabled = true;
            _Camera._DOFEnabled = false;

/*            string log = "", error = "";
            _BackgroundScript = new WooScript();
            _BackgroundScript._Program = "rule main {pos.y -= 1 box}";
            _BackgroundScript.Parse(ref log, ref error);
            _PreviewScript = new WooScript();
            _PreviewScript._Program = "rule main {diff=vec("+_Material._DiffuseColour.ToString()+")\r\n"
                + "spec=vec(" + _Material._SpecularColour.ToString() + ")\r\n"
                + "refl=vec(" + _Material._Reflectivity.ToString() + ")\r\n"
                + "emi=vec(" + _Material._EmissiveColour.ToString() + ")\r\n"
                + "power=" + _Material._SpecularPower.ToString() + "\r\n"
                + "gloss=" + _Material._Shininess.ToString() + "\r\n"
                + "sphere}";
            _PreviewScript.Parse(ref log, ref error);
            _LightingScript = new WooScript();
            _LightingScript._Program = "rule main {directionalLight(vec(1.0, 1.0, 1.0), vec(-0.7, 1.0, -0.6), 0.02, 1) background(vec(0.0,0.0,0.0))}";
            _LightingScript.Parse(ref log, ref error);
*/
//            _Scene = new Scene(_Camera);
//            _Scene.AddRenderObject(_BackgroundScript);
            //_Scene.AddRenderObject(_PreviewScript);
            //_Scene.AddRenderObject(_LightingScript);
            //BuildXML();
//            _ImageRenderer = new ImageRenderer(image1, _XML, (int)image1.Width, (int)image1.Height, false);
//            _ImageRenderer.Render();
//            _ImageRenderer.SetPostProcess(new PostProcess());
            //_ImageRenderer.TransferLatest(false);

        }

        Camera _Camera;

        private void BuildXML()
        {
//            _XML = @"<VIEWPORT width=" + image1.Width + @" height=" + image1.Height + @"/>";
//            _XML += _Camera.CreateElement().ToString();
//            _XML += _Scene.CreateElement(false, false).ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _OK = true;
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _OK = false;
            this.Close();
        }

    }
}
