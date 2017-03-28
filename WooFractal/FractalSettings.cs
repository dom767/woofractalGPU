using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace WooFractal
{
    public class FractalSettings
    {
        public FractalSettings()
        {
            _FractalIterations = new List<WooFractalIteration>();
            _RenderOptions = new RenderOptions();
            _FractalColours = new FractalColours();
        }

        public void Set(RenderOptions renderOptions, FractalColours fractalColours, List<WooFractalIteration> fractalIterations)
        {
            _RenderOptions = renderOptions;
            _FractalColours = fractalColours;
            _FractalIterations = fractalIterations;
        }
        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("FRACTAL");
            _RenderOptions.CreateElement(ret);
            _FractalColours.CreateElement(ret);
            for (int i = 0; i < _FractalIterations.Count; i++)
                _FractalIterations[i].CreateElement(ret);
            parent.Add(ret);
        }
        public void LoadXML(XmlReader reader)
        {
            _FractalIterations = new List<WooFractalIteration>();
            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "RENDEROPTIONS")
                {
                    _RenderOptions = new RenderOptions();
                    _RenderOptions.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "FRACTALCOLOURS")
                {
                    _FractalColours = new FractalColours();
                    _FractalColours.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "KIFSFRACTAL")
                {
                    KIFSIteration fractalIteration = new KIFSIteration();
                    fractalIteration.LoadXML(reader);
                    _FractalIterations.Add(fractalIteration);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "BULBFRACTAL")
                {
                    MandelbulbIteration fractalIteration = new MandelbulbIteration();
                    fractalIteration.LoadXML(reader);
                    _FractalIterations.Add(fractalIteration);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "BOXFRACTAL")
                {
                    MandelboxIteration fractalIteration = new MandelboxIteration();
                    fractalIteration.LoadXML(reader);
                    _FractalIterations.Add(fractalIteration);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "KLEINIANGROUP")
                {
                    KleinianGroupIteration fractalIteration = new KleinianGroupIteration();
                    fractalIteration.LoadXML(reader);
                    _FractalIterations.Add(fractalIteration);
                }
            }
            reader.Read();
        }
        public void Compile(ref string frag)
        {
            string frag2 = @"

void Menger(inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in vec3 mOffset, in mat3 mRotate1Matrix, in mat3 mRotate2Matrix)
{
	pos *= mRotate1Matrix;
    vec3 mPOffset = mOffset*(mScale-1);
	float tmp;

	pos = abs(pos);
	if(pos[0]-pos[1]<0){tmp=pos[1];pos[1]=pos[0];pos[0]=tmp;}
	if(pos[0]-pos[2]<0){tmp=pos[2];pos[2]=pos[0];pos[0]=tmp;}
	if(pos[1]-pos[2]<0){tmp=pos[2];pos[2]=pos[1];pos[1]=tmp;}
      
	pos[2]-=0.5f*mOffset[2]*(mScale-1)/mScale;
	pos[2]=-abs(-pos[2]);
	pos[2]+=0.5f*mOffset[2]*(mScale-1)/mScale;
      
	pos *= mRotate2Matrix;

	pos[0]=mScale*pos[0]-mPOffset[0];
	pos[1]=mScale*pos[1]-mPOffset[1];
	pos[2]=mScale*pos[2];

	scale *= mScale;
}

void Tetra(inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in vec3 mPOffset, in mat3 mRotate1Matrix, in mat3 mRotate2Matrix)
{
	float tmp;

	pos *= mRotate1Matrix;

	if(pos[0]+pos[1]<0){tmp=-pos[1];pos[1]=-pos[0];pos[0]=tmp;}
	if(pos[0]+pos[2]<0){tmp=-pos[2];pos[2]=-pos[0];pos[0]=tmp;}
	if(pos[1]+pos[2]<0){tmp=-pos[2];pos[2]=-pos[1];pos[1]=tmp;}
      
	pos *= mRotate2Matrix;

	pos = pos*mScale - mPOffset;
	scale *= mScale;
}

void Bulb(in float r, inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in mat3 mRotate1Matrix)
{
    float theta, phi;
    
	theta = atan(pos.y / pos.x);
	phi = asin(pos.z / r);
	scale =  pow( r, mScale-1.0)*mScale*scale + 1.0;

	// scale and rotate the point
	float zr = pow( r,mScale);
	theta = theta*mScale;
	phi = phi*mScale;

	// convert back to cartesian coordinates
	pos = zr * vec3(cos(theta)*cos(phi), sin(theta)*cos(phi), sin(phi)) + origPos;

	pos = mRotate1Matrix * pos;
}

void Kleinian(inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in mat3 mRotate1Matrix, in float mMultiplier)//, in float mMinRadius )
{
	float m = 0.5;//mMultiplier;

	pos = vec3(-1.0,-1,-1) + 2.0*fract(pos*m + vec3((1-m),(1-m),(1-m)));

	float r2 = dot(pos, pos);
	float k = mScale/r2;
	pos *= k;
	scale *= k;

//	pos = mRotate1Matrix * pos;
}

void Box(inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in mat3 mRotate1Matrix, in float mMinRadius )
{
//	pos *= mRotate1Matrix;
	float fixedRadius = 1.0;
	float fR2 = fixedRadius * fixedRadius;
	float mR2 = mMinRadius * mMinRadius;

    pos = clamp(pos, -1, 1) *2.0 - pos;		

	float r2 = dot(pos,pos);

	if (r2 < mR2)
	{
		pos *= fR2 / mR2;
		scale*= fR2 / mR2;
	}
	else if (r2 < fR2)
	{
		pos *= fR2 / r2;
		scale*= fR2 / r2;
	}
			
    pos = pos * mScale + origPos;
	scale = scale * mScale + 1.0f;
}

void Cuboid(inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in vec3 mPOffset, in mat3 mRotate1Matrix, in mat3 mRotate2Matrix )
{
	pos *= mRotate1Matrix;

	pos = abs(pos);

	pos *= mRotate2Matrix;

	pos = pos*mScale - mPOffset;
	scale *= mScale;
}

float DE(in vec3 origPos, out vec4 orbitTrap)
{
  origPos.xyz = origPos.xzy;
  vec3 pos = origPos;
  float r, theta, phi;
  float scale = 1;
  float mScale = 8;
  float fracIterations = 0;
  int DEMode = 0;
  orbitTrap = vec4(10000,10000,10000,10000);
  
  for (int j=0; j<" + _RenderOptions._FractalIterationCount+@"; j++)
  {
    r = length(pos);
    if (r>100.0) continue;
    if (j<"+_RenderOptions._ColourIterationCount+@") orbitTrap = min(orbitTrap, vec4(abs(pos),r));
    if (fracIterations>" + _RenderOptions._FractalIterationCount + @") continue;";

            for (int i = 0; i < _FractalIterations.Count; i++)
            {
                frag2 += @"fracIterations+="+_FractalIterations[i]._Repeats+@";
";
                for (int r=0; r<_FractalIterations[i]._Repeats; r++)
                    _FractalIterations[i].Compile(ref frag2);
            }
//            Bulb(r, pos, origPos, scale, 8, mat3(1,0,0,0,1,0,0,0,1));
            //    Tetra(pos, origPos, scale, 2, 1, mat3(1,0,0,0,1,0,0,0,1), mat3(1,0,0,0,1,0,0,0,1));

            frag2 += @"
  }
 // DEMode 0=KIFS, 1=BOX, 2=BULB, 3=kleinian
 if (DEMode==1) return (r - 1) / abs(scale);
 if (DEMode==2) return 0.5*log(r)*r/scale;
 if (DEMode==0) return (r - 1) / abs(scale);
 if (DEMode==3) return 0.25*abs(pos.z)/scale;
 // return length(origPos-vec3(0,0,0))-1;
}

// https://github.com/hpicgs/cgsee/wiki/Ray-Box-Intersection-on-the-GPU
void intersectAABB(in vec3 pos, in vec3 dir, out float tmin, out float tmax)
{
 float tymin, tymax, tzmin, tzmax;
 vec3 invdir = vec3(1)/dir;
 vec3 sign = vec3(dir.x>0?" + _RenderOptions._DistanceExtents + @":-" + _RenderOptions._DistanceExtents + @", dir.y>0?" + _RenderOptions._DistanceExtents + @":-" + _RenderOptions._DistanceExtents + @", dir.z>0?" + _RenderOptions._DistanceExtents + @":-" + _RenderOptions._DistanceExtents + @");
 tmin = (-sign.x - pos.x) * invdir.x;
 tmax = (sign.x - pos.x) * invdir.x;
 tymin = (-sign.y - pos.y) * invdir.y;
 tymax = (sign.y - pos.y) * invdir.y;
 tzmin = (-sign.z - pos.z) * invdir.z;
 tzmax = (sign.z - pos.z) * invdir.z;
 tmin = max(max(tmin, tymin), tzmin);
 tmax = min(min(tmax, tymax), tzmax);   
}

bool traceFractal(in vec3 pos, in vec3 dir, inout float dist, out vec3 out_pos, out vec3 normal, out material mat)
{
mat.diff = vec3(1,1,1);
mat.spec = vec3(0.2,0.2,0.2);
mat.refl = vec3(0.2,0.2,0.2);
mat.specPower = 50;
mat.gloss = 0.01;
  pos.y -= " + _RenderOptions._DistanceExtents + @";
  vec3 srcPos = camPos;
  srcPos.y -= " + _RenderOptions._DistanceExtents + @";
  float minDistance = " + Math.Pow(10, -_RenderOptions._DistanceMinimum).ToString("0.#######") + @";
  
  // clip to AABB
  float tmin, tmax;
  intersectAABB(pos, dir, tmin, tmax);

  // bail if no collision
  if (tmin>tmax) return false;
  
  // skip ray to start of AABB
  tmin = max(0, tmin);
  vec3 dp = pos + tmin*dir;

  // iterate...
  for (int i=0; i<" + _RenderOptions._DistanceIterations + @"; i++)
  {
   vec4 orbitTrap = vec4(10000,10000,10000,10000);
   float DEdist = DE(dp, orbitTrap);
   dp += " + _RenderOptions._StepSize + @"*DEdist*dir;
   tmax -= " + _RenderOptions._StepSize + @"*DEdist; // not sure this is the most efficient tbh
   if (tmax<0) return false; // exiting the AABB, skip
";

   if (_RenderOptions._DistanceMinimumMode==0)
       frag2 += "float minDistance2 = minDistance;";
   else
       frag2 += "float minDistance2 = length(dp-srcPos) / screenWidth;";
   
            frag2 += @"
   if (DEdist<minDistance2)
   {
    OrbitToColour(orbitTrap, mat);
	float normalTweak=minDistance2*0.1f;
	normal = vec3(DE(dp+vec3(normalTweak,0,0),orbitTrap) - DE(dp-vec3(normalTweak,0,0),orbitTrap),
		DE(dp+vec3(0,normalTweak,0),orbitTrap) - DE(dp-vec3(0,normalTweak,0),orbitTrap),
		DE(dp+vec3(0,0,normalTweak),orbitTrap) - DE(dp-vec3(0,0,normalTweak),orbitTrap));
    float magSq = dot(normal, normal);
    if (magSq<=0.0000000001*normalTweak)
        normal = -dir;
    else
        normal /= sqrt(magSq);

    out_pos = dp + normal*minDistance2 + vec3(0," + _RenderOptions._DistanceExtents+@",0);
    dist = length(dp - pos);
    return true;
   }
  }
  return false;
}
";
            frag += frag2;
        }
        public RenderOptions _RenderOptions;
        public FractalColours _FractalColours;
        public List<WooFractalIteration> _FractalIterations;
    }
}
