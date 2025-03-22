#pragma once
#ifndef OBJECT_H
#define OBJECT_H

#include <glad/glad.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <vector>
#include "Vertex.h"
#include <iostream>
//#include "ObjectData.h"

// Struktura wierzcho³ka
//struct Vertex {
//    glm::vec3 position;
//    glm::vec3 normal;
//    glm::vec3 color;
//};

class Object {
public:
    Object(const std::vector<Vertex>& vertices); // Konstruktor dla wierzcho³ków
    Object(const std::vector<Vertex>& vertices, const std::vector<GLuint>& indices); // Konstruktor z indeksami

    void draw(GLuint shaderProgram);
    void setPosition(float x, float y, float z);
    void setScale(float scale);
    void setRotation(float angle, float x, float y, float z);
    void updateModelMatrix();

    void moveForward(float distance);

    void moveBackward(float distance);

    void turn(float angle);

    glm::vec3 _position;
    float _rotation;
    float _scale;

private:
    std::vector<Vertex> vertices;
    std::vector<GLuint> indices;

    GLuint VAO, VBO, EBO;
    bool useIndices = false;

    glm::mat4 modelMatrix = glm::mat4(1.0f);

    void setup();
};

#endif


