#version 460 core
out vec4 FragColor;
in vec2 texCoord;

uniform sampler2D tex;

void main()
{
    vec4 texColor0 = texture(tex, texCoord);
    if (texColor0[3] < 0.01){
        discard;
    }
    FragColor = texColor0;
}