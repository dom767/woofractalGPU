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
    /// Interaction logic for MandelboxControl.xaml
    /// </summary>
    public partial class MandelboxControl : UserControl, IGUIUpdateable
    {
        public MandelboxControl(MandelboxIteration parent)
        {
            _Parent = parent;

            InitializeComponent();

            RenderSwatches();
        }

        private void RenderSwatches()
        {
            vectorEditor1.Set("Rotation", _Parent._Rotation, new Vector3(-Math.PI), new Vector3(Math.PI), VectorEditorFlags.Rotation, this);
            vectorEditor2.Set("Scale", _Parent._Scale, new Vector3(-4), new Vector3(4), VectorEditorFlags.None, this);
            floatEditor1.Set("Minimum Radius", _Parent._MinRadius, 0.01, 0.99, FloatEditorFlags.None, this);

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
            _Parent._Rotation = vectorEditor1.GetSliderValue();

            _Parent._MinRadius = floatEditor1.GetSliderValue();

            _Parent._Scale = vectorEditor2.GetSliderValue();

            int repeats;
            if (int.TryParse(textBox1.Text, out repeats)
                && repeats >= 1 && repeats < 10)
            {
                _Parent._Repeats = repeats;
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        public MandelboxIteration _Parent;
    }
}
