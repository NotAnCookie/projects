#include "Light.h"
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

Light::Light(LightType type, glm::vec3 position, glm::vec3 color, glm::vec3 direction, float cutOff, float outerCutOff)
    : type(type), position(position), color(color), direction(glm::normalize(direction)), cutOff(cutOff), outerCutOff(outerCutOff) {
}

void Light::setPosition(const glm::vec3& position) {
    this->position = position;
}

void Light::setDirection(const glm::vec3& direction) {
    this->direction = glm::normalize(direction);
}

glm::vec3 Light::getPosition() const {
    return position;
}

glm::vec3 Light::getDirection() const {
    return direction;
}

glm::vec3 Light::getColor() const {
    return color;
}

float Light::getCutOff() const {
    return cutOff;
}

float Light::getOuterCutOff() const {
    return outerCutOff;
}

Light::LightType Light::getType() const {
    return type;
}

void Light::applyToShader(GLuint shaderProgram, const std::string& uniformName) const {
    glUniform3fv(glGetUniformLocation(shaderProgram, (uniformName + ".position").c_str()), 1, glm::value_ptr(position));
    glUniform3fv(glGetUniformLocation(shaderProgram, (uniformName + ".color").c_str()), 1, glm::value_ptr(color));
    glUniform1i(glGetUniformLocation(shaderProgram, (uniformName + ".type").c_str()), static_cast<int>(type));

    if (type != LightType::POINT) {
        glUniform3fv(glGetUniformLocation(shaderProgram, (uniformName + ".direction").c_str()), 1, glm::value_ptr(direction));
    }
    if (type == LightType::SPOTLIGHT) {
        glUniform1f(glGetUniformLocation(shaderProgram, (uniformName + ".cutOff").c_str()), cutOff);
        glUniform1f(glGetUniformLocation(shaderProgram, (uniformName + ".outerCutOff").c_str()), outerCutOff);
    }
}

void Light::move(const glm::vec3& delta) {
    position += delta;
}

void Light::rotate(const glm::vec3& axis, float angle) {
    glm::mat4 rotationMatrix = glm::rotate(glm::mat4(1.0f), glm::radians(angle), axis);
    glm::vec4 rotatedDirection = rotationMatrix * glm::vec4(direction, 0.0f);
    direction = glm::vec3(rotatedDirection);
}

