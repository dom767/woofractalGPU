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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for WootracerControls.xaml
    /// </summary>
    public partial class WootracerControls : UserControl, IGUIUpdateable
    {
        WootracerOptions _Parent;
        public WootracerControls(WootracerOptions wootracerOptions)
        {
            InitializeComponent();

            _Parent = wootracerOptions;

            CreateGUI();
        }

        public void CreateGUI()
        {
            wooSlider6.Set(_Parent._Exposure, 0.1, 10, this);
            checkBox1.IsChecked = _Parent._AutoExposure;
        }

        public void GUIUpdate()
        {
            _Parent._Exposure = wooSlider6.GetSliderValue();
            _Parent._AutoExposure = checkBox1.IsChecked.HasValue && checkBox1.IsChecked.Value;

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void checkBox1_Modified(object sender, RoutedEventArgs e)
        {
            GUIUpdate();
        }
    }
}
