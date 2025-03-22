#pragma once
#include <glad/glad.h>
#include <glm/glm.hpp>
#include <iostream>

class Light {
public:
    enum class LightType {
        POINT,
        DIRECTIONAL,
        SPOTLIGHT
    };

    Light(LightType type, glm::vec3 position, glm::vec3 color, glm::vec3 direction = glm::vec3(0.0f, -1.0f, 0.0f), float cutOff = glm::radians(12.5f), float outerCutOff = glm::radians(17.5f));

    // Ustawienia pozycji i kierunku
    void setPosition(const glm::vec3& position);
    void setDirection(const glm::vec3& direction);

    // Pobranie wartoœci
    glm::vec3 getPosition() const;
    glm::vec3 getDirection() const;
    glm::vec3 getColor() const;
    float getCutOff() const;
    float getOuterCutOff() const;
    LightType getType() const;

    // Aktualizacja wartoœci w shaderze
    void applyToShader(GLuint shaderProgram, const std::string& uniformName) const;

    // Ruch œwiat³a
    void move(const glm::vec3& delta);
    void rotate(const glm::vec3& axis, float angle);

private:
    LightType type;
    glm::vec3 position;
    glm::vec3 direction;
    glm::vec3 color;
    float cutOff;       // K¹t wewnêtrzny dla reflektorów
    float outerCutOff;  // K¹t zewnêtrzny dla reflektorów
};
