#version 330 core

out vec4 FragColor;
in vec2 TexCoords;  // Współrzędne tekstury

uniform sampler2D reflectionTexture;
uniform vec3 viewPos;  // Pozycja kamery
uniform bool fogEnabled;  // Czy mgła jest włączona
uniform vec3 fogColor;  // Kolor mgły
uniform float fogDensity;  // Gęstość mgły

void main() {
    // Odwrócenie obrazu w pionie przez modyfikację Y w współrzędnych tekstury
    vec2 flippedTexCoords = vec2(TexCoords.x, 1.0 - TexCoords.y);
    
    // Pobieranie koloru z odbitej tekstury
    vec3 reflectionColor = texture(reflectionTexture, flippedTexCoords).rgb;
    
    // Obliczenie odległości od kamery do fragmentu odbicia
    // Musimy obliczyć tę odległość w przestrzeni kamery, używając współrzędnych widoku odbicia
    float distance = length(viewPos - vec3(0.0)); // Można dostosować, jeżeli masz dane o głębokości odbicia
    
    // Mgła globalna
    float fogFactor = 1.0;
    if (fogEnabled) {
        fogFactor = exp(-fogDensity * distance);  // Mgła ekspresyjna
        fogFactor = clamp(fogFactor, 0.0, 1.0);  // Ograniczenie do 0-1
    }

    // Mieszanie koloru odbicia z mgłą
    vec3 colorWithFog = mix(fogColor, reflectionColor, fogFactor);

    FragColor = vec4(colorWithFog, 1.0);  // Ustawienie koloru z efektem mgły
}


