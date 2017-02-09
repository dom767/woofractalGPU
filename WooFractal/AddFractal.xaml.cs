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
    /// Interaction logic for AddFractal.xaml
    /// </summary>
    public partial class AddFractal : UserControl
    {
        public AddFractal()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Add Cuboid
            ((MainWindow)System.Windows.Application.Current.MainWindow).AddCuboid();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // Add Menger
            ((MainWindow)System.Windows.Application.Current.MainWindow).AddMenger();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // Add Tetra
            ((MainWindow)System.Windows.Application.Current.MainWindow).AddTetra();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // Add Mandelbulb
            ((MainWindow)System.Windows.Application.Current.MainWindow).AddMandelbulb();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            // Add Mandelbox
            ((MainWindow)System.Windows.Application.Current.MainWindow).AddMandelbox();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            // Add Mandelbox
            ((MainWindow)System.Windows.Application.Current.MainWindow).AddKleinianGroup();
        }
    }
}
