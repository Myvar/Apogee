#version 450

in vec3 normal0;

void main()
{		
   
    gl_FragColor = vec4(vec3(0.8, 0.8, 0.8) * clamp(dot(-vec3(0,0,1), normal0), 0.0, 1.0), 1.0);
    
    //game correction
    float gamma = 2.2;
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vec3(1.0/gamma));
}  