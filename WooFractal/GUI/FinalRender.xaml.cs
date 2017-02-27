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

        private void BuildXML()
        {
//            _XML = @"<VIEWPORT width=" + image1.Width + @" height=" + image1.Height + @"/>";
//            _XML += _Camera.CreateElement().ToString();
//            _XML += _Scene.CreateElement(false, false).ToString();
        }

        public FinalRender(ref Scene scene, ref RaytracerOptions raytracerOptions, ref PostProcess postprocess)
        {
            _Scene = scene;
            _RaytracerOptions = raytracerOptions;
            _PostProcess = postprocess;
            DataContext = this;
            InitializeComponent();

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
        }

        public void GUIUpdate()
        {
            _PostProcess._ToneMappingMode = comboBox1.SelectedIndex;
            _PostProcess._GammaFactor = wooSlider3.GetSliderValue();
            _PostProcess._GammaContrast = wooSlider4.GetSliderValue();
            _PostProcess._ToneFactor = wooSlider1.GetSliderValue();

            _ShaderRenderer.SetPostProcess(_PostProcess);
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            _ShaderRenderer.Render(gl);
        }

        OpenGL _GL;

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            _GL = args.OpenGL;

            //  Initialise the scene.
            _ShaderRenderer = new ShaderRenderer();
            string frag = "";
            _Scene.Compile(_RaytracerOptions, ref frag);
            _ShaderRenderer.Compile(_GL, frag);
            _ShaderRenderer.Initialise(_GL, 1, 1);
            _ShaderRenderer.Clean(_GL);
        }

        private void OpenGL_Closing()
        {
            _ShaderRenderer.Destroy(_GL);
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;

            //  Initialise the scene.
            _ShaderRenderer = new ShaderRenderer();
            string frag="";
            _Scene.Compile(_RaytracerOptions, ref frag);
            _ShaderRenderer.Compile(gl, frag);
            _ShaderRenderer.Initialise(_GL, (int)ActualWidth, (int)ActualHeight);
            _ShaderRenderer.SetPostProcess(_PostProcess);
        }
        
        bool _ImageRendering;
        DispatcherTimer _Timer;

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            BuildXML();

            int width = 960;
            int height = 540;
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
//            _ImageRenderer = new ImageRenderer(image1, _XML, width, height, true);
//            _ImageRendering = true;

//            _ImageRenderer.SetPostProcess(_PostProcess);
//            _ImageRenderer.Render();

            _ShaderRenderer.Start();

            // set up animation thread for the camera movement
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(100);
            _Timer.Tick += this.timer_Tick;
            _Timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            updateRender();
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

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            PostProcessSettings ownedWindow = new PostProcessSettings(ref _PostProcess, this);

            ownedWindow.Owner = Window.GetWindow(this);
            ownedWindow.ShowDialog();

            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            // no blur
            _PostProcess._Settings5x5._Enabled = false;
            _PostProcess._SettingsFastGaussian._Enabled = false;

            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            // gaussian
            _PostProcess._Settings5x5._Enabled = false;
            _PostProcess._SettingsFastGaussian._Enabled = true;
            _PostProcess._SettingsFastGaussian._BoostPower = 1;
            _PostProcess._SettingsFastGaussian._Width = 50;
            _PostProcess._SettingsFastGaussian._SourceWeight = 0;
            _PostProcess._SettingsFastGaussian._TargetWeight = 1;

            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            // linear
            _PostProcess._Settings5x5._Enabled = true;
            _PostProcess._Settings5x5.SetLinear();
            _PostProcess._SettingsFastGaussian._Enabled = false;

            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            // bloom
            _PostProcess._Settings5x5._Enabled = false;
            _PostProcess._SettingsFastGaussian._Enabled = true;
            _PostProcess._SettingsFastGaussian._BoostPower = 8;
            _PostProcess._SettingsFastGaussian._Width = 20;
            _PostProcess._SettingsFastGaussian._SourceWeight = 1;
            _PostProcess._SettingsFastGaussian._TargetWeight = 1;

            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            // star
            _PostProcess._Settings5x5._Enabled = true;
            _PostProcess._Settings5x5.SetStar();
            _PostProcess._SettingsFastGaussian._Enabled = false;

            if (!_ImageRendering)
            {
                updateRender();
            }
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            // bloom 2
            _PostProcess._Settings5x5._Enabled = false;
            _PostProcess._SettingsFastGaussian._Enabled = true;
            _PostProcess._SettingsFastGaussian._BoostPower = 5;
            _PostProcess._SettingsFastGaussian._Width = 40;
            _PostProcess._SettingsFastGaussian._SourceWeight = 1;
            _PostProcess._SettingsFastGaussian._TargetWeight = 2;

            if (!_ImageRendering)
            {
                updateRender();
            }
        }
    }
}
