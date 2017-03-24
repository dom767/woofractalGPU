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

void calculateLighting(in vec3 pos, in vec3 normal, in vec3 reflection, in float specularPower, out vec3 lightDiff, out vec3 lightSpec)
{
   vec3 direction = normalize(vec3(1, 1, 1));
   vec3 opos, onor;
   material omat;
   float dist = 1000;
   ";
            if (raytracerOptions._ShadowsEnabled)
            {
                frag += @"if (trace(pos, direction, dist, opos, onor, omat))
    lightDiff = vec3(0,0,0);
   else
   {
    ";
            }
            
            frag += @"lightDiff = vec3(dot(direction, normal));";
    
            if (raytracerOptions._ShadowsEnabled)
            {
                frag += @"}";
            }

            frag += @"
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
