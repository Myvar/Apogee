#version 130

uniform sampler2D diffuse;

attribute vec3 position;
attribute vec3 upaxisMatrix;
attribute vec2 textCoord;
attribute vec3 color;
attribute vec3 normal;
attribute vec3 tangent;

out vec2 textCoord0;
out vec3 normal0;
out mat4 Model0;
out vec3 viewPos0;
out vec3 pos0;
out vec3 glpos;
flat out vec3 color0;

uniform mat4 MVP;
uniform mat4 Model;
uniform vec3 viewPos;

void main()
{
    textCoord0 = textCoord;
    normal0 = normal;
    Model0 = Model;
    color0 = color;
    viewPos0 = viewPos;
    pos0 = position;
    
    gl_Position = MVP * vec4(position, 1.0);
    glpos = gl_Position.xyz;
}