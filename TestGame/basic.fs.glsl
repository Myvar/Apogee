#version 150

in vec3 normal0;
in vec2 uv0;
uniform sampler2D sampler;

void main()
{
    vec3 color = texture(sampler, uv0).rgb;

    if(color.r == 0 && color.g == 0 && color.b == 0)
    {
        color = vec3(0.8, 0.8, 0.8);
    }

    vec3 light = vec3(0.1,0.1,0.1);
    light += clamp(dot(-vec3(1,0.2,0), normal0), 0.0, 1.0);

    gl_FragColor = vec4(color * light, 1.0);

    //game correction
    float gamma = 2.2;
    gl_FragColor.rgb = pow(gl_FragColor.rgb, vec3(1.0/gamma));
}
