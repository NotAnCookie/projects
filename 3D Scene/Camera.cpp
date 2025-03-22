#include "Camera.h"
#include <iostream>

Camera::Camera(int width, int height, glm::vec3 position)
    : width(width), height(height), position(position), front(glm::vec3(0.0f, 0.0f, -1.0f)) {
    updateProjectionMatrix();
    updateViewMatrix();
    std::cout << "Camera created at position: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

void Camera::updateProjectionMatrix() {
    projection = glm::perspective(glm::radians(45.0f), (float)width / (float)height, 0.1f, 100.0f); // rzutowanie perspektywiczne
    float orthoWidth = 4.0f; 
    float orthoHeight = 3.0f; 
    orthoProjection = glm::ortho(-orthoWidth, orthoWidth, -orthoHeight, orthoHeight, 0.1f, 100.0f); // rzutowanie ortogonalne
}

void Camera::updateViewMatrix() {
    view = glm::lookAt(position, position + front, up);
}

void Camera::moveForward(float deltaTime) {
    position += speed * deltaTime * front;
    std::cout << "Moved forward to: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

void Camera::moveBackward(float deltaTime) {
    position -= speed * deltaTime * front;
    std::cout << "Moved backward to: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

void Camera::moveLeft(float deltaTime) {
    position -= glm::normalize(glm::cross(front, up)) * speed * deltaTime;
    std::cout << "Moved left to: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

void Camera::moveRight(float deltaTime) {
    position += glm::normalize(glm::cross(front, up)) * speed * deltaTime;
    std::cout << "Moved right to: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

void Camera::moveUp(float deltaTime) {
    position += speed * deltaTime * up;
    std::cout << "Moved up to: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

void Camera::moveDown(float deltaTime) {
    position -= speed * deltaTime * up;
    std::cout << "Moved down to: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

void Camera::processKeyboard(GLFWwindow* window, float deltaTime) {
    if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS) {
        moveForward(deltaTime);
    }
    if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS) {
        moveBackward(deltaTime);
    }
    if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS) {
        moveLeft(deltaTime);
    }
    if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS) {
        moveRight(deltaTime);
    }
    if (glfwGetKey(window, GLFW_KEY_UP) == GLFW_PRESS) {
        moveUp(deltaTime);
    }
    if (glfwGetKey(window, GLFW_KEY_DOWN) == GLFW_PRESS) {
        moveDown(deltaTime);
    }
}

glm::mat4 Camera::getViewMatrix() const {
    return view;
}

glm::mat4 Camera::getProjectionMatrix(bool usePerspective) const {
    return usePerspective ? projection : orthoProjection;
}

glm::vec3 Camera::getPosition() const {
    return position;
}

void Camera::setViewMatrix(const glm::mat4& newViewMatrix) {
    this->viewLast = view;
    this->view = newViewMatrix;
}

void Camera::resetViewMatrix() {
    this->view = viewLast; 
}





void Camera::setPosition(const glm::vec3& newPosition) {
    position = newPosition;
    updateViewMatrix();
    std::cout << "Camera position set to: "
        << position.x << ", " << position.y << ", " << position.z << std::endl;
}

// Funkcja do ustawiania kierunku patrzenia kamery w oparciu o obrót obiektu
void Camera::setDirectionFromObject(const glm::vec3& objectPosition, float rotationAngle, const glm::vec3& rotationAxis) {
    glm::vec3 direction = glm::normalize(objectPosition - position);  // Wektor w stronê obiektu
    glm::mat4 rotationMatrix = glm::rotate(glm::mat4(1.0f), glm::radians(rotationAngle), rotationAxis);
    glm::vec3 rotatedDirection = glm::vec3(rotationMatrix * glm::vec4(direction, 1.0f));  // Obrót wokó³ osi
    direction = rotatedDirection;

    front = direction; 
    updateViewMatrix();

    std::cout << "Camera direction updated based on object rotation." << std::endl;
}
