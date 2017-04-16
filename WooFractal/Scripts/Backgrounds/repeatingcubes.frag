void backgroundInitialise(inout float distanceStep)
{
 distanceStep = 0.6;
}

vec3 repeat(vec3 p, vec3 c)
{
 return (mod(p,c)-0.5*c);
}

float udBox(vec3 p, vec3 b)
{
 return length(max(abs(p)-b,0.0));
}

float backgroundDE(in vec3 p)
{
 return min(udBox(repeatxzfixed(p-vec3(1.5,-0.2,1.5),vec3(3),vec3(20)), vec3(1,0.2,1), vec3(0,0.0,0)), udBox(p-vec3(1.5,0,1.5), vec3(20,2,20), vec3(0,-2.4,0)));
}

void backgroundMaterial(in vec3 pos, inout material mat)
{
 mat.diff = vec3(0.6);
 mat.refl = vec3(0.2);
}