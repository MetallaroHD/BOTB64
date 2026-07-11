#version 330

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexTexCoord;
uniform mat4 mvp;
uniform mat4 matModel;
uniform vec3 lightDir;     // key light
uniform vec3 lightColor;
uniform vec3 fillDir;      // secondary fill light (symmetric, low intensity)
uniform vec3 fillColor;
out vec2 fragTexCoord;
out vec3 vertColor;        // pre-lit color, just like N64 Gouraud shading
out vec3 fragPosition;     // world-space position, needed for fog distance

void main()
{
    vec3 worldPos = vec3(matModel * vec4(vertexPosition, 1.0));
    vec3 norm = normalize(mat3(transpose(inverse(matModel))) * vertexNormal);

    // Key light, toon-banded into 3 steps instead of smooth
    float diffKey = max(dot(norm, normalize(-lightDir)), 0.0);
    float bandedKey = floor(diffKey * 3.0) / 3.0;

    // Fill light — soft, no banding, just lifts the dark side
    float diffFill = max(dot(norm, normalize(-fillDir)), 0.0);
    float ambient = 0.35;

    vertColor = lightColor * (bandedKey + ambient) + fillColor * diffFill * 0.25;
    fragTexCoord = vertexTexCoord;
    fragPosition = worldPos;

    gl_Position = mvp * vec4(vertexPosition, 1.0);
}