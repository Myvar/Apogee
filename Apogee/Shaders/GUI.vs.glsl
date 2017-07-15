#version 130


attribute vec3 position;
attribute vec2 textCoord;
attribute vec3 color;
attribute vec3 normal;
attribute vec3 tangent;

uniform mat4 MVP;
uniform mat4 Model;

void main()
{
    
    gl_Position = MVP * vec4(position, 1.0);
}
