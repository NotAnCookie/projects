#pragma once
#ifndef CAMERA_H
#define CAMERA_H

#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

// Kamera FPP (First Person Perspective) lub TPP (Third Person Perspective)
class Camera {
public:
    Camera(int width, int height, glm::vec3 position);

    void updateProjectionMatrix();
    void updateViewMatrix();

    // Metody do poruszania kamer¹ (np. do wprowadzenia sterowania)
    void moveForward(float deltaTime);
    void moveBackward(float deltaTime);
    void moveLeft(float deltaTime);
    void moveRight(float deltaTime);
    void moveUp(float deltaTime);
    void moveDown(float deltaTime);

    void processKeyboard(GLFWwindow* window, float deltaTime);

    glm::mat4 getViewMatrix() const;
    glm::mat4 getProjectionMatrix(bool usePerspective) const;

    glm::vec3 getPosition() const;

    glm::vec3 position;
    glm::vec3 positionLast;
    glm::vec3 front;
    glm::vec3 up = glm::vec3(0.0f, 1.0f, 0.0f); // Domyœlna wektor górny

    float speed = 3.0f; // Prêdkoœæ poruszania siê kamery
    int width, height;

    void setViewMatrix(const glm::mat4& newViewMatrix);
    void resetViewMatrix();

    // Funkcja do ustawiania pozycji kamery
    void setPosition(const glm::vec3& newPosition);

    // Funkcja do ustawiania kierunku patrzenia kamery w oparciu o obrót obiektu
    void setDirectionFromObject(const glm::vec3& objectPosition, float rotationAngle, const glm::vec3& rotationAxis);

private:
    glm::mat4 projection;
    glm::mat4 orthoProjection;
    glm::mat4 view;
    glm::mat4 viewLast;
};


#endif
