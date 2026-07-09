#version 330
in vec2 fragTexCoord;
in vec3 vertColor;
uniform sampler2D texture0;
out vec4 finalColor;
void main()
{
    vec4 texColor = texture(texture0, fragTexCoord);
    vec3 lit = texColor.rgb * vertColor;
    finalColor = vec4(lit, 1.0);
}