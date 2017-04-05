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
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using SharpGL;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;

namespace WooFractal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PostProcess _PostProcess;
        RaytracerOptions _RaytracerOptions = new RaytracerOptions();
        RaytracerOptions _FinalRTOptions = new RaytracerOptions();
        Scene _Scene = new Scene();

        public void LoadScratch()
        {
            LoadContext("scratch");
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // initialise post process settings
            _PostProcess = new PostProcess();
            _PostProcess._PostProcessFilter = 1;
            _FinalRTOptions._Progressive = true;
            _FinalRTOptions._MaxIterations = -1;

            // initialise the script objects
            LoadScratch();

            UpdateGUI();
        }

        public void AddCuboid()
        {
            //TODO Move this into AddFractal.xaml.cs
            _Scene._FractalSettings._FractalIterations.Add(new KIFSIteration(EFractalType.Cube, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 2.1, new Vector3(1, 1, 1), 1));
            BuildFractalList();
        }

        public void AddMenger()
        {
            _Scene._FractalSettings._FractalIterations.Add(new KIFSIteration(EFractalType.Menger, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 3.0, new Vector3(1, 1, 1), 1));
            BuildFractalList();
        }

        public void AddTetra()
        {
            _Scene._FractalSettings._FractalIterations.Add(new KIFSIteration(EFractalType.Tetra, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 2.0, new Vector3(1, 1, 1), 1));
            BuildFractalList();
        }

        public void AddMandelbulb()
        {
            _Scene._FractalSettings._FractalIterations.Add(new MandelbulbIteration(new Vector3(0, 0, 0), 8, 1));
            BuildFractalList();
        }

        public void AddMandelbox()
        {
            _Scene._FractalSettings._FractalIterations.Add(new MandelboxIteration(new Vector3(0, 0, 0), 2.1, 0.5, 1));
            BuildFractalList();
        }

        public void AddKleinianGroup()
        {
            _Scene._FractalSettings._FractalIterations.Add(new KleinianGroupIteration(new Vector3(0, 0, 0), 1, 0.5, 1));
            BuildFractalList();
        }

        private void BuildFractalList()
        {
            stackPanel1.Children.Clear();

            for (int i = 0; i < _Scene._FractalSettings._FractalIterations.Count(); i++)
            {
                stackPanel1.Children.Add(_Scene._FractalSettings._FractalIterations[i].GetControl());
            }

            stackPanel1.Children.Add(new AddFractal());
        }

        private void BuildOptionsList()
        {
            stackPanel2.Children.Clear();

            stackPanel2.Children.Add(_Scene._FractalSettings._RenderOptions.GetControl());
            stackPanel2.Children.Add(_RaytracerOptions.GetControl());
            stackPanel2.Children.Add(_Scene._Camera.GetControl());
        }

        private void BuildColourList()
        {
            stackPanel3.Children.Clear();

            stackPanel3.Children.Add(_Scene._FractalSettings._FractalColours.GetControl());
        }

