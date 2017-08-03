using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using WooFractal.Objects;

namespace WooFractal
{
    public class FractalSettings
    {
        public FractalSettings()
        {
            _FractalIterations = new List<WooFractalIteration>();
            _RenderOptions = new RenderOptions();
            _FractalColours = new List<FractalGradient>();
            FractalGradient fractalGradient = new FractalGradient();
            _FractalColours.Add(fractalGradient);
            _MaterialSelection = new MaterialSelection();
        }

        public void Set(RenderOptions renderOptions, List<FractalGradient> fractalColours, List<WooFractalIteration> fractalIterations, MaterialSelection materialSelection)
        {
            _RenderOptions = renderOptions;
            _FractalColours = fractalColours;
            _FractalIterations = fractalIterations;
            _MaterialSelection = materialSelection;
        }
        public void CreateElement(XElement parent)
        {
            XElement ret = new XElement("FRACTAL");
            _RenderOptions.CreateElement(ret);
            for (int i = 0; i < _FractalColours.Count; i++)
                _FractalColours[i].CreateElement(ret);
            for (int i = 0; i < _FractalIterations.Count; i++)
                _FractalIterations[i].CreateElement(ret);
            _MaterialSelection.CreateElement(ret);
            parent.Add(ret);
        }
        public void LoadXML(XmlReader reader)
        {
            _FractalColours.Clear();
            _FractalIterations = new List<WooFractalIteration>();
            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "MATERIALSELECTION")
                {
                    _MaterialSelection = new MaterialSelection();
                    _MaterialSelection.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "RENDEROPTIONS")
                {
                    _RenderOptions = new RenderOptions();
                    _RenderOptions.LoadXML(reader);
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "FRACTALCOLOURS")
                {
                    FractalGradient fractalColour = new FractalGradient();
                    fractalColour.LoadXML(reader);
                    _FractalColours.Add(fractalColour);
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
        public void CompileColours(ref string frag)
        {
            int divisor = _FractalColours.Count();

            int maxSegments = 0;
            for (int i = 0; i < _FractalColours.Count(); i++)
            {
                int segCount = _FractalColours[i]._GradientSegments.Count();
                if (segCount > maxSegments)
                    maxSegments = segCount;
            }

            frag += @"void OrbitToColour(in vec4 orbitTrap, inout material mat) {
vec4 trappos;
vec3 diffStart[" + maxSegments + @"];
vec3 diffEnd[" + maxSegments + @"];
vec3 specStart[" + maxSegments + @"];
vec3 specEnd[" + maxSegments + @"];
vec3 reflStart[" + maxSegments + @"];
vec3 reflEnd[" + maxSegments + @"];
float roughStart[" + maxSegments + @"];
float roughEnd[" + maxSegments + @"];
float dlcStart[" + maxSegments + @"];
float dlcEnd[" + maxSegments + @"];
float segStart[" + maxSegments + @"];
float segEnd[" + maxSegments + @"];

mat.diff = vec3(0,0,0);
mat.spec = vec3(0,0,0);
mat.refl = vec3(0,0,0);
mat.dlc = 0.0;
mat.roughness = 0.0;

int currS;
float gradX;
";

            for (int i = 0; i < _FractalColours.Count(); i++)
            {
                frag += "trappos = vec4(";
                frag += (_FractalColours[i]._BlendType == EBlendType.Chop) ? "round(" : "";
                frag += "pow(mod((orbitTrap." + _FractalColours[i].GetOrbitTypeString() + "*" + _FractalColours[i]._Multiplier.ToString() + ")+" + _FractalColours[i]._Offset.ToString() + ",1.0)," + Math.Pow(10, _FractalColours[i]._Power).ToString() + ")";
                frag += (_FractalColours[i]._BlendType == EBlendType.Chop) ? ")" : "";
                frag += ",0,0,0);\r\n";

                for (int s=0; s<_FractalColours[i]._GradientSegments.Count(); s++)
                {
                    frag += @"
diffStart["+s+"] = vec3("+_FractalColours[i]._GradientSegments[s]._StartColour._DiffuseColour.ToString()+@");
diffEnd["+s+"] = vec3("+_FractalColours[i]._GradientSegments[s]._EndColour._DiffuseColour.ToString()+@");
specStart["+s+"] = vec3("+_FractalColours[i]._GradientSegments[s]._StartColour._SpecularColour.ToString()+@");
specEnd["+s+"] = vec3("+_FractalColours[i]._GradientSegments[s]._EndColour._SpecularColour.ToString()+@");
reflStart["+s+"] = vec3("+_FractalColours[i]._GradientSegments[s]._StartColour._Reflectivity.ToString()+@");
reflEnd["+s+"] = vec3("+_FractalColours[i]._GradientSegments[s]._EndColour._Reflectivity.ToString()+@");
roughStart[" + s + "] = " + _FractalColours[i]._GradientSegments[s]._StartColour._Roughness.ToString() + @";
roughEnd[" + s + "] = " + _FractalColours[i]._GradientSegments[s]._EndColour._Roughness.ToString() + @";
dlcStart[" + s + "] = " + _FractalColours[i]._GradientSegments[s]._StartColour._DiElectric.ToString() + @";
dlcEnd[" + s + "] = " + _FractalColours[i]._GradientSegments[s]._EndColour._DiElectric.ToString() + @";
segStart[" + s + "] = " + _FractalColours[i]._GradientSegments[s]._StartX.ToString() + @";
segEnd[" + s + "] = " + _FractalColours[i]._GradientSegments[s]._EndX.ToString() + @";
";
                }

                frag += @"
currS = 0;
for (int i=0; i<" + _FractalColours[i]._GradientSegments.Count()+ @"; i++)
{
 currS = int(max(currS, i * min(1, floor(1+(trappos.x-segStart[i])) * min(1, floor(1+(segEnd[i]-trappos.x))))));
}

gradX = (trappos.x - segStart[currS]) / (segEnd[currS] - segStart[currS]);
";
                frag += "mat.diff+=mix(diffStart[currS], diffEnd[currS], gradX);\r\n";
                frag += "mat.spec+=mix(specStart[currS], specEnd[currS], gradX);\r\n";
                frag += "mat.refl+=mix(reflStart[currS], reflEnd[currS], gradX);\r\n";
                frag += "mat.dlc+=mix(dlcStart[currS], dlcEnd[currS], gradX);\r\n";
                frag += "mat.roughness+=mix(roughStart[currS], roughEnd[currS], gradX);\r\n";
            }

            frag += "mat.diff/=" + divisor.ToString() + @";";
            frag += "mat.spec/=" + divisor.ToString() + @";";
            frag += "mat.refl/=" + divisor.ToString() + @";";
            frag += "mat.dlc/=" + divisor.ToString() + @";";
            frag += "mat.roughness/=" + divisor.ToString() + @";
}";
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

void Bulb(in float r, inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in mat3 mRotate1Matrix, in bool juliaEnabled, in vec3 julia)
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
	pos = zr * vec3(cos(theta)*cos(phi), sin(theta)*cos(phi), sin(phi));

    // julia Land?
    pos += juliaEnabled ? julia : origPos;

	pos = mRotate1Matrix * pos;
}

void Kleinian(inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in vec3 mCSize, in vec3 mJulia)
{
	pos = 2.0*clamp(pos, -mCSize, mCSize) - pos;

	float r2 = dot(pos, pos);
	float k = max(mScale/r2, 1);
	pos *= k;
	scale *= k;

	pos += mJulia;
}

void Box(inout vec3 pos, in vec3 origPos, inout float scale, in vec3 mScale, in mat3 mRotate1Matrix, in float mMinRadius )
{
	pos *= mRotate1Matrix;
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
			
    pos = (pos * mScale) + origPos;
	scale = scale * max(mScale.x, max(mScale.y,mScale.z)) + 1.0f;
}

void Cuboid(inout vec3 pos, in vec3 origPos, inout float scale, in float mScale, in vec3 mPOffset, in mat3 mRotate1Matrix, in mat3 mRotate2Matrix )
{
	pos *= mRotate1Matrix;

	pos = abs(pos);

	pos *= mRotate2Matrix;

	pos = pos*mScale - mPOffset;
	scale *= mScale;
}";
            for (int i = 0; i < _FractalIterations.Count; i++)
            {
                _FractalIterations[i].CompileDeclerations(ref frag2, i);
            }
            frag2 += @"

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
    if (r>40.0) continue;
    if (j<"+_RenderOptions._ColourIterationCount+@") orbitTrap = min(orbitTrap, vec4(abs(pos),r));";

