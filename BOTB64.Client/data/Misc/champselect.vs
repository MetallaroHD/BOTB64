#version 330
in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexTexCoord;
uniform mat4 mvp;
uniform mat4 matModel;
uniform vec3 camPos;
uniform vec3 lightColor;
out vec2 fragTexCoord;
out vec3 vertColor;
void main()
{
    vec3 worldPos = vec3(matModel * vec4(vertexPosition, 1.0));
    vec3 norm = normalize(mat3(transpose(inverse(matModel))) * vertexNormal);
    vec3 lightDir = normalize(camPos - worldPos);
    float diff = max(dot(norm, lightDir), 0.0);
    float ambient = 0.35;
    vertColor = lightColor * (diff + ambient);
    fragTexCoord = vertexTexCoord;
    gl_Position = mvp * vec4(vertexPosition, 1.0);
}