//        ImageRenderer _ImageRenderer;
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Preview(true);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Compile();
        }

        private void Preview(bool preview)
        {
//            _ImageRenderer.Stop();
//            _ImageRenderer.SetFixedExposure(true);//!_WootracerOptions._AutoExposure);
            //_ImageRenderer.SetExposureValue((float)_WootracerOptions._Exposure);
            if (!preview)
            {
//                _ImageRenderer = new ImageRenderer(image1, BuildXML(false), 480, 270, false);   
//                _ImageRenderer.Render();
//                _ImageRenderer = new ImageRenderer(image1, BuildXML(false), (int)((float)480 * _Scale), (int)((float)270 * _Scale), false);
            }
            else
            {
//                _ImageRenderer.Render();
            }
            if (_RaytracerOptions._AutoExposure)
            {
//                _WootracerOptions._Exposure = _ImageRenderer._MaxValue;
            }
        }

        bool _Clean = true;

        public void Compile()
        {
            _Clean = true; // indicate to the OpenGL draw loop that it needs to clean the backbuffer

            string frag = "";
            _Scene.Compile(_RaytracerOptions, ref frag);
            _ShaderRenderer.Compile(_GL, frag, _RaytracerOptions.GetRaysPerPixel());

            SaveStatus();

            TriggerPreview();

            _Dirty = false;
        }

        private void TriggerPreview()
        {
            _SimpleLighting = getSimpleLighting();

            // reset the renderer
//            if (_ImageRenderer != null)
            {
//                _ImageRenderer.TransferLatest(false);
//                _ImageRenderer.Stop();
            }

//            _ImageRenderer = new ImageRenderer(image1, BuildXML(true), (int)image1.Width, (int)image1.Height, true);
//            _ImageRenderer.SetFixedExposure(true);//!_WootracerOptions._AutoExposure);
//            _ImageRenderer.SetExposureValue((float)_WootracerOptions._Exposure);
//            _ImageRenderer.Render();
        }

        Vector3 _Velocity = new Vector3(0, 0, 0);

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
//            Button source = e.Source as Button;
            if (sender != null)
            {
                double Multiplier = 0.1;
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    Multiplier = 0.01;
                }

                if (e.Key == Key.Left)
                {
                    _Velocity.x -= Multiplier * _Scene._Camera._FocusDepth;
                    e.Handled = true;
                    _CameraDirty = true;
                }
                else if (e.Key == Key.Right)
                {
                    _Velocity.x += Multiplier * _Scene._Camera._FocusDepth;
                    e.Handled = true;
                    _CameraDirty = true;
                }
                else if (e.Key == Key.Up)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _Velocity.z += Multiplier * _Scene._Camera._FocusDepth;
                        e.Handled = true;
                        _CameraDirty = true;
                    }
                    else
                    {
                        _Velocity.y += Multiplier * _Scene._Camera._FocusDepth;
                        e.Handled = true;
                        _CameraDirty = true;
                    }
                }
                else if (e.Key == Key.Down)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _Velocity.z -= Multiplier * _Scene._Camera._FocusDepth;
                        e.Handled = true;
                        _CameraDirty = true;
                    }
                    else
                    {
                        _Velocity.y -= Multiplier * _Scene._Camera._FocusDepth;
                        e.Handled = true;
                        _CameraDirty = true;
                    }
                }

                if (!_Timer.IsEnabled)
                    _Timer.Start();
            }
        }

        bool _ReInitialise = false;
        bool _Dirty = true;
        bool _CameraDirty = false;
        DispatcherTimer _Timer;

        public void SetDirty()
        {
            _Dirty = true;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Vector3 to = _Scene._Camera._Target - _Scene._Camera._Position;
            to.Normalise();
            Vector3 up = new Vector3(0, 1, 0);
            Vector3 right = up.Cross(to);
            right.Normalise();

            Vector3 newup = to.Cross(right);
            newup.Normalise();

            right.Mul(_Velocity.x);
            to.Mul(_Velocity.z);
            newup.Mul(_Velocity.y);

//            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                _Scene._Camera._Position.Add(right);
                _Scene._Camera._Position.Add(to);
                _Scene._Camera._Position.Add(newup);
                _Scene._Camera._Target.Add(right);
                _Scene._Camera._Target.Add(to);
                _Scene._Camera._Target.Add(newup);
            }
  /*          else
            {
                _Camera._Position.Add(right);
                _Camera._Position.Add(newup);
            }
            */
 //           _WootracerOptions.UpdateGUI();
 //           _WootracerOptions._FocusDistance = (_Camera._Target - _Camera._Position).Magnitude();

            _Velocity *= 0.6;

//            _ImageRenderer._RampValue = 1;// _ImageRenderer._MaxValue;
//            _ImageRenderer.TransferLatest(false);
            
            if (_ReInitialise)
            {
                _ShaderRenderer.Initialise(_GL, (int)openGlCtrl.ActualWidth / _RaytracerOptions._Resolution, (int)openGlCtrl.ActualHeight / _RaytracerOptions._Resolution, _Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition());
                _ShaderRenderer.SetProgressive(_RaytracerOptions._Progressive);
                _ShaderRenderer.SetMaxIterations(_RaytracerOptions._MaxIterations);
                _ReInitialise = false;
            }

            if (_Dirty)
            {
                Compile();
            }

            if (_CameraDirty || _Velocity.MagnitudeSquared() > 0.0001)
            {
                _Clean = true;
                _ShaderRenderer.SetCameraVars(_Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition());
                _CameraDirty = false;
            }

            if (_Velocity.MagnitudeSquared() < 0.0001)
                _CameraDirty = false;
        }

        private void image1_GotFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("GotFocus");
        }

        bool _ImageDrag = false;
        double _Pitch;
        double _Yaw;
        Point _DragStart;
        bool _SimpleLighting = false;

        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as System.Windows.IInputElement);
            Debug.WriteLine("LMBDown");

