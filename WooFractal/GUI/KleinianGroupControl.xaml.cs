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
    /// Interaction logic for KleinianGroupControl.xaml
    /// </summary>
    public partial class KleinianGroupControl : UserControl, IGUIUpdateable
    {
        public KleinianGroupControl(KleinianGroupIteration parent)
        {
            _Parent = parent;

            InitializeComponent();

            RenderSwatches();
        }

        private void RenderSwatches()
        {
            floatEditor1.Set("Scale", _Parent._Scale, -2, 2, FloatEditorFlags.None, this);
            vectorEditor1.Set("Constant Size", _Parent._CSize, new Vector3(-4), new Vector3(4), VectorEditorFlags.None, this);
            vectorEditor2.Set("Julia", _Parent._Julia, new Vector3(-3), new Vector3(3), VectorEditorFlags.None, this);

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
            _Parent._Scale = floatEditor1.GetSliderValue();
            _Parent._CSize = vectorEditor1.GetSliderValue();
            _Parent._Julia = vectorEditor2.GetSliderValue();

            int repeats;
            if (int.TryParse(textBox1.Text, out repeats)
                && repeats >= 1 && repeats < 20
                && repeats != _Parent._Repeats)
            {
                _Parent._Repeats = repeats;
                ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetCameraDirty();
        }

        public KleinianGroupIteration _Parent;
    }
}
