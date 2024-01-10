#version 460 core
out vec4 FragColor;
in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform sampler2D tex;
uniform vec3 viewPos;

struct Material{
    float ambient;
    float diffuse;
    float specular;

    float shininess;
};

uniform Material material;

struct PointLight {
    vec3 color;
    float strength;
    
    float constant;
    float linear;
    float quadratic;

    vec3 pos;
};
#define MAX_LIGHTS 32
uniform PointLight pointLights[MAX_LIGHTS];
uniform int numLights;

struct DirectionalLight {
    vec3 color;
    float strength;

    vec3 direction;
};
uniform DirectionalLight directionalLight;

vec3 calcPointLight(PointLight light, vec3 norm, vec3 viewDir){
    vec3 lightVec = light.pos - fragPos;
    float dist = lightVec.length();
    
    vec3 lightDir = normalize(lightVec);
    vec3 lightBase = light.color * light.strength *  1 / (light.constant + light.linear * dist + light.quadratic * pow(dist, 2));
    
    vec3 ambient = lightBase;
    vec3 diffuse = max(dot(norm, lightDir), 0.0) * lightBase;

    vec3 reflectDir = reflect(-lightDir, norm);
    vec3 specular = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess) * lightBase;
    
    return ambient * material.ambient + diffuse * material.diffuse + specular * material.specular;
}

vec3 calcDirLight(DirectionalLight light, vec3 norm, vec3 viewDir){
    vec3 lightVec = light.direction;
    float dist = lightVec.length();

    vec3 lightDir = normalize(lightVec);
    vec3 reflectDir = reflect(-lightDir, norm);

    vec3 lightBase = light.color * light.strength;

    vec3 ambient = lightBase;
    vec3 diffuse = max(dot(norm, lightDir), 0.0) * lightBase;
    vec3 specular = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess) * lightBase;

    return ambient * material.ambient + diffuse * material.diffuse + specular * material.specular;
}

void main()
{
    vec3 light = vec3(0.0);
    vec3 norm = normalize(normal);
    vec3 viewDirection = normalize(viewPos - fragPos);

    for (int i = 0; i < numLights; i++) {
        light += calcPointLight(pointLights[i], norm, viewDirection);
    }
    light += calcDirLight(directionalLight, norm, viewDirection) * 0;

    vec4 result = vec4(light, 1.0);
    FragColor = result * texture(tex, texCoord);
}