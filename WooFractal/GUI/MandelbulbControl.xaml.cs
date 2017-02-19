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
    /// Interaction logic for MandelbulbControl.xaml
    /// </summary>
    public partial class MandelbulbControl : UserControl, IGUIUpdateable
    {
        public MandelbulbControl(MandelbulbIteration parent)
        {
            _Parent = parent;

            InitializeComponent();

            RenderSwatches();
        }

        private void RenderSwatches()
        {
            wooSlider1.Set(_Parent._Rotation.x, -Math.PI, Math.PI, this);
            wooSlider2.Set(_Parent._Rotation.y, -Math.PI, Math.PI, this);
            wooSlider3.Set(_Parent._Rotation.z, -Math.PI, Math.PI, this);

            wooSlider10.Set(_Parent._Scale, -16, 16, this);

            textBox1.Text = _Parent._Repeats.ToString();
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).RemoveIteration(_Parent);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).PromoteIteration(_Parent);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).DemoteIteration(_Parent);
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            GUIUpdate();
        }

        public void GUIUpdate()
        {
            _Parent._Rotation.x = wooSlider1.GetSliderValue();
            _Parent._Rotation.y = wooSlider2.GetSliderValue();
            _Parent._Rotation.z = wooSlider3.GetSliderValue();

            _Parent._Scale = wooSlider10.GetSliderValue();

            int repeats;
            if (int.TryParse(textBox1.Text, out repeats)
                && repeats >= 1 && repeats < 10)
            {
                _Parent._Repeats = repeats;
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        public MandelbulbIteration _Parent;
    }
}
