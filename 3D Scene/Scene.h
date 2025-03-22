#pragma once

#include <glad/glad.h>
#include <vector>
#include <cmath>
#include <glm/glm.hpp>
#include <glm/gtc/constants.hpp>
#include "Vertex.h"
#include "Shader.h"
#include "Camera.h"
#include "Object.h"
#include "ObjectData.h"
#include "Renderer.h"

class Scene {
public:
    std::vector<Object> objectList;
    std::vector<glm::vec3> originalPositions;  // Wektor do przechowywania oryginalnych pozycji obiektów

    Scene();

    void renderAll(Renderer& renderer, Shader& shader, Camera& camera, bool usePerspective);
    //void renderReflection(Renderer& renderer, Shader& shader, Camera& camera, bool usePerspective, const glm::vec3& mirrorPoint, const glm::vec3& mirrorNormal);
};
