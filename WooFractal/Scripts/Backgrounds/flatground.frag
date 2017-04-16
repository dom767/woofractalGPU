void backgroundInitialise(inout float distanceStep)
{
 distanceStep = 1.0;
}

float backgroundDE(in vec3 p)
{
 return udBox(p, vec3(20,2,20), vec3(0,-2,0));
}

void backgroundMaterial(in vec3 pos, inout material mat)
{
 mat.diff = vec3(0.6,0.6,0.6);
 mat.refl = vec3(0.2,0.2,0.2);
}