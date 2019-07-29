#ifdef VERTEX


layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 Projection;
};
layout(set = 0, binding = 1) uniform ViewBuffer
{
    mat4 View;
};
layout(set = 1, binding = 0) uniform WorldBuffer
{
    mat4 World;
};
layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in vec4 Color;
layout(location = 3) in vec3 Normal;
layout(location = 0) out vec2 fsin_texCoords;
layout(location = 1) out vec4 fsin_color;
layout(location = 2) out vec3 fsin_normal;
void main()
{
    vec4 worldPosition = World * vec4(Position, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
    fsin_texCoords = TexCoords;
    fsin_texCoords = TexCoords;
    fsin_color = Color;
    fsin_normal = Normal;
}

#endif

#ifdef FRAGMENT

layout(location = 0) in vec2 fsin_texCoords;
layout(location = 1) in vec4 fsin_color;
layout(location = 2) in vec3 fsin_normal;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
void main()
{
    vec3 color =  texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords).xyz ;
      fsout_color = vec4(fsin_color.xyz  * (clamp(dot(-vec3(1,0,0), fsin_normal) , 0.0, 1.0) + vec3(0.2,0.2,0.2)), 1.0) ;
   // fsout_color =  fsin_color;
       //game correction
    float gamma = 2.2;
    fsout_color.rgb = pow(fsout_color.rgb, vec3(1.0/gamma));
}

#endif