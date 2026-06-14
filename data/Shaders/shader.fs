#version 330
in vec3 fragPos;
in vec3 normal;
in vec2 fragTexCoord;

out vec4 finalColor;

uniform vec3 lightDir;
uniform vec3 lightColor;
uniform sampler2D texture0;

void main()
{
    vec4 texColor = texture(texture0, fragTexCoord);

    vec3 norm = normalize(normal);
    vec3 L = normalize(-lightDir);
    float diff = max(dot(norm, L), 0.0);

    float ambient = 0.15;
    vec3 lighting = lightColor * (diff + ambient);

    finalColor = vec4(texColor.rgb * lighting, texColor.a);
}