void backgroundInitialise(inout float distanceStep)
{
 distanceStep = 0.6;
}

vec3 repeat(vec3 p, vec3 c)
{
 return (mod(p,c)-0.5*c);
}

float backgroundDE(in vec3 p)
{
 return min(sdSphere(repeatxzfixed(p-vec3(0,-0.1,0),vec3(0.3),vec3(200)), 0.1), udBox(p, vec3(20,2,20), vec3(0,-2.2,0)));
}

void backgroundMaterial(in vec3 pos, inout material mat)
{
 mat.diff = vec3(0.6);
 mat.refl = vec3(0.2);
}