//            Point mousePos = e.GetPosition(image1);

//            GetDepth(ref depth, (int)(_Scale * mousePos.X), (int)(_Scale * mousePos.Y));
//            if (depth>0)
//                _WootracerOptions._FocusDistance = (double)depth;

            Vector3 dir = (_Scene._Camera._Target - _Scene._Camera._Position);
            dir.Normalise();

            _Pitch = Math.Asin(dir.y);
            dir.y = 0;
            dir.Normalise();
            _Yaw = Math.Asin(dir.x);
            if (dir.z < 0)
                _Yaw = (Math.PI) - _Yaw;

            _DragStart = e.GetPosition(this);
            Debug.WriteLine("dragstart (x = " + _DragStart.X + ", y=" + _DragStart.Y + ")");

            dir = (_Scene._Camera._Target - _Scene._Camera._Position);
            dir.Normalise();
            dir *= _Scene._Camera._FocusDepth;
            _Scene._Camera._Target = _Scene._Camera._Position + dir;
            _Scene._Camera.UpdateGUI();

            _ImageDrag = true;

        //    CaptureMouse();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_ImageDrag)
            {
                Point dragPos = e.GetPosition(this);
                dragPos.X -= _DragStart.X;
                dragPos.Y -= _DragStart.Y;
                Debug.WriteLine("dragpos (x = " + dragPos.X + ", y=" + dragPos.Y + ")");

                double newyaw = _Yaw - 0.01 * dragPos.X;
                double newpitch = _Pitch + 0.01 * dragPos.Y;
                if (newpitch > Math.PI * 0.49f) newpitch = Math.PI * 0.49f;
                if (newpitch < -Math.PI * 0.49f) newpitch = -Math.PI * 0.49f;
                Vector3 dir = _Scene._Camera._Target - _Scene._Camera._Position;
                double length = dir.Magnitude();

                Vector3 newdir = new Vector3();
                newdir.y = Math.Sin(newpitch);
                newdir.x = Math.Cos(newpitch) * Math.Sin(newyaw);
                newdir.z = Math.Cos(newpitch) * Math.Cos(newyaw);

                double mag = newdir.Magnitude();
                newdir *= length;

                _Scene._Camera._Target = _Scene._Camera._Position + newdir;
                _Scene._Camera.UpdateGUI();
                _CameraDirty = true;

                if (!_Timer.IsEnabled)
                    _Timer.Start();
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            TriggerPreview();
        }


        bool _PreviewRender = true;

        public void StopPreview()
        {
            _PreviewRender = false;
            MainCanvas.Children.Remove(openGlCtrl);
            _Timer.Stop();
//            _ImageRenderer.Stop();
//            _ImageRenderer = null;
        }

        public void StartPreview()
        {
            _PreviewRender = true;
            MainCanvas.Children.Add(openGlCtrl);
            Compile();
            _Timer.Start();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            SaveStatus(); 

            _Velocity = new Vector3(0, 0, 0);
//            _Scene._Camera._FocusDepth = (float)_Scene._Camera._FocusDepth;
//            _Scene._Camera._ApertureSize = (float)_Scene._Camera._ApertureSize * _Scene._Camera._FocusDepth;
//            _Scene._Camera._FOV = (float)_Scene._Camera._FieldOfView;
//            _Scene._Camera._Spherical = (float)_Scene._Camera._Spherical;
//            _Scene._Camera._Stereographic = (float)_CameraOptions._Stereographic;

            StopPreview();

            FinalRender ownedWindow = new FinalRender(ref _Scene, ref _FinalRTOptions, ref _PostProcess);

            ownedWindow.Owner = Window.GetWindow(this);
            ownedWindow.ShowDialog();

            StartPreview();
        }

        private void image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("ButtonLMBUp");
            _ImageDrag = false;
            Mouse.Capture(null);

        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
