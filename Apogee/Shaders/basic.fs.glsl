#version 130

in vec2 textCoord0;
in vec3 normal0;
in vec3 viewPos0;
in vec3 glpos;
in mat4 Model0;
in vec3 pos0;
flat in vec3 color0;

uniform vec3 Color;
uniform sampler2D sampler;


uniform vec3 Phong_Emission;
uniform vec3 Phong_Ambient;
uniform vec3 Phong_Diffuse;
uniform vec3 Phong_Specular;
uniform vec3 Phong_Shininess;


// material parameters
float metallic = 0.5;
float roughness = 0.5;
float ao = 1;

// lights
vec3 lightPositions = vec3(2,2,1);
vec3 lightColors = vec3(1,1,1);

void main()
{		
   
    gl_FragColor = vec4(0, 1, 1, 1.0);
}  