            int totalIterations = 0,iterationIndex=0;
            for (int i=0; i<_FractalIterations.Count; i++)
            {
                totalIterations += _FractalIterations[i]._Repeats;
            }

            frag2+=@"
 int modj = j%"+totalIterations+@";";

            for (int i = 0; i < _FractalIterations.Count; i++)
            {
                int repeats = _FractalIterations[i]._Repeats;
                frag2 += @"
 if (modj>=" + iterationIndex + " && modj<" + (iterationIndex + repeats).ToString() + @")
 {";
                _FractalIterations[i].Compile(ref frag2, i);
                frag2 += @"
}";
                iterationIndex += repeats;
            }

            frag2 += @"
  }
 float ret=0;
 // DEMode 0=KIFS, 1=BOX, 2=BULB, 3=kleinian
 if (DEMode==1) ret = (r - 1) / abs(scale);
 if (DEMode==2) ret = 0.5*log(r)*r/scale;
 if (DEMode==0) ret = (r - 1) / abs(scale);
 if (DEMode==3) ret = 0.5*abs(pos.z)/scale;
 float bbdist = length(origPos - clamp(origPos, vec3(-" + _RenderOptions._DistanceExtents + "), vec3(" + _RenderOptions._DistanceExtents + @")));
 ret = max(ret, bbdist);
 return ret;
}

