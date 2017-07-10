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
    /// Interaction logic for ColourSelector.xaml
    /// </summary>
    public partial class ColourSelector : UserControl, IGUIUpdateable
    {
        public ColourSelector()
        {
            InitializeComponent();
        }

        IGUIUpdateable _GUIUpdateTarget;
        Colour _Colour;
        HSLColour _HSLColour;

        public void Set(Colour colour, IGUIUpdateable guiUpdateTarget)
        {
            _GUIUpdateTarget = guiUpdateTarget;
            _Colour = colour;
            _HSLColour = new HSLColour(colour);
            RenderSwatches();
        }

        private void RenderSwatches()
        {
            HSLColour col = new HSLColour(1, 0.5, 0.5);
            Colour bob = col.GetColour();
            RenderHS();
            RenderL();
            RenderCol();
        }

        private void RenderCol()
        {
            int width = (int)image1.Width;
            int height = (int)image1.Height;

            byte[] pixels = new byte[4 * height * width];
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            int p = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    HSLColour hslcol = new HSLColour(_HSLColour._H, _HSLColour._S, _HSLColour._L);
                    Colour col = hslcol.GetColour();

                    col.Clamp(0, 1);
                    pixels[p * 4] = (byte)(col._Blue * 255.99f);
                    pixels[p * 4 + 1] = (byte)(col._Green * 255.99f);
                    pixels[p * 4 + 2] = (byte)(col._Red * 255.99f);
                    pixels[p * 4 + 3] = 255;
                    p++;
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);

            writeableBitmap.WritePixels(rect, pixels, width * 4, (int)0);

            image1.Source = writeableBitmap;
        }

        private void RenderHS()
        {
            int width = (int)image2.Width;
            int height = (int)image2.Height;

            byte[] pixels = new byte[4 * height * width];
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            int p = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float floatx = (float)x / (float)(width - 1);
                    float floaty = (float)y / (float)(height - 1);

                    HSLColour hslcol = new HSLColour(floatx, floaty, _HSLColour._L);
                    Colour col = hslcol.GetColour();

                    col.Clamp(0, 1);
                    pixels[p * 4] = (byte)(col._Blue * 255.99f);
                    pixels[p * 4 + 1] = (byte)(col._Green * 255.99f);
                    pixels[p * 4 + 2] = (byte)(col._Red * 255.99f);
                    pixels[p * 4 + 3] = 255;
                    p++;
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);

            writeableBitmap.WritePixels(rect, pixels, width * 4, (int)0);

            image2.Source = writeableBitmap;
        }

        private void RenderL()
        {
            int width = (int)image3.Width;
            int height = (int)image3.Height;

            byte[] pixels = new byte[4 * height * width];
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            int p = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float floaty = (float)y / (float)(height - 1);

                    HSLColour hslcol = new HSLColour(_HSLColour._H, _HSLColour._S, floaty);
                    Colour col = hslcol.GetColour();

                    col.Clamp(0, 1);
                    pixels[p * 4] = (byte)(col._Blue * 255.99f);
                    pixels[p * 4 + 1] = (byte)(col._Green * 255.99f);
                    pixels[p * 4 + 2] = (byte)(col._Red * 255.99f);
                    pixels[p * 4 + 3] = 255;
                    p++;
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);

            writeableBitmap.WritePixels(rect, pixels, width * 4, (int)0);

            image3.Source = writeableBitmap;
        }

        public void GUIUpdate()
        {
//            _Colour._Red = wooSlider1.GetSliderValue();
  //          _Colour._Green = wooSlider2.GetSliderValue();
    //        _Colour._Blue = wooSlider3.GetSliderValue();

            _GUIUpdateTarget.GUIUpdate();

            UpdateSwatch();
        }

        private void UpdateSwatch()
        {
        }

        public Colour GetColour()
        {
            return _Colour;
        }

        public void SetColour(Colour colour, bool guiUpdate)
        {
            _Colour = colour;
            _HSLColour = new HSLColour(colour);
            RenderSwatches();
        }

        private void setHS(Point position)
        {
            double h = (double)(position.X) / (double)(image2.Width);
            double s = (double)(position.Y) / (double)(image2.Height);

            h = Math.Max(0.0, Math.Min(1.0, h));
            s = Math.Max(0.0, Math.Min(1.0, s));

            _HSLColour._H = h;
            _HSLColour._S = s;
            _Colour = _HSLColour.GetColour();

            RenderSwatches();
        }

        private void setL(Point position)
        {
            double l = (double)(position.Y) / (double)(image3.Height);
            l = Math.Max(0.0, Math.Min(1.0, l));

            _HSLColour._L = l;
            _Colour = _HSLColour.GetColour();

            RenderSwatches();
        }
        
        bool _DragHS = false;
        bool _DragL = false;

        private void image2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as System.Windows.IInputElement);
            setHS(e.GetPosition(image2));
            _DragHS = true;
        }

        private void image2_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_DragHS)
                setHS(e.GetPosition(image2));
        }

        private void image2_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            setHS(e.GetPosition(image2));
            _DragHS = false;

            _GUIUpdateTarget.GUIUpdate();
        }

        private void image3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as System.Windows.IInputElement);
            setL(e.GetPosition(image3));
            _DragL = true;
        }

        private void image3_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_DragL)
                setL(e.GetPosition(image3));

        }

        private void image3_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            setL(e.GetPosition(image3));
            _DragL = false;

            _GUIUpdateTarget.GUIUpdate();
        }
    }
}
