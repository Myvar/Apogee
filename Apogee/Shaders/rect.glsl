

#ifdef VERTEX

#define MAX_JOINTS 50
#define MAX_WEIGHTS 3

in vec3 position;
in vec2 uv;
in vec3 JointWeight;

in vec3 Normal;
in vec3 Color;
in ivec3 JointID;



out vec2 uvOut;
out vec3 NormalOut;

uniform mat4 mvp; 

    
uniform mat4 jointTransforms[MAX_JOINTS];

void main(void)
{
    vec4 totalLocalPos = vec4(0.0);
	vec4 totalNormal = vec4(0.0);
	for(int i=0;i<MAX_WEIGHTS;i++){
		mat4 jointTransform = jointTransforms[JointID[i]];
		vec4 posePosition = jointTransform * vec4(position, 1.0);
		totalLocalPos += posePosition * JointWeight[i];
		
		vec4 worldNormal = jointTransform * vec4(Normal, 0.0);
		totalNormal += worldNormal * JointWeight[i];
	}
	
	NormalOut = (mvp * totalNormal.xyzw).xyz;	
	gl_Position = mvp * totalLocalPos;

   // gl_Position = mvp * vec4(position, 1.0);
    uvOut = uv;
   // NormalOut = Normal;
}

#endif
 
#ifdef FRAGMENT


out vec4 color;
in vec2 uvOut;
in vec3 NormalOut;
uniform sampler2D sampler;
uniform vec4 uColor;

void main(void)
{    
  float gamma = 2.2;

vec3 c = pow(texture(sampler, uvOut.st * vec2(1.0, -1.0)).rgb, vec3(gamma));
        color = vec4(c * (vec3(0.4,0.4,0.4) + clamp(dot(-vec3(-1,0.2,0), NormalOut), 0.0, 1.0)), 1.0);
      //color = texture(sampler, uvOut);
      //color = vec4(1, uvOut.x, uvOut.y, 1);
      
 
    color.rgb = pow(color.rgb, vec3(1.0/gamma));
}

#endif