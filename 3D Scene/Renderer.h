#pragma once
#ifndef RENDERER_H
#define RENDERER_H

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include "Shader.h"
#include "Object.h"
#include "Camera.h"

class Renderer {
public:
    void render(Object& object, Shader& shader, Camera& camera, bool usePerspective);
};

#endif
