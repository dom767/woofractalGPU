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

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for FinalRender.xaml
    /// </summary>
    public partial class FinalRender : Window
    {
        public double _Min
        {
            get { return (double)GetValue(_MinProperty); }
            set { SetValue(_MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MinProperty =
            DependencyProperty.Register("_Min", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _Max
        {
            get { return (double)GetValue(_MaxProperty); }
            set { SetValue(_MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MaxProperty =
            DependencyProperty.Register("_Max", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _MaxValue
        {
            get { return (double)GetValue(_MaxValueProperty); }
            set { SetValue(_MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _MaxValueProperty =
            DependencyProperty.Register("_MaxValue", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _Factor
        {
            get { return (double)GetValue(_FactorProperty); }
            set { SetValue(_FactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Factor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _FactorProperty =
            DependencyProperty.Register("_Factor", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _ToneFactor
        {
            get { return (double)GetValue(_ToneFactorProperty); }
            set { SetValue(_ToneFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _ToneFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _ToneFactorProperty =
            DependencyProperty.Register("_ToneFactor", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _GammaFactor
        {
            get { return (double)GetValue(_GammaFactorProperty); }
            set { SetValue(_GammaFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _GammaFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GammaFactorProperty =
            DependencyProperty.Register("_GammaFactor", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

        public double _GammaContrast
        {
            get { return (double)GetValue(_GammaContrastProperty); }
            set { SetValue(_GammaContrastProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _GammaContrast.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GammaContrastProperty =
            DependencyProperty.Register("_GammaContrast", typeof(double), typeof(FinalRender), new UIPropertyMetadata((double)0));

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

        public int _SamplesPerPixel
        {
            get { return (int)GetValue(_SamplesPerPixelProperty); }
            set { SetValue(_SamplesPerPixelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _SamplesPerPixel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _SamplesPerPixelProperty =
            DependencyProperty.Register("_SamplesPerPixel", typeof(int), typeof(FinalRender), new UIPropertyMetadata(0));

        public int _Recursions
        {
            get { return (int)GetValue(_RecursionsProperty); }
            set { SetValue(_RecursionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Recursions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _RecursionsProperty =
            DependencyProperty.Register("_Recursions", typeof(int), typeof(FinalRender), new UIPropertyMetadata(2));

        Camera _Camera;
        PostProcess _PostProcess;

        private void BuildXML()
        {
//            _XML = @"<VIEWPORT width=" + image1.Width + @" height=" + image1.Height + @"/>";
//            _XML += _Camera.CreateElement().ToString();
//            _XML += _Scene.CreateElement(false, false).ToString();
        }

        public FinalRender(ref Camera camera, ref PostProcess postprocess)
        {
/*            _Scene = scene;
            _Camera = camera;
            _Recursions = _Scene._Recursions;

            DataContext = this;
            InitializeComponent();
            _MaxValue = 1;
            _Factor = 1;
            _ToneFactor = 1.4;
            _GammaFactor = 0.7;
            _GammaContrast = 0.7;

            if (_Camera._AAEnabled)
                checkBox1.IsChecked = true;
            if (_Camera._DOFEnabled)
                checkBox2.IsChecked = true;
            if (_Scene._PathTracer)
                checkBox3.IsChecked = true;
            if (_Scene._Caustics)
                checkBox4.IsChecked = true;
            _SamplesPerPixel = _Camera._MinSamples;

            _PostProcess = postprocess;

            BuildXML();*/
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
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
//            _Camera._AAEnabled = true;
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
//            _Camera._AAEnabled = false;
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
//            _Camera._DOFEnabled = true;
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
//            _Camera._DOFEnabled = false;
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
            _Timer.Stop();
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

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_Recursions >= 0 && _Recursions < 64)
            {
//                _Scene._Recursions = _Recursions;
            }
        }
    }
}
