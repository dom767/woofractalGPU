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
    /// Interaction logic for MaterialSelector.xaml
    /// </summary>
    public partial class MaterialSelector : UserControl
    {
        MaterialSelection _MaterialSelection;
        Material _Selected;
        IGUIUpdateable _GUIUpdateTarget;
        
        public MaterialSelector()
        {
            InitializeComponent();
        }

        public Material GetSelectedMaterial()
        {
            return _Selected;
        }

        public MaterialSelection GetMaterialSelection()
        {
            return _MaterialSelection;
        }

        public void Set(MaterialSelection materialSelection, Material material, IGUIUpdateable guiTarget)
        {
            _MaterialSelection = materialSelection;
            _Selected = material;
            _GUIUpdateTarget = guiTarget;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ValueUpdated(false);
        }

        private void ValueUpdated(bool trigger)
        {
            if (_MaterialSelection == null)
                return;

            _MaterialSelection._Defaults[0].RenderThumb(image1);
            _MaterialSelection._Defaults[1].RenderThumb(image2);
            _MaterialSelection._Defaults[2].RenderThumb(image3);
            _MaterialSelection._Defaults[3].RenderThumb(image4);
            _MaterialSelection._Defaults[4].RenderThumb(image5);
            _MaterialSelection._Defaults[5].RenderThumb(image6);

            _MaterialSelection._Defaults[6].RenderThumb(image7);
            _MaterialSelection._Defaults[7].RenderThumb(image8);
            _MaterialSelection._Defaults[8].RenderThumb(image9);
            _MaterialSelection._Defaults[9].RenderThumb(image10);
            _MaterialSelection._Defaults[10].RenderThumb(image11);
            _MaterialSelection._Defaults[11].RenderThumb(image12);

            _MaterialSelection._Defaults[12].RenderThumb(image13);
            _MaterialSelection._Defaults[13].RenderThumb(image14);
            _MaterialSelection._Defaults[14].RenderThumb(image15);
            _MaterialSelection._Defaults[15].RenderThumb(image16);
            _MaterialSelection._Defaults[16].RenderThumb(image17);
            _MaterialSelection._Defaults[17].RenderThumb(image18);

            _Selected.RenderThumb(current);
            
            if (trigger)
                _GUIUpdateTarget.GUIUpdate();
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image clicked = (Image)sender;
            int number;
            bool parsed = int.TryParse(clicked.Name.Remove(0, 5), out number);
            if (parsed)
            {
                _Selected = _MaterialSelection._Defaults[number-1];
            }
            ValueUpdated(true);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).StopPreview();

            MaterialEditor ownedWindow = new MaterialEditor(_MaterialSelection);

            ownedWindow.Owner = Window.GetWindow(this);
            ownedWindow.ShowDialog();
            if (ownedWindow._OK)
            {
                _MaterialSelection = ownedWindow._MaterialSelection;
                ValueUpdated(true);
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).StartPreview();
        }
        
    }
}
