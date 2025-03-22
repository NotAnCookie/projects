#version 460 core

layout (location = 0) in vec3 aPos;   // Pozycja wierzchołka
layout (location = 1) in vec3 aNormal; // Normalna wierzchołka
layout (location = 2) in vec3 aColor;  // Kolor wierzchołka

uniform mat4 view;       // Macierz widoku (kamera)
uniform mat4 projection; // Macierz projekcji
uniform mat4 model;      // Macierz modelu (przemieszczenie, obrót, skala)
uniform vec3 lightPos;   // Pozycja światła
uniform vec3 lightPos3;
uniform vec3 lightColor; // Kolor światła
uniform vec3 viewPos;    // Pozycja kamery

out vec3 fragColor;      // Kolor przekazany do fragment shaderu
out vec3 fragNormal;     // Normalna
out vec3 fragPos;   // Pozycja światła w przestrzeni widoku
out vec3 lightPos2;

void main() {
    // Zastosowanie macierzy modelu, a potem view i projection
    vec3 temp = lightPos + viewPos;
    gl_Position = projection * view * model * vec4(aPos, 1.0);
    fragColor = aColor;
    // Transformacja normalnej (przestrzeń świata)
    fragNormal = mat3(transpose(inverse(model))) * aNormal;

    // Pozycja wierzchołka w przestrzeni świata
    fragPos = vec3(model * vec4(aPos, 1.0));
    lightPos2 = lightPos3;
}