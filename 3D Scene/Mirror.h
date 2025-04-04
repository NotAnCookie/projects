#pragma once
#ifndef MIRROR_H
#define MIRROR_H

#include <glad/glad.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include <functional>
#include "Camera.h"
#include "Shader.h"
#include "Object.h"


class Mirror {
private:
    GLuint framebuffer;          // Framebuffer dla odbicia
    GLuint reflectionTexture;    // Tekstura odbicia
    GLuint depthBuffer;          // Bufor głębokości
    GLuint vao, vbo, ebo;        // Buffery dla geometrii lustra
    glm::vec3 position;          // Pozycja lustra w przestrzeni świata
    glm::vec2 size;              // Rozmiar lustra (szerokość, wysokość)

    int screenWidth, screenHeight; // Rozdzielczość ekranu

    void setupFramebuffer();     // Konfiguracja framebuffer
    void setupGeometry();        // Konfiguracja VAO/VBO/EBO lustra

public:
    // Konstruktor i destruktor
    Mirror(int screenWidth, int screenHeight, glm::vec3 position, glm::vec2 size);
    ~Mirror();

    // Funkcje publiczne
    void renderReflection(Camera& camera,bool usePerspective, const std::function<void()>& renderScene, const std::function<void()>& renderSkybox, Shader& shader, const std::function<void()>& renderCar);
    void renderMirror(Camera& camera, bool usePerspective, GLuint shaderProgram);

    GLuint getReflectionTexture() const { return reflectionTexture; }
};

#endif // MIRROR_H
