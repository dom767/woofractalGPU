using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WooFractal
{
    class GPULight
    {
        public void Compile(RaytracerOptions raytracerOptions, ref string frag)
        {
            frag += @"

void calculateLighting(in vec3 pos, in vec3 normal, in vec3 eye, in vec3 reflection, in float roughness, out vec3 lightDiff, out vec3 lightSpec)
{
   vec3 direction = normalize(vec3(1, 1, 1));
   vec3 opos, onor;
   material omat;
   float dist = 1000;
   vec3 shadow = vec3(1,1,1);
   ";
            if (raytracerOptions._ShadowsEnabled)
            {
                frag += @"if (trace(pos, direction, dist, opos, onor, omat))
    shadow = vec3(0,0,0);
";
            }

            frag += @"lightDiff = shadow * vec3(max(dot(direction, normal),0));

 vec3 halfVec = normalize(direction - eye);
 float NdotH = max(dot(normal, halfVec),0);
 float RdotL = max(dot(reflection, direction), 0);
 lightSpec = vec3(pow(NdotH, 1.00001/(0.00001+roughness)));
//lightDiff = vec3(dot(direction, normal));
//
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
            frag += @"lightDiff += getBackgroundColour(wdirection).xyz*vec3(dot(wdirection, normal));
}
";
        }
    }
}
