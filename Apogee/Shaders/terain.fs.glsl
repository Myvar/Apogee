#version 150

flat in vec3 normal0;
in vec3 pos0;
in vec2 UV;
uniform sampler2D texture;
uniform vec3 viewPos;


void main()
{		
    vec3 lightColor = vec3(1,1,1);
    vec3 lightPos = viewPos;

    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;
    
    vec3 norm = normalize(normal0);
    vec3 lightDir = normalize(lightPos - pos0);  
    
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;
    
    vec3 result = (ambient + diffuse) * vec3(0.8,0.8,0.8);

    gl_FragColor = vec4(result, 1.0);
    
    //game correction
    float gamma = 2.2;
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vec3(1.0/gamma));
}  