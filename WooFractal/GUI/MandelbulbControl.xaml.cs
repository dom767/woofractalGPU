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
            vectorEditor1.Set("Rotation", _Parent._Rotation, new Vector3(-Math.PI), new Vector3(Math.PI), VectorEditorFlags.Rotation, this);
            floatEditor1.Set("Scale", _Parent._Scale, -16, 16, FloatEditorFlags.None, this);
            vectorEditor2.Set("Julia", _Parent._Julia, new Vector3(-2), new Vector3(2), VectorEditorFlags.None, this);

            checkBox1.IsChecked = _Parent._JuliaMode;

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
            _Parent._Scale = floatEditor1.GetSliderValue();
            _Parent._Julia = vectorEditor2.GetSliderValue();

            _Parent._JuliaMode = checkBox1.IsChecked.HasValue ? checkBox1.IsChecked.Value : false;

            int repeats;
            if (int.TryParse(textBox1.Text, out repeats)
                && repeats >= 1 && repeats < 10)
            {
                _Parent._Repeats = repeats;
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        public MandelbulbIteration _Parent;

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            _Parent._JuliaMode = checkBox1.IsChecked.HasValue ? checkBox1.IsChecked.Value : false;
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }
    }
}
