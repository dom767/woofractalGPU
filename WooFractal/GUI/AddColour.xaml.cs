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

namespace WooFractal.GUI
{
    /// <summary>
    /// Interaction logic for AddColour.xaml
    /// </summary>
    public partial class AddColour : UserControl
    {
        public AddColour()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Add FractalColour
            ((MainWindow)System.Windows.Application.Current.MainWindow).AddFractalColours();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).LoadColours();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SaveColours();
        }
    }
}
