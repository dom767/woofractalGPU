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
        Scene _Scene = new Scene();

        public void LoadScratch()
        {
            LoadContext("scratch");
        }

        string _SettingsLocation;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            _SettingsLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WooFractal\\Settings.xml";

            // initialise post process settings
            _PostProcess = new PostProcess();

            // initialise the script objects
            LoadScratch();

            UpdateGUI();

            BuildFractalList();

            BuildOptionsList();

            BuildColourList();
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
        GPULight _GPULight = new GPULight();

        public void Compile()
        {
            _Clean = true;
            string frag = @"
#version 330
uniform float screenWidth;
uniform float screenHeight;
uniform sampler2D renderedTexture;
uniform sampler2D randomNumbers;
uniform float frameNumber;
uniform bool depth;
uniform float mouseX;
uniform float mouseY;
uniform float progressiveInterval;
float randomIndex;
float pixelIndex;
float sampleIndex;

in vec2 texCoord;
out vec4 FragColor;

void calculateLighting(in vec3 pos, in vec3 normal, in vec3 reflection, in float specularPower, out vec3 lightDiff, out vec3 lightSpec);

vec2 rand2d(vec3 co)
{
//    return vec2(fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453), fract(sin(dot(co.xy+vec2(243,71) ,vec2(12.9898,78.233))) * 43758.5453));
	uint clamppixel = uint(co.x)%3592;
	uint sequence = uint(uint(co.z)/1024)*4801 + uint(co.x)*uint(co.x) + uint(co.y);
	
	sequence = ((sequence >> 16) ^ sequence) * 0x45d9f3b;
    sequence = ((sequence >> 16) ^ sequence) * 0x45d9f3b;
    sequence = ((sequence >> 16) ^ sequence);

//  sequence = int(floor(fract(sin(dot(vec2(clamppixel, co.y*13) ,vec2(12.9898,78.233))) * 43758.5453)*1024));//floor(mod(sequence, 1024));

  uint x = uint(co.z) % 1024;
  uint y = sequence % 1024;

  vec4 rand = texture(randomNumbers, vec2((float(x)+0.5)/1024, (float(y)+0.5)/1024));
  return vec2(rand.x, rand.y);
}

//  See : http://lolengine.net/blog/2013/09/21/picking-orthogonal-vector-combing-coconuts
vec3 ortho(in vec3 v) {
    return abs(v[0]) > abs(v[2]) ? vec3(-v[1], v[0], 0.0) : vec3(0.0, -v[2], v[1]);
}

vec3 getSampleBiased(in vec3 dir, in float power)
{
	dir = normalize(dir);
	vec3 o1 = normalize(ortho(dir));
	vec3 o2 = normalize(cross(dir, o1));

    vec2 r = rand2d(vec3(pixelIndex, sampleIndex++, randomIndex));
	r.x = r.x * 2.0f * 3.14159265f;
	r.y = pow(r.y, 1.0f/(power+1.0f));
	float oneminus = sqrt(1.0f-r.y*r.y);
	return o1*cos(r.x)*oneminus+o2*sin(r.x)*oneminus+dir*r.y;
}

vec3 getRandomDirection3d()
{
	vec2 random2d = rand2d(vec3(pixelIndex, sampleIndex++, randomIndex));
	float azimuth = random2d.x * 2 * 3.14159265f;
	vec2 dir2d = vec2(cos(azimuth), sin(azimuth));
	float z = (2*random2d.y) - 1;
	vec2 planar = dir2d * sqrt(1-z*z);
	return vec3(planar.x, planar.y, z);
}

vec2 getRandomDisc()
{
 vec2 random = rand2d(vec3(pixelIndex, sampleIndex++, randomIndex));
 random = vec2(sqrt(random.x), 2*3.14159265*random.y);
 return vec2(random.x * cos(random.y), random.x * sin(random.y));
}

vec4 getBackgroundColour(vec3 dir)
{
  return vec4(0.5+0.5*dir.x, 0.5+0.5*dir.y, 0.5+0.5*dir.z, 0.0);
}

";

            _Scene._Camera.Compile(ref frag);

            _Scene._FractalSettings._FractalColours.Compile(ref frag);

            _Scene._FractalSettings.Compile(ref frag);

            frag += @"
float udBox(in vec3 p, in vec3 b, in vec3 c)
{
return length(max(abs(p-c)-b, 0.0));
}

float bkgScene(in vec3 p)
{
  return udBox(p, vec3(20,0.5,20), vec3(0,-1.5,0));
}

bool traceBackground(in vec3 pos, in vec3 dir, inout float dist, out vec3 out_pos, out vec3 normal, out vec3 out_diff, out vec3 out_spec)
{
 vec3 p = pos;
 float r = 0;
out_diff = vec3(0.6,0.6,0.6);
out_spec = vec3(0.3,0.3,0.3);
 for (int j=0; j<200; j++)
 {
  r = bkgScene(p);
  if (r>100)
   return false;
  if (r<0.001)
  {
    float normalTweak=0.0001f;
	normal = vec3(bkgScene(p+vec3(normalTweak,0,0)) - bkgScene(p-vec3(normalTweak,0,0)),
		bkgScene(p+vec3(0,normalTweak,0)) - bkgScene(p-vec3(0,normalTweak,0)),
		bkgScene(p+vec3(0,0,normalTweak)) - bkgScene(p-vec3(0,0,normalTweak)));
    float magSq = dot(normal, normal);
    if (magSq<=0.0000000001*normalTweak)
        normal = -dir;
    else
        normal /= sqrt(magSq);

   out_pos = p + normal*0.001;
   dist = length(out_pos - pos);
   return true;
  }
  p += 0.6 * r * dir;
 }
 return false;
}


bool trace(in vec3 pos, in vec3 dir, inout float dist, out vec3 out_pos, out vec3 normal, out vec3 out_diff, out vec3 out_spec)
{
 vec3 bkgpos, bkgnormal, bkgdiff, bkgspec;
 float bkgdist=dist;
 bool hitFractal = traceFractal(pos, dir, dist, out_pos, normal, out_diff, out_spec);
 bool hitBkg = traceBackground(pos, dir, bkgdist, bkgpos, bkgnormal, bkgdiff, bkgspec);
 if ((hitFractal && hitBkg && dist>bkgdist) || hitBkg && !hitFractal)
 {
  dist = bkgdist;
  out_pos = bkgpos;
  normal = bkgnormal;
  out_diff = bkgdiff;
  out_spec = bkgspec;
 }
 return (hitFractal || hitBkg);
}";

            _GPULight.Set(_RaytracerOptions);
            _GPULight.Compile(ref frag);

            string frag2 = @"
void main(void)
{
  vec2 q = texCoord.xy;
  vec3 pos;
  vec3 dir;
  vec2 xy = vec2(0.5*(texCoord.x+1)*screenWidth, 0.5*(texCoord.y+1)*screenHeight);

  vec4 buffer2Col = texture(renderedTexture, vec2((texCoord.x+1)*0.5, (texCoord.y+1)*0.5));
  randomIndex = buffer2Col.a;
  pixelIndex = xy.x-0.5 + ((xy.y-0.5) * screenWidth);
  sampleIndex = 0;

  if (depth) q = vec2(2*((mouseX/screenWidth)-0.5), 2*((mouseY/screenHeight)-0.5));

//  if (!clean && !depth && floor(rand(vec2(floor(xy.x/16), floor(xy.y/16)))*800.0f)!=mod(frameNumber, 800))
//  float val = floor(rand2d(vec3(floor(xy.x/64), floor(xy.y/64), randomIndex)).x*progressiveInterval);//mod (floor(xy.x/64) + 1 * floor(xy.y/64), progressiveInterval);
//  float comp = mod(frameNumber, progressiveInterval);
//  if (!clean && !depth && (val>comp+0.1 || val<comp-0.1))
//  {
//    FragColor = texture(renderedTexture, vec2((texCoord.x+1)*0.5, (texCoord.y+1)*0.5));
//    discard;
//  }

  getcamera(pos, dir, q, depth);
  
  vec3 out_pos, normal, out_diff, out_spec;
  float dist = 10000;
  float factor = 1.0;
  vec4 oCol = vec4(0.0);
  vec3 iterpos = pos;
  vec3 iterdir = dir;
  
  for (int i=0; i<(depth ? 1 : 1); i++)
  {
   bool hit = trace(iterpos, iterdir, dist, out_pos, normal, out_diff, out_spec);

   if (hit)
   {
    normal += 0.01 * getRandomDirection3d();
    normal = normalize(normal);
    vec3 reflection = iterdir - normal*dot(normal,iterdir)*2.0f;
    vec3 lightDiff = vec3(0,0,0);
    vec3 lightSpec = vec3(0,0,0);
    calculateLighting(out_pos, normal, reflection, 10, lightDiff, lightSpec);

    vec3 col = out_diff*lightDiff;
    oCol+=factor*vec4(col, 0.0);

    iterpos = out_pos;
    iterdir = reflection;
    factor *= 0.6;
   }
   else
   {
    oCol+=factor*getBackgroundColour(iterdir);
    break;
   }
  }

  oCol += buffer2Col;
  oCol.a += 1;
  
  if (depth)
  {
    if (dist>9999) dist = -1;
    FragColor = vec4(dist);
  }
  else
  {
    FragColor = oCol;
  }
}
";
            frag += frag2;

            _ShaderRenderer.Compile(_GL, frag);

//            _BackgroundScript._Program = backgroundDesc.Text;
//            _SceneScript._Program = sceneDesc.Text;
//            _LightingScript._Program = lightingDesc.Text;

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
            
            if (_Dirty || _CameraDirty || _Velocity.MagnitudeSquared() > 0.0001)// && _ImageRenderer != null)
            {
//                _ImageRenderer.Stop();
                if (_Dirty)
                {
                    Compile();
                }
                else
                {
                    Compile();
//                    _ImageRenderer.UpdateCamera(_Camera.CreateElement().ToString());
                }
//                _ImageRenderer.Render();
            }

            if (_Velocity.MagnitudeSquared() < 0.0001)
                _CameraDirty = false;
        }

        private void image1_GotFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("GotFocus");
        }

        [DllImport(@"coretracer.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetDepth(ref float depth, int x, int y);

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

        public void StopPreview()
        {
            _Timer.Stop();
//            _ImageRenderer.Stop();
//            _ImageRenderer = null;
        }

        public void StartPreview()
        {
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

            FinalRender ownedWindow = new FinalRender(ref _Scene._Camera, ref _PostProcess);

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
                LoadContext(filename);
            }
        }

        ShaderRenderer _ShaderRenderer = new ShaderRenderer();

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;

            //  Clear the color and depth buffer.
            gl.ClearColor(0f, 0f, 0f, 1f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            uint error = gl.GetError();

            _ShaderRenderer.Render(gl, _Clean);
            _Clean = false;

            if (_ShaderRenderer._ImageDepthSet)
            {
                SetFocusDistance(_ShaderRenderer._ImageDepth);
                _ShaderRenderer._ImageDepthSet = false;
            }
        }

        OpenGL _GL;

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            _GL = args.OpenGL;

            //  Initialise the scene.
            _ShaderRenderer.Initialise(_GL, 1, 1);
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
            _ShaderRenderer.Initialise(_GL, (int)ActualWidth, (int)ActualHeight);
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
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    try
                    {
                        XElement sceneElement = _Scene.CreateElement();
                        sw.Write(sceneElement.ToString());
                        sw.Close();
                    }
                    catch (Exception /*e*/)
                    {
                        // lets not get overexcited...
                    }
                }
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
        }

        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            _RaytracerOptions._ShadowsEnabled = !_RaytracerOptions._ShadowsEnabled;
            _Dirty = true;
            UpdateGUI();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            _RaytracerOptions._ReflectionsEnabled = !_RaytracerOptions._ReflectionsEnabled;
            _Dirty = true;
            UpdateGUI();
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            // DoF
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            // Headlight
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            // Colours
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            // Progressive
        }
    }
}
