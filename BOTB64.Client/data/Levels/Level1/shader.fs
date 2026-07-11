#version 330
in vec2 fragTexCoord;
in vec3 vertColor;
in vec3 fragPosition;

uniform sampler2D texture0;
uniform vec4 fogColor;
uniform float fogStartRadius;   // distance from arena center where fog begins
uniform float fogEndRadius;     // distance from arena center where fog is fully opaque

out vec4 finalColor;

void main()
{
    vec4 texColor = texture(texture0, fragTexCoord);
    vec3 lit = texColor.rgb * vertColor;
    lit = clamp((lit - 0.5) * 1.08 + 0.5, 0.0, 1.0);

    // distance from arena center (world origin), not camera
    float distFromCenter = length(fragPosition.xz);

    float fogFactor = smoothstep(fogStartRadius, fogEndRadius, distFromCenter);

    vec3 finalRGB = mix(lit, fogColor.rgb, fogFactor);
    finalColor = vec4(finalRGB, 1.0);
}