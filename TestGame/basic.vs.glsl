#version 150

uniform sampler2D diffuse;

attribute vec3 position;
attribute vec3 normal;
attribute vec2 textCoord;

out vec3 normal0;
out vec2 uv0;


uniform mat4 MVP;
uniform mat4 Model;
uniform vec3 viewPos;

void main()
{
    normal0 = normal;
    uv0 = textCoord;
    gl_Position = MVP * vec4(position, 1.0);
}