#pragma once
#ifndef SKYBOX_H
#define SKYBOX_H

#include <vector>
#include <string>
#include <glad/glad.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include "shader.h"
#include "camera.h"

class Skybox {
public:
    // Konstruktor i destruktor
    Skybox();
    ~Skybox();

    // Funkcja ³aduj¹ca tekstury dla Skyboxa
    void loadSkyboxTextures(const std::vector<std::string>& faces);

    // Funkcja renderuj¹ca Skybox
    void render(Shader& shaderSkybox, Camera& camera, bool usePerspective);

    void changeTexture(const std::vector<std::string>& newFaces);

    void createSolidColorSkybox(const glm::vec3& color);

    void updateSkybox(bool isDay, bool fogEnabled, float fogDensity, const glm::vec3& fogColor);

private:
    GLuint vao;              // VAO dla skyboxa
    GLuint cubemapTexture;   // ID tekstury cubemap
    GLuint skyboxVBO;        // VBO dla skyboxa
    void setupSkybox();      // Funkcja do inicjalizacji VAO i VBO
};

#endif

