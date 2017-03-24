using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace WooFractal
{
    public class Scene
    {
        public Camera _Camera;
        public FractalSettings _FractalSettings;
        GPULight _GPULight;

        public Scene()
        {
            _Camera = new Camera();
            _FractalSettings = new FractalSettings();
            _GPULight = new GPULight();
        }

        public void LoadXML(XmlReader reader)
        {
            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "FRACTAL")
                {
                    _FractalSettings = new FractalSettings();
                    _FractalSettings.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "CAMERA")
                {
                    _Camera = new Camera();
                    _Camera.LoadXML(reader);
                }
            }
            reader.Read(); // finish off reading the scene
        }

        public string Compile(RaytracerOptions raytracerOptions, ref string frag)
        {
            frag = @"
#version 130
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

struct material
{
 vec3 diff;
 vec3 spec;
 float specPower;
 vec3 refl;
 float gloss;
 vec3 emi;
};

void calculateLighting(in vec3 pos, in vec3 normal, in vec3 reflection, in float specularPower, out vec3 lightDiff, out vec3 lightSpec);

vec2 rand2d(vec3 co)
{
    return vec2(fract(sin(dot(co.xyz ,vec3(12.9898,78.233,32.11))) * 43758.5453), fract(sin(dot(co.xyz+vec3(243,71,711) ,vec3(12.9898,78.233,32.11))) * 43758.5453));
	/*uint clamppixel = uint(co.x)%3592;
	uint sequence = uint(uint(co.z)/1024)*4801 + uint(co.x)*uint(co.x) + uint(co.y);
	
	sequence = ((sequence >> 16) ^ sequence) * 0x45d9f3b;
    sequence = ((sequence >> 16) ^ sequence) * 0x45d9f3b;
    sequence = ((sequence >> 16) ^ sequence);

  uint x = uint(co.z) % 1024;
  uint y = sequence % 1024;

  vec4 rand = texture(randomNumbers, vec2((float(x)+0.5)/1024, (float(y)+0.5)/1024));
  return vec2(rand.x, rand.y);*/
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
  return vec4(1,1,1,1);//vec4(0.5+0.5*dir.x, 0.5+0.5*dir.y, 0.5+0.5*dir.z, 0.0);
}

";

            _Camera.Compile(raytracerOptions, ref frag);

            _FractalSettings._FractalColours.Compile(ref frag);

            _FractalSettings.Compile(ref frag);

            frag += @"
float udBox(in vec3 p, in vec3 b, in vec3 c)
{
return length(max(abs(p-c)-b, 0.0));
}

float sdSphere( vec3 p, float s )
{
  return length(p)-s;
}

vec3 repeatxzfixed(vec3 p, vec3 c, vec3 dist)
{
 vec3 q = min(-dist,p)+dist + mod(min(max(p,-dist),dist),c)-0.5*c + max(p, dist)-dist;
 return vec3(q.x, p.y, q.z);
}

vec3 repeatxz(vec3 p, vec3 c)
{
 return vec3((mod(p,c)-0.5*c).x, p.y, (mod(p,c)-0.5*c).z);
}

float GetValue(int x, int seed, int axis, int octave)
{
return 1.0;
//	int val = x + axis*789221 + octave*15731 + seed*761;
//	val = (val<<13) ^ val;
//	return 1.0f - ( float(val * (val * val * 15731 + 789221) + 1376312589 & 0x7fffffff) / 1073741824.0f);
}

int imod(int arg, int mod)
{
return arg;
//    int ret = arg % mod;
//	if (ret<0) ret += mod;
//	return ret;
}
float GetPerlin2d(float posx, float posy, float rep, float scale, int seed, int octaves, float weightingMultiplier)
{return 1.0;
/*	float normalX = posx*rep;
	float normalY = posy*rep;
	float sum=0;
	float weighting = scale;

	for (int i=0; i<octaves; i++)
	{
		int perlinScale = 2<<(i+1);

		// calculate x axis position and y axis position
		int x0 = imod(int(floor(normalX*float(perlinScale))), perlinScale);
		float remainder = normalX*float(perlinScale) - float(floor(normalX*float(perlinScale)));
		int x1 = imod(x0+1, perlinScale);

		int y0 = imod(int(floor(normalY*float(perlinScale))), perlinScale);
		float remaindery = normalY*float(perlinScale) - float(floor(normalY*float(perlinScale)));
		int y1 = imod(y0+1, perlinScale);

		float fx0y0 = GetValue(x0 + perlinScale*y0, seed, 0, i);
		float fx1y0 = GetValue(x1 + perlinScale*y0, seed, 0, i);
		float fx0y1 = GetValue(x0 + perlinScale*y1, seed, 0, i);
		float fx1y1 = GetValue(x1 + perlinScale*y1, seed, 0, i);

		float ftx = remainder * 3.1415927f;
		float fx = (1 - cos(ftx)) * .5f;
		float fty = remaindery * 3.1415927f;
		float fy = (1 - cos(fty)) * .5f;

		sum += ((fx0y0*(1-fx) + fx1y0*fx) * (1-fy)
			+ (fx0y1*(1-fx) + fx1y1*fx) * fy)
			* weighting;
		weighting *= weightingMultiplier;
	}

	if (sum<-1) sum=-1;
	if (sum>1) sum=1;

	return sum;*/
}";

            frag += _FractalSettings._RenderOptions._Backgrounds[_FractalSettings._RenderOptions._Background]._Description;

            frag += @"
bool traceBackground(in vec3 pos, in vec3 dir, inout float dist, out vec3 out_pos, out vec3 normal, out material mat)
{
 vec3 p = pos;
 float r = 0;
 float distanceStep = 0.6;
 
 backgroundInitialise(distanceStep);
 for (int j=0; j<200; j++)
 {
  r = backgroundDE(p);
  if (r>100)
   return false;
  if (r<0.001)
  {
    float normalTweak=0.0001f;
	normal = vec3(backgroundDE(p+vec3(normalTweak,0,0)) - backgroundDE(p-vec3(normalTweak,0,0)),
		backgroundDE(p+vec3(0,normalTweak,0)) - backgroundDE(p-vec3(0,normalTweak,0)),
		backgroundDE(p+vec3(0,0,normalTweak)) - backgroundDE(p-vec3(0,0,normalTweak)));
    float magSq = dot(normal, normal);
    if (magSq<=0.0000000001*normalTweak)
        normal = -dir;
    else
        normal /= sqrt(magSq);

   out_pos = p + normal*0.001;
   dist = length(out_pos - pos);

   mat.diff = vec3(0.6,0.6,0.6);
   mat.refl = vec3(0.2,0.2,0.2);
   mat.spec = vec3(0.2,0.2,0.2);
   mat.specPower = 50;
   mat.gloss = 0.01;
   backgroundMaterial(out_pos, mat);
   return true;
  }
  p += 0.6 * r * dir;
 }
 return false;
}


bool trace(in vec3 pos, in vec3 dir, inout float dist, out vec3 out_pos, out vec3 normal, out material mat)
{
 vec3 bkgpos, bkgnormal;
 material bkgmat;
 float bkgdist=dist;
 bool hitFractal = traceFractal(pos, dir, dist, out_pos, normal, mat);
 bool hitBkg = traceBackground(pos, dir, bkgdist, bkgpos, bkgnormal, bkgmat);
 if ((hitFractal && hitBkg && dist>bkgdist) || hitBkg && !hitFractal)
 {
  dist = bkgdist;
  out_pos = bkgpos;
  normal = bkgnormal;
  mat = bkgmat;
 }
 return (hitFractal || hitBkg);
}";

            _GPULight.Compile(raytracerOptions, ref frag);

            frag += @"
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
  
  vec3 out_pos, normal;
  material mat;
  float dist = 10000;
  vec3 factor = vec3(1.0);
  vec4 oCol = vec4(0.0);
  vec3 iterpos = pos;
  vec3 iterdir = dir;
  
  for (int i=0; i<(depth ? 1 : "+(1+raytracerOptions._Reflections).ToString()+ @"); i++)
  {
   bool hit = trace(iterpos, iterdir, dist, out_pos, normal, mat);

   if (hit)
   {
    normal += mat.gloss * getRandomDirection3d();
    normal = normalize(normal);
    vec3 reflection = iterdir - normal*dot(normal,iterdir)*2.0f;
    vec3 lightDiff = vec3(0,0,0);
    vec3 lightSpec = vec3(0,0,0);
    calculateLighting(out_pos, normal, reflection, 10, lightDiff, lightSpec);

    vec3 col = mat.diff*lightDiff + mat.spec*lightSpec;
    oCol+=vec4(factor,0.0)*vec4(col, 0.0);

    iterpos = out_pos;
    iterdir = reflection;
    factor *= mat.refl;
   }
   else
   {
    oCol+=vec4(factor,0.0)*getBackgroundColour(iterdir);
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
}";
            return frag;
        }

        public XElement CreateElement()
        {
            XElement ret = new XElement("SCENE");
            _FractalSettings.CreateElement(ret);
            ret.Add(_Camera.CreateElement());
            return ret;
        }
    }
}
