#version 450

in vec2 UV;
uniform sampler2D texture;

void main()
{    
    gl_FragColor = texture2D( texture, UV).rgba;
    
    float gamma = 2.2;
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vec3(1.0/gamma));
} 