/*            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\XML";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = store;
            saveFileDialog1.Filter = "Scene XML (*.xml)|*.xml";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                // Save document
                string filename = saveFileDialog1.FileName;
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(XML);
                sw.Close();
            }*/
        }

        private bool getShadowsEnabled()
        {
            return _RaytracerOptions._ShadowsEnabled;
        }

        private bool getSimpleLighting()
        {
            return true;// (simpleLighting.IsChecked.HasValue && simpleLighting.IsChecked.Value);
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
                TriggerPreview();
        }

        private void SaveStatus()
        {
            SaveContext("scratch");
        }

        private void LoadContext(string name)
        {
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Scenes\\" + name + ".wsd";
            if (System.IO.File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                string sceneDescriptor = sr.ReadToEnd();
                using (XmlReader reader = XmlReader.Create(new StringReader(sceneDescriptor)))
                {
                    try
                    {
                        while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "CONTEXT")
                            {
                                reader.Read();
                                while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "OPTIONS")
                                    {
                                        _RaytracerOptions = new RaytracerOptions();
                                        _RaytracerOptions.LoadXML(reader);
                                    }
                                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "SCENE")
                                    {
                                        _Scene = new Scene();
                                        _Scene.LoadXML(reader);
                                    }
                                }
                                reader.Read();
                            }
                        }
                    }
                    catch (XmlException /*e*/)
                    {
                        _Scene = new Scene();
                    }
                }
                sr.Close();
            }

            BuildFractalList();

            BuildOptionsList();

            BuildColourList();
        }
        private void SaveContext(string name)
        {
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Scenes";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }
            string filename = store + "\\" + name + ".wsd";

            using (StreamWriter sw = new StreamWriter(filename))
            {
                try
                {
                    XElement context = new XElement("CONTEXT");
                    context.Add(_Scene.CreateElement());
                    context.Add(_RaytracerOptions.CreateElement());
                    sw.Write(context.ToString());
                    sw.Close();
                }
                catch (Exception /*e*/)
                {
                    // lets not get overexcited...
                }
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenGL_Closing();
            SaveStatus();
        }

        private void RefreshRender(object sender, TextChangedEventArgs e)
        {
            if (_Timer != null && !_Timer.IsEnabled)
                _Timer.Start();
        }

        private void RefreshRenderRouted(object sender, RoutedEventArgs e)
        {
            if (_Timer!=null && !_Timer.IsEnabled)
                _Timer.Start();
            if (this.IsLoaded)
                TriggerPreview();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
//            image1.Height = imagebutton.ActualHeight;
//            image1.Width = imagebutton.ActualWidth;
            Compile();

            // set up animation thread for the camera movement
            _Timer = new DispatcherTimer();
            _Timer.Interval = TimeSpan.FromMilliseconds(17);
            _Timer.Tick += this.timer_Tick;
            _Timer.Start();
        }

        private void imagebutton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
//            image1.Height = imagebutton.ActualHeight;
//            image1.Width = imagebutton.ActualWidth;

//            if (imagebutton.IsLoaded)
  //             TriggerPreview();
        }

        public void RemoveIteration(WooFractalIteration iteration)
        {
            _Scene._FractalSettings._FractalIterations.Remove(iteration);

            Compile();

            BuildFractalList();
        }

        public void PromoteIteration(WooFractalIteration iteration)
        {
            int index = -1;
            for (int i = 0; i < _Scene._FractalSettings._FractalIterations.Count; i++)
            {
                if (_Scene._FractalSettings._FractalIterations[i] == iteration)
                {
                    index = i;
                }
            }
            if (index != -1 && index>0)
            {
                _Scene._FractalSettings._FractalIterations[index] = _Scene._FractalSettings._FractalIterations[index - 1];
                _Scene._FractalSettings._FractalIterations[index - 1] = iteration;
            }

            Compile();

            BuildFractalList();
        }

        public void DemoteIteration(WooFractalIteration iteration)
        {
            int index = -1;
            for (int i = 0; i < _Scene._FractalSettings._FractalIterations.Count; i++)
            {
                if (_Scene._FractalSettings._FractalIterations[i] == iteration)
                {
                    index = i;
                }
            }
            if (index != -1 && index < _Scene._FractalSettings._FractalIterations.Count - 1)
            {
                _Scene._FractalSettings._FractalIterations[index] = _Scene._FractalSettings._FractalIterations[index + 1];
                _Scene._FractalSettings._FractalIterations[index + 1] = iteration;
            }

            Compile();

            BuildFractalList();
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            // load fractal
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooScripter\\Scenes";

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "scene"; // Default file name
            dlg.DefaultExt = ".wsd"; // Default file extension
            dlg.Filter = "Scene Descriptor|*.wsd"; // Filter files by extension
            dlg.InitialDirectory = store;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // get name of file
            if (result == true)
            {
                string filename = dlg.FileName;
                filename = filename.Substring(0, filename.IndexOf(".wsd"));
                filename = filename.Substring(filename.LastIndexOf("\\")+1, filename.Length - (filename.LastIndexOf("\\")+1));
                LoadContext(filename);
            }
        }

        ShaderRenderer _ShaderRenderer = new ShaderRenderer();

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            if (!_PreviewRender)
                return;

            var gl = args.OpenGL;

            if (_Clean)
            {
                _ShaderRenderer.Clean(gl);
                _Clean = false;
                _ShaderRenderer.Start();
            }

            _ShaderRenderer.Render(gl);

            if (_ShaderRenderer._ImageDepthSet)
            {
                SetFocusDistance(_ShaderRenderer._ImageDepth);
                _ShaderRenderer._ImageDepthSet = false;
                _Dirty = true;
            }
        }

        OpenGL _GL;

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            _GL = args.OpenGL;

            string version = _GL.GetString(OpenGL.GL_SHADING_LANGUAGE_VERSION);

            //  Initialise the scene.
            _ShaderRenderer.Initialise(_GL, 1, 1, _Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition());
            _ShaderRenderer.SetProgressive(_RaytracerOptions._Progressive);
            _ShaderRenderer.SetMaxIterations(_RaytracerOptions._MaxIterations);
        }

        private void OpenGL_Closing()
        {
            _ShaderRenderer.Destroy(_GL);
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;
  
            //  Initialise the scene.
            _ShaderRenderer.Initialise(_GL, (int)openGlCtrl.ActualWidth / _RaytracerOptions._Resolution, (int)openGlCtrl.ActualHeight / _RaytracerOptions._Resolution, _Scene._Camera.GetViewMatrix(), _Scene._Camera.GetPosition());
            _ShaderRenderer.SetProgressive(_RaytracerOptions._Progressive);
            _ShaderRenderer.SetMaxIterations(_RaytracerOptions._MaxIterations);
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            // save fractal
            string store = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Scenes";
            if (!System.IO.Directory.Exists(store))
            {
                System.IO.Directory.CreateDirectory(store);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = store;
            saveFileDialog1.Filter = "Scene Descriptor (*.wsd)|*.wsd";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                // Save document
                string filename = saveFileDialog1.FileName;
                filename = filename.Substring(0, filename.IndexOf(".wsd"));
                filename = filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - (filename.LastIndexOf("\\") + 1));
                SaveContext(filename);
            }
        }

        private void openGlCtrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as System.Windows.IInputElement);
            Debug.WriteLine("LMBDown");
  
            Point mousePos = e.GetPosition(openGlCtrl);

            _ShaderRenderer.GetDepth((int)(mousePos.X), (int)(mousePos.Y));

            Debug.WriteLine("click (x = " + mousePos.X + ", y=" + mousePos.Y + ")");

            Vector3 dir = (_Scene._Camera._Target - _Scene._Camera._Position);
            dir.Normalise();

            _Pitch = Math.Asin(dir.y);
            dir.y = 0;
            dir.Normalise();
            _Yaw = Math.Asin(dir.x);
            if (dir.z < 0)
                _Yaw = (Math.PI) - _Yaw;

            _DragStart = e.GetPosition(this);
       //     Debug.WriteLine("dragstart (x = " + _DragStart.X + ", y=" + _DragStart.Y + ")");

            dir = (_Scene._Camera._Target - _Scene._Camera._Position);
            dir.Normalise();
            dir *= _Scene._Camera._FocusDepth;
            _Scene._Camera._Target = _Scene._Camera._Position + dir;
            _Scene._Camera.UpdateGUI();

            _ImageDrag = true;
        }

        private void SetFocusDistance(float depth)
        {
            if (depth > 0)
            {
                _Scene._Camera._FocusDepth = (double)depth;
                _CameraDirty = true;
            }
        }

        private void openGlCtrl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("ButtonLMBUp");
            _ImageDrag = false;
            Mouse.Capture(null);
        }

        private void openGlCtrl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_ImageDrag)
            {
                Point dragPos = e.GetPosition(this);
                dragPos.X -= _DragStart.X;
                dragPos.Y -= _DragStart.Y;
                Debug.WriteLine("dragpos (x = " + dragPos.X + ", y=" + dragPos.Y + ")");

                double newyaw = _Yaw - 0.01 * dragPos.X;
                double newpitch = _Pitch + 0.01 * dragPos.Y;
                if (newpitch > Math.PI * 0.49f) newpitch = Math.PI * 0.49f;
                if (newpitch < -Math.PI * 0.49f) newpitch = -Math.PI * 0.49f;
                Vector3 dir = _Scene._Camera._Target - _Scene._Camera._Position;
                double length = dir.Magnitude();

                Vector3 newdir = new Vector3();
                newdir.y = Math.Sin(newpitch);
                newdir.x = Math.Cos(newpitch) * Math.Sin(newyaw);
                newdir.z = Math.Cos(newpitch) * Math.Cos(newyaw);

                double mag = newdir.Magnitude();
                newdir *= length;

                _Scene._Camera._Target = _Scene._Camera._Position + newdir;
                _Scene._Camera.UpdateGUI();
                _CameraDirty = true;

                if (!_Timer.IsEnabled)
                    _Timer.Start();
            }
        }

        private void UpdateGUI()
        {
            button3.Content = "Shadows : " + (_RaytracerOptions._ShadowsEnabled ? "On" : "Off");
            button6.Content = "Reflections : " + (_RaytracerOptions._Reflections.ToString());
            button7.Content = "Depth of Field : " + (_RaytracerOptions._DoFEnabled ? "On" : "Off");
            button8.Content = "Max Iterations : " + (_RaytracerOptions._MaxIterations);
            button10.Content = "Progressive : " + (_RaytracerOptions._Progressive ? "On" : "Off");
            button9.Content = "Resolution : " + ((_RaytracerOptions._Resolution == 1) ? "1" : (_RaytracerOptions._Resolution == 2) ? "1/2" : "1/4");

            _ShaderRenderer.SetProgressive(_RaytracerOptions._Progressive);
            _ShaderRenderer.SetMaxIterations(_RaytracerOptions._MaxIterations);
        }

        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            _RaytracerOptions._ShadowsEnabled = !_RaytracerOptions._ShadowsEnabled;
            _Dirty = true;
            UpdateGUI();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            _RaytracerOptions._Reflections++;
            if (_RaytracerOptions._Reflections > 3)
                _RaytracerOptions._Reflections = 0;
            _Dirty = true;
            UpdateGUI();
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            _RaytracerOptions._DoFEnabled = !_RaytracerOptions._DoFEnabled;
            _Dirty = true;
            UpdateGUI();
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            // Headlight
            _RaytracerOptions._MaxIterations = _RaytracerOptions._MaxIterations * 2;
            if (_RaytracerOptions._MaxIterations > 16)
                _RaytracerOptions._MaxIterations = 1;
            _Dirty = true;
            UpdateGUI();
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            // Resolution
            _RaytracerOptions._Resolution *= 2;
            if (_RaytracerOptions._Resolution > 4)
                _RaytracerOptions._Resolution = 1;
            _Dirty = true;
            _ReInitialise = true;
            UpdateGUI();
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            // Progressive
            _RaytracerOptions._Progressive = !_RaytracerOptions._Progressive;
            _Dirty = true;
            UpdateGUI();
        }
    }
}
