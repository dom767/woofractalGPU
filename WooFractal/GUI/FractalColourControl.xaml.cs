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
using System.Diagnostics;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for FractalColourControl.xaml
    /// </summary>
    public partial class FractalColourControl : UserControl, IGUIUpdateable
    {
        FractalGradient _FractalColours = new FractalGradient();
        MaterialSelection _MaterialSelection;
        int _CurrentSegment = 0;

        public FractalColourControl(FractalGradient fractalColours, MaterialSelection materialSelection)
        {
            InitializeComponent();

            _FractalColours = fractalColours;
            _MaterialSelection = materialSelection;

            SetupGUI();
        }

        public void SetupGUI()
        {
            comboBox2.SelectedIndex = _FractalColours.GetOrbitTypeIndex();
            floatEditor1.Set("Multiplier", _FractalColours._Multiplier, 0, 10, FloatEditorFlags.None, this);
            floatEditor2.Set("Offset", _FractalColours._Offset, 0, 1, FloatEditorFlags.None, this);
            floatEditor3.Set("Power", _FractalColours._Power, -2, 2, FloatEditorFlags.None, this);

            materialSelector1.Set(_MaterialSelection, _FractalColours._GradientSegments[0]._StartColour, this);
            materialSelector2.Set(_MaterialSelection, _FractalColours._GradientSegments[0]._EndColour, this);

            RenderGradient();
        }

        public void GUIUpdate()
        {
            _FractalColours.SetOrbitTypeIndex(comboBox2.SelectedIndex);
            _FractalColours._Multiplier = floatEditor1.GetSliderValue();
            _FractalColours._Offset = floatEditor2.GetSliderValue();
            _FractalColours._Power = floatEditor3.GetSliderValue();

            _FractalColours._GradientSegments[_CurrentSegment]._StartColour = materialSelector1.GetSelectedMaterial();
            _FractalColours._GradientSegments[_CurrentSegment]._EndColour = materialSelector2.GetSelectedMaterial();

            _MaterialSelection = materialSelector1.GetMaterialSelection();

            RenderGradient();

            SetCameraDirty();
        }

        private void SetCameraDirty()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetCameraDirty();
        }

        private void SetDirty()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SetDirty();
        }

        private void RenderGradient()
        {
            int width = (int)image1.Width;
            int height = (int)image1.Height;

            byte[] pixels = new byte[4 * height * width];
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            float lightPos = 0.7f;
            int p = 0;
            int segmentIdx = 0;
            int leftSelect = (int)(_FractalColours._GradientSegments[_CurrentSegment]._StartX * width);
            int rightSelect = (int)(_FractalColours._GradientSegments[_CurrentSegment]._EndX * (width))-1;
            for (int x = 0; x < width; x++)
            {
                float floatx = (float)x / (float)(width-1);
                if (floatx > _FractalColours._GradientSegments[segmentIdx]._EndX-0.0001 && segmentIdx<_FractalColours._GradientSegments.Count-1) // fucking floating point fuck
                    segmentIdx++;

                Material interpmat = _FractalColours._GradientSegments[segmentIdx].GetMaterial(floatx);

                for (int y = 0; y < height; y++)
                {
                    Colour pixelColour;

                    if (x == leftSelect || x == rightSelect || (segmentIdx == _CurrentSegment && y == 0))
                    {
                        pixelColour = new Colour(0, 0, 0);
                    }
                    else
                    {
                        float floaty = 1.0f - (float)y / (float)(width - 1); // deliberate width to keep aspect ratio 1:1

                        float dy = lightPos - floaty;
                        float dist = (float)Math.Sqrt((double)(dy * dy));

                        float highlight = (float)Math.Exp((float)(-dist) / Math.Sqrt((double)interpmat._Roughness));// (float)Math.Pow((double)dist, (double)(material._Roughness));
                        pixelColour = interpmat._DiffuseColour + interpmat._SpecularColour * highlight;
                        pixelColour.Clamp(0, 1);
                    }

                    p = x + y * width;
                    pixels[p * 4] = (byte)(pixelColour._Blue * 255.99f);
                    pixels[p * 4 + 1] = (byte)(pixelColour._Green * 255.99f);
                    pixels[p * 4 + 2] = (byte)(pixelColour._Red * 255.99f);
                    pixels[p * 4 + 3] = 255;
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);

            writeableBitmap.WritePixels(rect, pixels, width * 4, (int)0);

            image1.Source = writeableBitmap;
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _FractalColours.SetOrbitTypeIndex(comboBox2.SelectedIndex);
            SetDirty();
        }

        float _DragStart = 0.0f;
        bool _ValueDrag = false;
        int _DragEdge = 0;

        public int GetNearestEdge(float value)
        {
            float nearestDistance = 1.0f;
            int nearestEdge = 0;
            for (int i = 0; i < _FractalColours._GradientSegments.Count() - 1; i++)
            {
                float dist = Math.Abs(value - _FractalColours._GradientSegments[i]._EndX);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearestEdge = i;
                }
            }
            if (nearestDistance < 0.05f) // tolerance
            {
                return nearestEdge;
            }
            return -1;
        }

        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clkPosition = e.GetPosition(image1);
            float left = (float)clkPosition.X / ((float)image1.ActualWidth - 1.0f);

            int edge = GetNearestEdge(left);
            if (edge > -1)
            {
                _DragEdge = edge;
                Mouse.Capture(sender as System.Windows.IInputElement);
                _ValueDrag = true;
                _DragStart = left;
            }

            _CurrentSegment = 0;
            for (int i = 0; i < _FractalColours._GradientSegments.Count(); i++)
            {
                if (left > _FractalColours._GradientSegments[i]._StartX)
                    _CurrentSegment++;
            }
            _CurrentSegment--;

            if (e.ClickCount == 2)
            {
                if (GetNearestEdge(left) == -1)
                {
                    Material mat = _FractalColours._GradientSegments[_CurrentSegment].GetMaterial(left);

                    GradientSegment segment = new GradientSegment();
                    segment._StartX = _FractalColours._GradientSegments[_CurrentSegment]._StartX;
                    segment._StartColour = _FractalColours._GradientSegments[_CurrentSegment]._StartColour.Clone();
                    segment._EndX = left;
                    segment._EndColour = mat.Clone();

                    _FractalColours._GradientSegments[_CurrentSegment]._StartX = left;
                    _FractalColours._GradientSegments[_CurrentSegment]._StartColour = mat.Clone();

                    _FractalColours._GradientSegments.Insert(_CurrentSegment, segment);

                    SetDirty();
                }
            }

            materialSelector1.SetSelectedMaterial(_FractalColours._GradientSegments[_CurrentSegment]._StartColour);
            materialSelector2.SetSelectedMaterial(_FractalColours._GradientSegments[_CurrentSegment]._EndColour);


            GUIUpdate();
        }

        private void SetValue(float position)
        {
            float newPos = _FractalColours._GradientSegments[_DragEdge]._EndX + (position - _DragStart);
            newPos = Math.Min(_FractalColours._GradientSegments[_DragEdge + 1]._EndX - 0.05f, newPos);
            newPos = Math.Max(_FractalColours._GradientSegments[_DragEdge]._StartX + 0.05f, newPos);

            _FractalColours._GradientSegments[_DragEdge]._EndX = newPos;
            _DragStart = newPos;
            _FractalColours._GradientSegments[_DragEdge + 1]._StartX = newPos;
            GUIUpdate();
        }

        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_ValueDrag)
            {
                Mouse.Capture(null);

                Point clkPosition = e.GetPosition(image1);
                float left = (float)clkPosition.X / ((float)image1.ActualWidth - 1.0f);

                SetValue(left);

                _ValueDrag = false;
            }

        }

        private void image1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_ValueDrag)
            {
                Point clkPosition = e.GetPosition(image1);
                float left = (float)clkPosition.X / ((float)image1.ActualWidth - 1.0f);

                SetValue(left);
            }
        }

        public void DeleteSelectedGradient()
        {
            if (_FractalColours._GradientSegments.Count() > 1)
            {
                if (_CurrentSegment == 0)
                {
                    return;
                }
                else if (_CurrentSegment == _FractalColours._GradientSegments.Count() - 1)
                {
                    _FractalColours._GradientSegments[_CurrentSegment - 1]._EndX = 1.0f;
                    _FractalColours._GradientSegments.RemoveAt(_CurrentSegment);
                    _CurrentSegment--;
                }
                else
                {
                    _FractalColours._GradientSegments[_CurrentSegment - 1]._EndX = _FractalColours._GradientSegments[_CurrentSegment]._StartX;
                    _FractalColours._GradientSegments[_CurrentSegment + 1]._StartX = _FractalColours._GradientSegments[_CurrentSegment]._StartX;
                    _FractalColours._GradientSegments.RemoveAt(_CurrentSegment);
                }

                SetDirty();
                GUIUpdate();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).RemoveFractalColour(_FractalColours);
            SetDirty();
        }
    }
 }