// https://github.com/hpicgs/cgsee/wiki/Ray-Box-Intersection-on-the-GPU
void intersectAABB(in vec3 pos, in vec3 dir, out float tmin, out float tmax)
{
 float tymin, tymax, tzmin, tzmax;
 vec3 invdir = vec3(1)/(dir);
 vec3 sign = vec3(dir.x>=0?" + _RenderOptions._DistanceExtents + @":-" + _RenderOptions._DistanceExtents + @", dir.y>=0?" + _RenderOptions._DistanceExtents + @":-" + _RenderOptions._DistanceExtents + @", dir.z>=0?" + _RenderOptions._DistanceExtents + @":-" + _RenderOptions._DistanceExtents + @");
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
mat.roughness = 0.01;
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

  vec4 orbitTrap = vec4(10000,10000,10000,10000);
  float DEdist;
  vec3 oldDp;
  float minDistance2 = minDistance;

  // iterate...
  for (int i=0; i<" + _RenderOptions._DistanceIterations + @"; i++)
  {
   DEdist = DE(dp, orbitTrap);
   oldDp = dp;
   dp += " + _RenderOptions._StepSize + @"*DEdist*dir;
   tmax -= " + _RenderOptions._StepSize + @"*DEdist; // not sure this is the most efficient tbh
   if (tmax<0) return false; // exiting the AABB, skip
";

   if (_RenderOptions._DistanceMinimumMode!=0)
       frag2 += "minDistance2 = length(dp-srcPos) / screenWidth;";
   
            frag2 += @"
   if (DEdist<minDistance2)
   {
    vec3 mid;
    vec4 torbitTrap;
    vec3 midInside;
    for (int j=0; j<4; j++)
    {
     mid = (dp+oldDp)*0.5;
     torbitTrap;
";

            if (_RenderOptions._DistanceMinimumMode != 0)
                frag2 += "minDistance2 = length(mid-srcPos) / screenWidth;";

            frag2 += @"
     midInside.x = max(0,sign(DE(mid, torbitTrap)-minDistance2));
     midInside.y = 1-midInside.x;
     dp = midInside.xxx*dp + midInside.yyy*mid;
     orbitTrap = midInside.xxxx*torbitTrap + midInside.yyyy*orbitTrap;
     oldDp = midInside.xxx*mid + midInside.yyy*oldDp;
    }

    OrbitToColour(orbitTrap, mat);
	vec3 normalTweak=vec3(minDistance2*0.1f,0,0);
	normal = vec3(DE(dp+normalTweak.xyy,orbitTrap) - DE(dp-normalTweak.xyy,orbitTrap),
		DE(dp+normalTweak.yxy,orbitTrap) - DE(dp-normalTweak.yxy,orbitTrap),
		DE(dp+normalTweak.yyx,orbitTrap) - DE(dp-normalTweak.yyx,orbitTrap));
    float magSq = dot(normal, normal);
    if (magSq<=0.001*minDistance2*minDistance2)
        normal = -dir;
    else
        normal /= sqrt(magSq);

    out_pos = dp + normal*(4*minDistance2) + vec3(0," + _RenderOptions._DistanceExtents + @",0);
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
        public List<FractalGradient> _FractalColours;
        public List<WooFractalIteration> _FractalIterations;
        public MaterialSelection _MaterialSelection;
    }
}
