#version 460 core

in vec3 fragColor;
in vec3 fragNormal;
in vec3 fragPos;
out vec4 finalColor;

struct Light {
    vec3 position;
    vec3 color;
    vec3 direction;
    float cutOff;
    float outerCutOff;
    int type; // 0: POINT, 1: DIRECTIONAL, 2: SPOTLIGHT
};

#define MAX_LIGHTS 10
uniform Light lights[MAX_LIGHTS];
uniform int numLights;
uniform vec3 viewPos;

uniform vec3 ambientLight;


// Parametry mgły
uniform bool fogEnabled;          // Czy mgła jest włączona
uniform vec3 fogColor;            // Kolor mgły
uniform float fogDensity;         // Gęstość mgły (całościowa mgła)

void main() {
    vec3 norm = normalize(fragNormal);
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 result = vec3(0.0);

    for (int i = 0; i < numLights; i++) {
        vec3 lightDir;
        float attenuation = 1.0;
        float distance = 0.0;

        // Parametry zanikania dla poszczególnych typów świateł
        float K_c = 1.0; // Domyślne wartości
        float K_l = 0.22;
        float K_q = 0.20;

        if (lights[i].type == 0) { // POINT
            K_c = 1.0;
            K_l = 0.09;
            K_q = 0.032;

            lightDir = normalize(lights[i].position - fragPos);
            distance = length(lights[i].position - fragPos);
            attenuation = 1.0 / (K_c + K_l * distance + K_q * (distance * distance));
        } else if (lights[i].type == 1) { // DIRECTIONAL
            K_c = 1.0; // Stała, dla kierunkowego zanikanie może być minimalne
            K_l = 0.05;
            K_q = 0.01;

            lightDir = normalize(-lights[i].direction);
            attenuation = 1.0;
        } else if (lights[i].type == 2) { // SPOTLIGHT
            K_c = 1.0;
            K_l = 0.14;
            K_q = 0.07;

            lightDir = normalize(lights[i].position - fragPos);
            distance = length(lights[i].position - fragPos);
            attenuation = 1.0 / (K_c + K_l * distance + K_q * (distance * distance));

            float theta = dot(lightDir, normalize(-lights[i].direction));
            float epsilon = lights[i].cutOff - lights[i].outerCutOff;
            float intensity = clamp((theta - lights[i].outerCutOff) / epsilon, 0.0, 1.0);
            attenuation *= intensity;
        }

        // Składowa dyfuzyjna
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * lights[i].color;

        // Składowa specularna
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
        vec3 specular = 0.5 * spec * lights[i].color;

        // Sumowanie z uwzględnieniem zanikania
        result += attenuation * (diffuse + specular);
    }
    result += ambientLight;

    // Mgła globalna
    float fogFactor = 1.0;
    if (fogEnabled) {
        float distance = length(fragPos - viewPos); // Odległość od kamery
        fogFactor = exp(-fogDensity * distance);    // Mgła ekspresyjna
        fogFactor = clamp(fogFactor, 0.0, 1.0);    // Ograniczenie do 0-1
    }

    // Mieszanie koloru obiektu z mgłą
    vec3 colorWithFog = mix(fogColor, result * fragColor, fogFactor);

    finalColor = vec4(colorWithFog, 1.0);

    //finalColor = vec4(result * fragColor, 1.0);
}

