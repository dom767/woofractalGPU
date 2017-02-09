#version 130

in vec2 texCoord;

out vec4 FragColor;
/*
float DE(vec3 pos)
{
}

vec4 fractal(vec3 ray)
{
for(int i=0;i<99;i++){
      d=DE(ro+rd*t);
      float px=pxl*(1.+t);
      if(d<px){
         vec3 scol=mcol;
         float d2=DE(ro+rd*t+LDir*px);
         float shad=abs(d2/d),shad2=max(0.0,1.0-d/od);
         scol=scol*shad+vec3(0.2,0.0,-0.2)*(shad-0.5)+vec3(0.1,0.15,0.2)*shad2;
         scol*=3.0*max(0.2,shad2);
         scol/=(1.0+t);//*(0.2+10.0*dL*dL);
         
         float alpha=(1.0-col.w)*clamp(1.0-d/(px),0.0,1.0);
         col+=vec4(clamp(scol,0.0,1.0),1.0)*alpha;
         if(col.w>0.9)break;
      }
      od=d;
      t+=d;
      if(t>6.0)break;
   }
}

vec4 trace(vec3 ray)
{
return vec4(1,0.5,0,1);
}


*/

void main(void)
{
  vec2 q = texCoord.xy;

 // vec3 pos, dir;

//  getcamera(pos, dir, q);

  FragColor=vec4(q.x, q.y, 1.0, 1.0);
  //FragColor=trace(vec3(1,1,1));
}

void getcamera(out vec3 pos, out vec3 dir, in vec2 q)
{
}