#version 150

uniform sampler2D diffuse;

attribute vec3 position;
attribute vec3 normal;
out vec3 pos0;

flat out vec3 normal0;

attribute vec2 textCoord;
out vec2 UV;


uniform mat4 MVP;
uniform mat4 Model;
uniform vec3 viewPos;

void main()
{
    UV = textCoord;
    normal0 = normal;
    pos0 = position;
    gl_Position = MVP * vec4(position, 1.0);
}