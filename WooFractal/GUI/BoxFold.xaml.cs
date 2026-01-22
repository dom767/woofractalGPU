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
using WooFractal.Objects;

namespace WooFractal.GUI
{
    /// <summary>
    /// Interaction logic for BoxFold.xaml
    /// </summary>
    public partial class BoxfoldControl : UserControl, IGUIUpdateable
    {
        public BoxfoldControl(BoxfoldIteration boxFold)
        {
            _Parent = boxFold;
            InitializeComponent();
            RenderGUI();
        }
        private void RenderGUI()
        {
            vectorEditor1.Set("FoldScale", _Parent._FoldRadius, new Vector3(-4), new Vector3(4), VectorEditorFlags.None, this);

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
            _Parent._FoldRadius = vectorEditor1.GetSliderValue();

            int repeats;
            if (int.TryParse(textBox1.Text, out repeats)
                && repeats >= 1 && repeats < 100
                && repeats != _Parent._Repeats)
            {
                _Parent._Repeats = repeats;
                ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).SetCameraDirty();
        }

        public BoxfoldIteration _Parent;
    }
}
