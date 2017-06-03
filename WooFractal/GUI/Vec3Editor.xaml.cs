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
    [Flags]
    public enum VectorEditorFlags
    {
        None = 0,
        Rotation = 1,
        Integer = 2
    };
}

namespace WooFractal.GUI
{
    /// <summary>
    /// Interaction logic for FloatEditor.xaml
    /// </summary>
    public partial class VectorEditor : UserControl, IGUIUpdateable
    {
        public VectorEditor()
        {
            DataContext = this;
            InitializeComponent();
        }

        IGUIUpdateable _GUIUpdateTarget;
        string _LabelName;
        VectorEditorFlags _Flags;

        public void Set(string labelName, Vector3 value, Vector3 min, Vector3 max, VectorEditorFlags flags, IGUIUpdateable guiTarget)
        {
            _LabelName = labelName;
            _Flags = flags;
            wooSlider1.Set(value.x, min.x, max.x, this);
            wooSlider2.Set(value.y, min.y, max.y, this);
            wooSlider3.Set(value.z, min.z, max.z, this);
            if ((_Flags & VectorEditorFlags.Rotation) > 0)
            {
                label2.Content = "R";
                label3.Content = "P";
                label4.Content = "Y";
            }

            _GUIUpdateTarget = guiTarget;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ValueUpdated(false);
        }

        private void ValueUpdated(bool trigger)
        {
            double valuex = wooSlider1.GetSliderValue();
            double valuey = wooSlider2.GetSliderValue();
            double valuez = wooSlider3.GetSliderValue();
            if ((_Flags & VectorEditorFlags.Integer) > 0)
                label1.Content = _LabelName + " : {" + valuex.ToString("0") + ", " + valuey.ToString("0") + ", " + valuez.ToString("0") + "}";
            else
                label1.Content = _LabelName + " : {" + valuex.ToString("0.##") + ", " + valuey.ToString("0.##") + ", " + valuez.ToString("0.##") + "}";

            if (trigger)
                _GUIUpdateTarget.GUIUpdate();
        }
        
        public Vector3 GetSliderValue()
        {
            return new Vector3(wooSlider1.GetSliderValue(), wooSlider2.GetSliderValue(), wooSlider3.GetSliderValue());
        }

        bool _ValueDrag;
        Point _LastPos;

        public void GUIUpdate()
        {
            ValueUpdated(true);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            directEdit.Visibility = System.Windows.Visibility.Visible;
            textBox1.Text = ((float)wooSlider1.GetSliderValue()).ToString();
            textBox2.Text = ((float)wooSlider2.GetSliderValue()).ToString();
            textBox3.Text = ((float)wooSlider3.GetSliderValue()).ToString();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            directEdit.Visibility = System.Windows.Visibility.Hidden;
            double value;
            if (double.TryParse(textBox1.Text, out value))
            {
                wooSlider1.SetSliderValue(value);
            }
            if (double.TryParse(textBox2.Text, out value))
            {
                wooSlider2.SetSliderValue(value);
            }
            if (double.TryParse(textBox3.Text, out value))
            {
                wooSlider3.SetSliderValue(value);
            }
        }
    }
}
