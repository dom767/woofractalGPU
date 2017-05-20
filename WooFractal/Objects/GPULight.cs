using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooFractal
{
    public abstract class Light
    {
        public abstract void Compile(RaytracerOptions raytracerOptions, ref string frag);
    }

    public class PointLight : Light
    {
        public override void Compile(RaytracerOptions raytracerOptions, ref string frag)
        {
            frag += @"";
        }
    }


    class GPULight
    {
        public void Compile(RaytracerOptions raytracerOptions, RenderOptions renderOptions, ref string frag)
        {
            Vector3 sunDirection = renderOptions.GetSunVector3();

            frag += @"
void calculateDirectionalLight(in vec3 pos, in vec3 normal, in vec3 eye, in vec3 direction, in vec3 reflection, in float roughness, in bool shadows, out vec3 lightDiff, out vec3 lightSpec)
{
 vec3 opos, onor;
 material omat;
 float dist = 1000;
// vec3 direction = normalize(vec3(1, 1, 1));
 direction += getRandomDirection3d()*0.01;
 direction = normalize(direction);
 
 vec3 shadow = vec3(1,1,1);
 if (shadows)
 {
  if (trace(pos, direction, dist, opos, onor, omat))
   shadow = vec3(0,0,0);
 }
 lightDiff += shadow * vec3(max(dot(direction, normal),0));

 vec3 halfVec = normalize(direction - eye);
 float NdotH = max(dot(normal, halfVec),0);
 float RdotL = max(dot(reflection, direction), 0);
 lightSpec += vec3(pow(NdotH, 1.00001/(0.00001+roughness)));
}

void calculateWorldLight(in vec3 pos, in vec3 normal, in vec3 eye, in vec3 reflection, in float roughness, in bool shadows, out vec3 lightDiff, out vec3 lightSpec)
{
 vec3 opos, onor;
 material omat;
 float dist = 1000;
 vec3 wdirection = getSampleBiased(normal, 1);

 if (!shadows || !trace(pos, wdirection, dist, opos, onor, omat))
    lightDiff += getBackgroundColour(wdirection, pos).xyz*vec3(dot(wdirection, normal));
}


void calculateLighting(in vec3 pos, in vec3 normal, in vec3 eye, in vec3 reflection, in float roughness, out vec3 lightDiff, out vec3 lightSpec)
{
 lightDiff = vec3(0,0,0);
 lightSpec = vec3(0,0,0);
 
 calculateDirectionalLight(pos, normal, eye, vec3(" + (sunDirection.x) + "," + (sunDirection.y) + "," + (sunDirection.z) + @"), reflection, roughness, " + (raytracerOptions._ShadowsEnabled ? "true" : "false") + @", lightDiff, lightSpec);

 //calculateWorldLight(pos, normal, eye, reflection, roughness, " + (raytracerOptions._ShadowsEnabled ? "true" : "false") + @", lightDiff, lightSpec);
 
 vec3 opos, onor;
 material omat;
 float dist = 1000;


 vec3 wdirection = getSampleBiased(normal, 1);
   dist = 1000;
   ";
            if (raytracerOptions._ShadowsEnabled)
            {
                frag += @"if (trace(pos, wdirection, dist, opos, onor, omat))
    lightDiff = vec3(0,0,0);
   else
    ";
            }
            frag += @"lightDiff += getBackgroundColour(wdirection, pos).xyz*vec3(dot(wdirection, normal));
}
";
        }
    }
}
