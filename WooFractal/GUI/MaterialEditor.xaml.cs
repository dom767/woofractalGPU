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
using SharpGL;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;
using WooFractal.Objects;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for MaterialEditor.xaml
    /// </summary>
    public partial class MaterialEditor : Window, IGUIUpdateable
    {
        public MaterialSelection _MaterialSelection;
        private Material _Material;
        public bool _OK;
        public bool _Clean = false;

        public MaterialEditor(MaterialSelection materialSelection)
        {
            InitializeComponent();

            _MaterialSelection = materialSelection;
            _Material = _MaterialSelection._Defaults[0];
            _OK = false;

            ConfigureGUI();

            RenderPreview();
        }

        public MaterialSelection GetMaterialSelection()
        {
            return _MaterialSelection;
        }

        private void ConfigureGUI()
        {
            floatEditor1.Set("Roughness", _Material._Roughness, 0, 1.0, FloatEditorFlags.None, this);

            colourSelector1.Set(_Material._DiffuseColour, 1, this);
            colourSelector2.Set(_Material._SpecularColour, 1, this);
            colourSelector3.Set(_Material._Reflectivity, 1, this);

            RenderThumbs();

            RenderPreview();
        }

        private void RenderThumbs()
        {
            _MaterialSelection._Defaults[0].RenderThumb(image1);
            _MaterialSelection._Defaults[1].RenderThumb(image2);
            _MaterialSelection._Defaults[2].RenderThumb(image3);
            _MaterialSelection._Defaults[3].RenderThumb(image4);
            _MaterialSelection._Defaults[4].RenderThumb(image5);
            _MaterialSelection._Defaults[5].RenderThumb(image6);

            _MaterialSelection._Defaults[6].RenderThumb(image7);
            _MaterialSelection._Defaults[7].RenderThumb(image8);
            _MaterialSelection._Defaults[8].RenderThumb(image9);
            _MaterialSelection._Defaults[9].RenderThumb(image10);
            _MaterialSelection._Defaults[10].RenderThumb(image11);
            _MaterialSelection._Defaults[11].RenderThumb(image12);

            _MaterialSelection._Defaults[12].RenderThumb(image13);
            _MaterialSelection._Defaults[13].RenderThumb(image14);
            _MaterialSelection._Defaults[14].RenderThumb(image15);
            _MaterialSelection._Defaults[15].RenderThumb(image16);
            _MaterialSelection._Defaults[16].RenderThumb(image17);
            _MaterialSelection._Defaults[17].RenderThumb(image18);
        }

        private void UpdateGUI()
        {
            floatEditor1.SetSliderValue(_Material._Roughness, false);

            colourSelector1.SetColour(_Material._DiffuseColour, false);
            colourSelector2.SetColour(_Material._SpecularColour, false);
            colourSelector3.SetColour(_Material._Reflectivity, false);

            RenderThumbs();

            RenderPreview();
        }

        public void GUIUpdate()
        {
            _Material._Roughness = (float)floatEditor1.GetSliderValue();
            _Material._DiffuseColour = colourSelector1.GetColour();
            _Material._SpecularColour = colourSelector2.GetColour();
            _Material._Reflectivity = colourSelector3.GetColour();

            RenderThumbs();

            RenderPreview();
        }

        public void RenderPreview()
        {
            _Clean = true;
        }

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

        OpenGL _GL;
        PostProcess _PostProcess;
        Scene _Scene;
        RaytracerOptions _RaytracerOptions;
        ShaderRenderer _ShaderRenderer;

        
        private void InitialiseRenderer()
        {
            _RaytracerOptions = new RaytracerOptions();
            _RaytracerOptions._DoFEnabled = false;
            _RaytracerOptions._ShadowsEnabled = true;
            _RaytracerOptions._Reflections = 1;

            _Scene = new Scene();
            _Scene._FractalSettings._FractalIterations.Clear();
            _Scene._FractalSettings._FractalColours.Clear();
            _Scene._FractalSettings._RenderOptions._Background = 1;
            FractalGradient preview = new FractalGradient();
            preview._StartColour = _Material;
            preview._Multiplier = 0.0f;
            _Scene._FractalSettings._FractalColours.Add(preview);
            _Scene._Camera._Position = new Vector3(0, 1.5, 1.5);
            _Scene._Camera._Target = new Vector3(0, 1, 0);

            _PostProcess = new PostProcess();
//            _PostProcess._ToneMappingMode = 3;
            _PostProcess.Initialise(_GL);

            //  Initialise the scene.
            string frag = "";
            _Scene.Compile(_RaytracerOptions, _Scene._FractalSettings._RenderOptions, ref frag);
            
            _ShaderRenderer = new ShaderRenderer();
            _ShaderRenderer.Compile(_GL, frag, 16);
            
            int width, height;
            width = (int)openGlCtrl.ActualWidth;
            height = (int)openGlCtrl.ActualHeight;
            _ShaderRenderer.Initialise(_GL, width, height, _Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition());
            _ShaderRenderer.SetCameraVars(_Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition(), _Scene._FractalSettings._RenderOptions.GetSunVec3(), _Scene._Camera);
            _ShaderRenderer.SetProgressive(_RaytracerOptions._Progressive);
            _ShaderRenderer.SetPostProcess(_PostProcess);
            _ShaderRenderer.Clean(_GL);
            _ShaderRenderer.Start();
        }


        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            if (_Clean)
            {
                InitialiseRenderer();
                _ShaderRenderer.Clean(gl);
                _Clean = false;
                _ShaderRenderer.Start();
            }
            _ShaderRenderer.Render(gl);
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            _GL = args.OpenGL;
            InitialiseRenderer();
        }

        private void OpenGL_Closing()
        {
            _ShaderRenderer.Destroy(_GL);
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            _GL = args.OpenGL;
            InitialiseRenderer();
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image clicked = (Image)sender;
            int number;
            bool parsed = int.TryParse(clicked.Name.Remove(0, 5), out number);
            if (parsed)
            {
                _Material = _MaterialSelection._Defaults[number - 1];
            }
            UpdateGUI();
        }
    }
}
