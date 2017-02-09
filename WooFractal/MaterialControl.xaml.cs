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
    /// Interaction logic for MaterialControl.xaml
    /// </summary>
    public partial class MaterialControl : UserControl
    {
        Material _Material;
        IGUIUpdateable _GUIUpdateable;

        public MaterialControl()
        {
            InitializeComponent();

//            RenderMaterial();
        }

        public void SetMaterial(Material material, IGUIUpdateable guiUpdateable)
        {
            _Material = material;
            _GUIUpdateable = guiUpdateable;

            RenderMaterial();
        }

        public void RenderMaterial()
        {
            int width = (int)Width;
            int height = (int)Height;

            byte[] pixels = new byte[4 * height * width];
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            for (int p=0; p<width*height; p++)
            {
                Colour fillColour = _Material._DiffuseColour;
                fillColour.Clamp(0, 1);
                pixels[p*4] = (byte)(fillColour._Blue * 255.99f);
                pixels[p*4+1] = (byte)(fillColour._Green * 255.99f); 
                pixels[p*4+2] = (byte)(fillColour._Red * 255.99f); 
                pixels[p*4+3] = 255;
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);

            writeableBitmap.WritePixels(rect, pixels, width * 4, (int)0);

            MaterialImage.Source = writeableBitmap;

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).StopPreview();

            MaterialEditor ownedWindow = new MaterialEditor(_Material);

            ownedWindow.Owner = Window.GetWindow(this);
            ownedWindow.ShowDialog();
            if (ownedWindow._OK)
            {
                _Material = ownedWindow._Material;
                _GUIUpdateable.GUIUpdate();
                RenderMaterial();
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).StartPreview();
        }

        public Material GetMaterial()
        {
            return _Material;
        }
    }
}
