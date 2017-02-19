using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GlmNet;
using System.Xml;
using System.IO;

namespace WooFractal
{
    [Serializable]
    public class Camera
    {
        public Vector3 _Position;
        public Vector3 _Target;
        public double _FOV;
        public double _FocusDepth;
        public double _ApertureSize;
        public double _Spherical;
        public double _Stereographic;

        public Camera()
        {
            _Position = new Vector3(-2,0,0);
            _Target = new Vector3(0, 0, 0);
            _FOV = 40;
            _FocusDepth = 1;
            _ApertureSize = 0.1;
            _Spherical = 0;
            _Stereographic = 0;
        }

        public XElement CreateElement()
        {
            return new XElement("CAMERA",
                new XAttribute("from", _Position),
                new XAttribute("target", _Target),
                new XAttribute("fov", _FOV),
                new XAttribute("apertureSize", _ApertureSize),
                new XAttribute("spherical", _Spherical),
                new XAttribute("stereographic", _Stereographic));
        }

        public void LoadXML(XmlReader reader)
        {
            XMLHelpers.ReadVector3(reader, "from", ref _Position);
            XMLHelpers.ReadVector3(reader, "target", ref _Target);
            XMLHelpers.ReadDouble(reader, "fov", ref _FOV);
            XMLHelpers.ReadDouble(reader, "apertureSize", ref _ApertureSize);
            XMLHelpers.ReadDouble(reader, "spherical", ref _Spherical);
            XMLHelpers.ReadDouble(reader, "stereographic", ref _Stereographic);
            reader.Read();
        }

        public void Compile(ref string frag)
        {
            Vector3 to = _Target - _Position;
            to.Normalise();
            Vector3 up = new Vector3(0, 1, 0);
            if (to.Dot(up) > 0.999f) up = new Vector3(0, 0, 1);
            Vector3 right = up.Cross(to);
            right.Normalise();
            up = to.Cross(right);
            up.Normalise();
          
            mat4 viewMatrix = mat4.identity();
            viewMatrix[0, 0] = (float)right.x;
            viewMatrix[0, 1] = (float)right.y;
            viewMatrix[0, 2] = (float)right.z;
            viewMatrix[0, 3] = 0;
            viewMatrix[1, 0] = (float)up.x;
            viewMatrix[1, 1] = (float)up.y;
            viewMatrix[1, 2] = (float)up.z;
            viewMatrix[1, 3] = 0;
            viewMatrix[2, 0] = (float)to.x;
            viewMatrix[2, 1] = (float)to.y;
            viewMatrix[2, 2] = (float)to.z;
            viewMatrix[2, 3] = 0;
            viewMatrix[3, 0] = 0;
            viewMatrix[3, 1] = 0;
            viewMatrix[3, 2] = 0;
            viewMatrix[3, 3] = 1;
            
            frag += @"
uniform vec3 camPos = vec3("+_Position.x+","+_Position.y+","+_Position.z+@");
uniform mat4 viewMatrix = mat4(" + Utils.mat4ToString(viewMatrix) + @");

void getcamera(out vec3 pos, out vec3 dir, in vec2 q, in bool depth)
{
float aspect = screenHeight/screenWidth;
if (!depth)
{
vec2 r = getRandomDisc()*0.5;
q.x += r.x/screenWidth;
q.y += r.y/screenHeight;
}
vec3 direction = vec3(q.x*" + _FOV+", q.y*"+_FOV+@"*aspect, 45);
direction = normalize(direction);
pos = camPos;

if (!depth)
{
vec3 offset = vec3(" + _ApertureSize + @"*(rand2d(vec3(pixelIndex, sampleIndex++, randomIndex))-vec2(0.5,0.5)), 0);

// get the focal point
vec3 focusedPoint = direction * " +_FocusDepth+ @";
	
// Calculate new direction from origin to focus point
direction = focusedPoint - offset;

pos += vec3(viewMatrix * vec4(offset, 0.0));
}

dir = normalize(vec3(viewMatrix * vec4(direction, 0.0)));
}";
        }
    }
}
