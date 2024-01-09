#version 460 core
out vec4 FragColor;
in vec2 texCoord;
in vec3 faceNorm;
in vec3 fragPos;

uniform sampler2D tex;
uniform vec3 viewPos;

void main()
{
    float ambientStrength = 0.5;
    vec3 lightColor = vec3(1,1,1);
    vec3 ambient = ambientStrength * lightColor;

    float diffuseStrength = 0;
    vec3 lightPos = vec3(0, 100, 0);
    vec3 lightDir = normalize(lightPos - fragPos);
    vec3 norm = faceNorm;
    float diff = sqrt(max(dot(norm, lightDir), 0.0));
    vec3 diffuse = diff * lightColor * diffuseStrength;


    float specularStrength = 1;
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;

    vec4 result = vec4((ambient+diffuse+specular), 1.0);
    FragColor = result * texture(tex, texCoord);
}