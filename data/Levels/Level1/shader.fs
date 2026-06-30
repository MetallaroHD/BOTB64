#version 330

in vec2 fragTexCoord;
in vec3 vertColor;

uniform sampler2D texture0;

out vec4 finalColor;

void main()
{
    vec4 texColor = texture(texture0, fragTexCoord);
    vec3 lit = texColor.rgb * vertColor;

    // Mild contrast/saturation push — N64 textures were low-res but punchy
    lit = clamp((lit - 0.5) * 1.08 + 0.5, 0.0, 1.0);

    finalColor = vec4(lit, texColor.a);
}