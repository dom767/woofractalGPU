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
using System.Windows.Threading;
using System.Diagnostics;
using SharpGL;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for FinalRender.xaml
    /// </summary>
    public partial class FinalRender : Window, IGUIUpdateable
    {
        public int _ImageWidth
        {
            get { return (int)GetValue(_ImageWidthProperty); }
            set { SetValue(_ImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ImageWidthProperty =
            DependencyProperty.Register("_ImageWidth", typeof(int), typeof(FinalRender), new UIPropertyMetadata(960));

        public int _ImageHeight
        {
            get { return (int)GetValue(_ImageHeightProperty); }
            set { SetValue(_ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ImageHeightProperty =
            DependencyProperty.Register("_ImageHeight", typeof(int), typeof(FinalRender), new UIPropertyMetadata(480));

        PostProcess _PostProcess;
        Scene _Scene;
        RaytracerOptions _RaytracerOptions;
        ShaderRenderer _ShaderRenderer;

        public FinalRender(ref Scene scene, ref RaytracerOptions raytracerOptions, ref PostProcess postprocess)
        {
            DataContext = this;
            InitializeComponent();

            _Scene = scene;
            _RaytracerOptions = raytracerOptions;
            _PostProcess = postprocess;

            UpdateGUI();
        }

        private void UpdateGUI()
        {
            button11.Content = "Shadows : " + (_RaytracerOptions._ShadowsEnabled ? "On" : "Off");
            button12.Content = "Reflections : " + (_RaytracerOptions._Reflections.ToString());
            button13.Content = "Depth of Field : " + (_RaytracerOptions._DoFEnabled ? "On" : "Off");

            wooSlider3.Set(_PostProcess._GammaFactor, 0.01, 5, this);
            wooSlider4.Set(_PostProcess._GammaContrast, 0.01, 5, this);
            wooSlider1.Set(_PostProcess._ToneFactor, 0.01, 5, this);
            wooSlider5.Set(_PostProcess._GaussianExposure, 1, 10, this);
            wooSlider6.Set(_PostProcess._GaussianSD, 2, 100, this);
            wooSlider7.Set(_PostProcess._PostProcessAmount, 0, 8, this);
            comboBox1.SelectedIndex = _PostProcess._ToneMappingMode;
        }

        public void GUIUpdate()
        {
            _PostProcess._GammaFactor = wooSlider3.GetSliderValue();
            _PostProcess._GammaContrast = wooSlider4.GetSliderValue();
            _PostProcess._ToneFactor = wooSlider1.GetSliderValue();
            _PostProcess._GaussianExposure = wooSlider5.GetSliderValue();
            _PostProcess._GaussianSD = wooSlider6.GetSliderValue();
            _PostProcess._PostProcessAmount = wooSlider7.GetSliderValue();

            _PostProcess.Initialise(_GL);
            _ShaderRenderer.SetPostProcess(_PostProcess);
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            _ShaderRenderer.Render(gl);
            UpdatePerfStats();
        }

        private void UpdatePerfStats()
        {
            double time = _ShaderRenderer.GetElapsedTime();
            perf1.Content = "Elapsed Time : " + time.ToString() + "s";
            perf2.Content = "Rays / sec : " + (int)(time==0 ? 0 : _ShaderRenderer.GetRayCount()/time);
            perf3.Content = "Samples / pixel : " + _ShaderRenderer.GetFrameCount();
        }

        OpenGL _GL;

        private void InitialiseRenderer()
        {
            //  Initialise the scene.
            _ShaderRenderer = new ShaderRenderer();
            string frag = "";
            _Scene.Compile(_RaytracerOptions, _Scene._FractalSettings._RenderOptions, ref frag);
            _ShaderRenderer.Compile(_GL, frag, _RaytracerOptions.GetRaysPerPixel());
            int width, height;
            GetWidthHeightSelection(out width, out height);
            _ShaderRenderer.SetShaderVars(_Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition(), _Scene._FractalSettings._RenderOptions.GetSunVec3(), _Scene._Camera, _Scene._FractalSettings);
            _ShaderRenderer.Initialise(_GL, width, height, _Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition(), true);
            _ShaderRenderer.SetProgressive(_RaytracerOptions._Progressive, progressiveSteps);
            _ShaderRenderer.SetPostProcess(_PostProcess);
            _ShaderRenderer.Clean(_GL);
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            _GL = args.OpenGL;
            _PostProcess.Initialise(_GL);
            InitialiseRenderer();
        }

        private void OpenGL_Closing()
        {
            _ShaderRenderer.Destroy(_GL);
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            _GL = args.OpenGL;

            InitialiseRenderer();
        }
        
        bool _ImageRendering;
        DispatcherTimer _Timer;
        int progressiveSteps;

        private void GetWidthHeightSelection(out int width, out int height)
        {
            width = 960;
            height = 540;
            if (radioButton3.IsChecked.HasValue && radioButton3.IsChecked.Value)
            {
                width = 480;
                height = 270;
            }
            if (radioButton5.IsChecked.HasValue && radioButton5.IsChecked.Value)
            {
                width = 1920;
                height = 1080;
            }
            if (radioButton6.IsChecked.HasValue && radioButton6.IsChecked.Value)
            {
                width = _ImageWidth;
                height = _ImageHeight;
            }
            progressiveSteps = 8 * (width / 960) * (height / 960);
            if (progressiveSteps < 1) progressiveSteps = 1;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            int width, height;
            GetWidthHeightSelection(out width, out height);

            if (_ShaderRenderer.GetTargetWidth() != width
                || _ShaderRenderer.GetTargetHeight() != height
                || _Dirty)
            {
                InitialiseRenderer();
                _Dirty = false;
            }

            _ShaderRenderer.Start();

            // set up animation thread for the camera movement
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(100);
            _Timer.Tick += this.timer_Tick;
            _Timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
  //          updateRender();
        }

        public void updateRender()
        {
            GUIUpdate();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // Stop Render
            _ShaderRenderer.Stop();
        }

        private void radioButton6_Checked(object sender, RoutedEventArgs e)
        {
            textBox12.IsEnabled = true;
            textBox13.IsEnabled = true;
        }

        private void radioButton6_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox12.IsEnabled = false;
            textBox13.IsEnabled = false;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // Save Image
            _ShaderRenderer.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
//            _Timer.Stop();
        }

        private void refreshRender(object sender, RoutedEventArgs e)
        {
            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void refreshRender(object sender, TextChangedEventArgs e)
        {
            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        bool _Dirty = false;
        private void button12_Click(object sender, RoutedEventArgs e)
        {
            //reflections
            _RaytracerOptions._Reflections++;
            if (_RaytracerOptions._Reflections > 3)
                _RaytracerOptions._Reflections = 0;
            _Dirty = true;
            UpdateGUI();
        }

        private void button11_Click(object sender, RoutedEventArgs e)
        {
            //shadows
            _RaytracerOptions._ShadowsEnabled = !_RaytracerOptions._ShadowsEnabled;
            _Dirty = true;
            UpdateGUI();
        }

        private void button13_Click(object sender, RoutedEventArgs e)
        {
            //dof
            _RaytracerOptions._DoFEnabled = !_RaytracerOptions._DoFEnabled;
            _Dirty = true;
            UpdateGUI();            
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_PostProcess!=null)
                _PostProcess._ToneMappingMode = comboBox1.SelectedIndex;
        }
    }
}
