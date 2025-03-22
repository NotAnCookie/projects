
#include "Object.h"
#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>

Object::Object(const std::vector<Vertex>& vertices)
    : vertices(vertices), modelMatrix(1.0f) {  
    setup();
}

Object::Object(const std::vector<Vertex>& vertices, const std::vector<GLuint>& indices)
    : vertices(vertices), indices(indices), useIndices(true), modelMatrix(1.0f) {  
    setup();
}


void Object::setup() {
    glGenVertexArrays(1, &VAO);
    glGenBuffers(1, &VBO);

    if (useIndices) {
        glGenBuffers(1, &EBO);
    }

    glBindVertexArray(VAO);

    // Bufor wierzcho³ków
    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), vertices.data(), GL_STATIC_DRAW);

    // Bufor indeksów
    if (useIndices) {
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(GLuint), indices.data(), GL_STATIC_DRAW);
    }

    // Atrybut pozycji
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, position));
    glEnableVertexAttribArray(0);

    // Atrybut normalnych
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, normal));
    glEnableVertexAttribArray(1);

    // Atrybut kolorów
    glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, color));
    glEnableVertexAttribArray(2);

    glBindVertexArray(0);
}

void Object::updateModelMatrix() {
    modelMatrix = glm::translate(glm::mat4(1.0f), _position);
    modelMatrix = glm::rotate(modelMatrix, glm::radians(_rotation), glm::vec3(0.0f, 1.0f, 0.0f)); // Obrót wokó³ osi Y
    modelMatrix = glm::scale(glm::mat4(1.0f), glm::vec3(_scale));
}



void Object::setPosition(float x, float y, float z) {
    _position = glm::vec3(x, y, z);
    modelMatrix = glm::translate(glm::mat4(1.0f), glm::vec3(x, y, z));
}

void Object::setScale(float scale) {
    _scale = scale;
    modelMatrix = glm::scale(glm::mat4(1.0f), glm::vec3(scale, scale, scale));
}

void Object::setRotation(float angle, float x, float y, float z) {
    _rotation = angle;
    modelMatrix = glm::rotate(glm::mat4(1.0f), glm::radians(angle), glm::vec3(x, y, z));
}



void Object::moveForward(float distance) {
    glm::vec3 direction = glm::vec3(
        glm::sin(_rotation),  
        0.0f,                           
        -glm::cos(_rotation)  
    );
    std::cout << "direction"<< (int)direction.x <<" Z " << (int)direction.z << std::endl;
    _position += direction * distance; 
    setPosition(_position.x, _position.y, _position.z);
}

void Object::moveBackward(float distance) {
    moveForward(-distance); // Przesuñ w przeciwnym kierunku
}

void Object::turn(float angle) {
    _rotation += angle; // Zmieñ k¹t obrotu
    modelMatrix = glm::rotate(glm::mat4(1.0f), glm::radians(_rotation), _position);
    //updateModelMatrix();
}



void Object::draw(GLuint shaderProgram) {
    glUseProgram(shaderProgram);
    glDisable(GL_CULL_FACE);
    GLuint lightPosLoc = glGetUniformLocation(shaderProgram, "lightPos");



    GLint modelLoc = glGetUniformLocation(shaderProgram, "model");
    if (modelLoc == -1) {
        std::cerr << "Could not find uniform 'model' in the shader program!" << std::endl;
    }
    glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelMatrix));

    glBindVertexArray(VAO);

    if (useIndices) {
        glDrawElements(GL_TRIANGLES, indices.size(), GL_UNSIGNED_INT, 0);
    }
    else {
        glDrawArrays(GL_TRIANGLES, 0, vertices.size());
    }

    glBindVertexArray(0);
}



