#version 130

vec2 data[6] = vec2[]
(
  vec2(0.0,  1.0),
  vec2(0.0, 0.0),
  vec2( 1.0,  1.0),
  vec2( 1.0,  1.0),
  vec2(0.0, 0.0),
  vec2( 1.0, 0.0)
);
out vec2 texCoord;
uniform float progressiveIndex;

// https://rauwendaal.net/2014/06/14/rendering-a-screen-covering-triangle-in-opengl/
void main()
{
    float index = floor(gl_VertexID/6.0) + progressiveIndex;
    float x = round(mod(index, 16));
	float y = floor(0.01+index/16.0);
	texCoord = vec2(data[gl_VertexID%6].x/16 + x/16, data[gl_VertexID%6].y/16 + y/16);
	texCoord = 2*texCoord - 1;
    gl_Position = vec4(texCoord.x, texCoord.y, 0, 1);
}