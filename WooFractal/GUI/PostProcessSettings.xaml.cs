using System;
using System.Globalization;
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
    /// Interaction logic for PostProcessSettings.xaml
    /// </summary>
    public partial class PostProcessSettings : Window
    {
        public int _Iterations
        {
            get { return (int)GetValue(_IterationsProperty); }
            set { SetValue(_IterationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Iterations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _IterationsProperty =
            DependencyProperty.Register("_Iterations", typeof(int), typeof(PostProcessSettings), new UIPropertyMetadata(1));

        public double _BoostPower
        {
            get { return (double)GetValue(_BoostPowerProperty); }
            set { SetValue(_BoostPowerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _BoostPower.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _BoostPowerProperty =
            DependencyProperty.Register("_BoostPower", typeof(double), typeof(PostProcessSettings), new UIPropertyMetadata(10.0));

        public double _SourceWeight
        {
            get { return (double)GetValue(_SourceWeightProperty); }
            set { SetValue(_SourceWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _SourceWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _SourceWeightProperty =
            DependencyProperty.Register("_SourceWeight", typeof(double), typeof(PostProcessSettings), new UIPropertyMetadata(0.9));

        public double _TargetWeight
        {
            get { return (double)GetValue(_TargetWeightProperty); }
            set { SetValue(_TargetWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _TargetWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _TargetWeightProperty =
            DependencyProperty.Register("_TargetWeight", typeof(double), typeof(PostProcessSettings), new UIPropertyMetadata(0.1));

        public int _GWidth
        {
            get { return (int)GetValue(_GWidthProperty); }
            set { SetValue(_GWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _Iterations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GWidthProperty =
            DependencyProperty.Register("_GWidth", typeof(int), typeof(PostProcessSettings), new UIPropertyMetadata(1));

        public double _GBoostPower
        {
            get { return (double)GetValue(_GBoostPowerProperty); }
            set { SetValue(_GBoostPowerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _BoostPower.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GBoostPowerProperty =
            DependencyProperty.Register("_GBoostPower", typeof(double), typeof(PostProcessSettings), new UIPropertyMetadata(10.0));

        public double _GSourceWeight
        {
            get { return (double)GetValue(_GSourceWeightProperty); }
            set { SetValue(_GSourceWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _SourceWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GSourceWeightProperty =
            DependencyProperty.Register("_GSourceWeight", typeof(double), typeof(PostProcessSettings), new UIPropertyMetadata(0.9));

        public double _GTargetWeight
        {
            get { return (double)GetValue(_GTargetWeightProperty); }
            set { SetValue(_GTargetWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for _TargetWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _GTargetWeightProperty =
            DependencyProperty.Register("_GTargetWeight", typeof(double), typeof(PostProcessSettings), new UIPropertyMetadata(0.1));

        FinalRender _Parent;
        PostProcess _PostProcess;
        public PostProcessSettings(ref PostProcess postProcess, FinalRender parent)
        {
            _PostProcess = postProcess;
            _Parent = parent;
            DataContext = this;

            _Iterations = _PostProcess._Settings5x5._Iterations;
            _BoostPower = _PostProcess._Settings5x5._BoostPower;
            _SourceWeight = _PostProcess._Settings5x5._SourceWeight;
            _TargetWeight = _PostProcess._Settings5x5._TargetWeight;

            _GWidth = _PostProcess._SettingsFastGaussian._Width;
            _GBoostPower = _PostProcess._SettingsFastGaussian._BoostPower;
            _GSourceWeight = _PostProcess._SettingsFastGaussian._SourceWeight;
            _GTargetWeight = _PostProcess._SettingsFastGaussian._TargetWeight;

            InitializeComponent();

            checkBox1.IsChecked = _PostProcess._Settings5x5._Enabled;
            checkBox2.IsChecked = _PostProcess._SettingsFastGaussian._Enabled;
            UpdateKernelUI();
        }

        private void UpdateKernelUI()
        {
            _KernelUpdating = true;
            int kidx = 0;
            g11.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g12.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g13.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g14.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g15.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g21.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g22.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g23.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g24.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g25.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g31.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g32.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g33.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g34.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g35.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g41.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g42.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g43.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g44.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g45.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g51.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g52.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g53.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g54.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            g55.Text = _PostProcess._Settings5x5._Kernel[kidx++].ToString(CultureInfo.InvariantCulture);
            _KernelUpdating = false;
        }

        private void refreshRender(object sender, RoutedEventArgs e)
        {
            _PostProcess._Settings5x5._Iterations = _Iterations;
            _PostProcess._Settings5x5._BoostPower = _BoostPower;
            _PostProcess._Settings5x5._SourceWeight = _SourceWeight;
            _PostProcess._Settings5x5._TargetWeight = _TargetWeight;
            _PostProcess._SettingsFastGaussian._Width = _GWidth;
            _PostProcess._SettingsFastGaussian._BoostPower = _GBoostPower;
            _PostProcess._SettingsFastGaussian._SourceWeight = _GSourceWeight;
            _PostProcess._SettingsFastGaussian._TargetWeight = _GTargetWeight;
            _Parent.updateRender();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _PostProcess.SetGaussian5x5();
            UpdateKernelUI();

            refreshRender(sender, e);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _PostProcess.SetLinear5x5();
            UpdateKernelUI();

            refreshRender(sender, e);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _PostProcess.SetStar5x5();
            UpdateKernelUI();

            refreshRender(sender, e);
        }

        bool _KernelUpdating = false;

        private void refreshRenderk(object sender, TextChangedEventArgs e)
        {
            if (!_KernelUpdating)
            {
                UpdateKernel();
                _Parent.updateRender();
            }
        }

        private void UpdateKernel()
        {
            float[] kernel = new float[25];
            int kidx = 0;
            float.TryParse(g11.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g12.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g13.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g14.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g15.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g21.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g22.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g23.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g24.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g25.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g31.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g32.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g33.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g34.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g35.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g41.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g42.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g43.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g44.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g45.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g51.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g52.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g53.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g54.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
            float.TryParse(g55.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _PostProcess._Settings5x5._Kernel[kidx++]);
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            _PostProcess._Settings5x5._Enabled = true;
            refreshRender(sender, e);
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            _PostProcess._Settings5x5._Enabled = false;
            refreshRender(sender, e);
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            _PostProcess._SettingsFastGaussian._Enabled = true;
            refreshRender(sender, e);
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            _PostProcess._SettingsFastGaussian._Enabled = false;
            refreshRender(sender, e);
        }
    }
}
