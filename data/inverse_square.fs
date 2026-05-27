#version 330

in vec3 fragPos;
in vec3 normal;

out vec4 finalColor;

uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;

void main()
{
    vec3 norm = normalize(normal);

    vec3 lightDir = lightPos - fragPos;
    float distance = length(lightDir);
    lightDir = normalize(lightDir);

    // Lambert diffuse
    float diff = max(dot(norm, lightDir), 0.0);

    // Inverse square attenuation
    float attenuation = 1.0 / (distance * distance);

    vec3 color = lightColor * diff * attenuation * 50.0;

    finalColor = vec4(color, 1.0);
}