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
    public enum FloatEditorFlags
    {
        None = 0,
        Integer = 1
    };
}

namespace WooFractal.GUI
{
    /// <summary>
    /// Interaction logic for FloatEditor.xaml
    /// </summary>
    public partial class FloatEditor : UserControl, IGUIUpdateable
    {
        public FloatEditor()
        {
            DataContext = this;
            InitializeComponent();
        }

        IGUIUpdateable _GUIUpdateTarget;
        string _LabelName;
        FloatEditorFlags _Flags;

        public void Set(string labelName, double value, double min, double max, FloatEditorFlags flags, IGUIUpdateable guiTarget)
        {
            _LabelName = labelName;
            _Flags = flags;
            wooSlider1.Set(value, min, max, this);
            _GUIUpdateTarget = guiTarget;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ValueUpdated(false);
        }

        private void ValueUpdated(bool trigger)
        {
            double value = wooSlider1.GetSliderValue();
            if ((_Flags&FloatEditorFlags.Integer)>0)
                label1.Content = _LabelName + " : " + value.ToString("0");
            else
                label1.Content = _LabelName + " : " + value.ToString("0.##");

            if (trigger)
                _GUIUpdateTarget.GUIUpdate();
        }
        
        public double GetSliderValue()
        {
            return wooSlider1.GetSliderValue();
        }

        public void SetSliderValue(float value, bool guiUpdate)
        {
            wooSlider1.SetSliderValue(value, guiUpdate);
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
            textBox1.Text = wooSlider1.GetSliderValue().ToString();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            directEdit.Visibility = System.Windows.Visibility.Hidden;
            double value;
            if (double.TryParse(textBox1.Text, out value))
            {
                wooSlider1.SetSliderValue(value, true);
            }
        }
    }
}
