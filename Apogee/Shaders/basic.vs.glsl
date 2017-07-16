#version 130

uniform sampler2D diffuse;

attribute vec3 position;
attribute vec3 normal;

out vec3 normal0;


uniform mat4 MVP;
uniform mat4 Model;
uniform vec3 viewPos;

void main()
{
    normal0 = normal;
    
    gl_Position = MVP * vec4(position, 1.0);
}