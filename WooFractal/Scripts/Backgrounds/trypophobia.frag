void backgroundInitialise(inout float distanceStep)
{
 distanceStep = 0.6;
}

float backgroundDE(in vec3 p)
{
 float radiusTweak = GetPerlin2d(p.x, p.z, 0.3, 0.4, 3453, 2, 0.5)*0.2;
 p.y += radiusTweak;
 return min(sdSphere(repeatxzfixed(p,vec3(1.3),vec3(20))-vec3(0,-0.6,0), 0.45+radiusTweak), max(udBox(p, vec3(20,2,20), vec3(0,-2,0)), -sdSphere(repeatxz(p,vec3(1.3))-vec3(0,-0.4,0), 0.65+radiusTweak)));
}

void backgroundMaterial(in vec3 pos, inout material mat)
{
 mat.diff = mix(vec3(0.6,0.1,0.1),vec3(0.9,0.9,0.7), min(1, max(0, (pos.y+0.4)*2)));
 mat.refl = mix(vec3(0.6,0.6,0.6),vec3(0.0,0.0,0.0), min(1, max(0, (pos.y+0.4)*2)));
}