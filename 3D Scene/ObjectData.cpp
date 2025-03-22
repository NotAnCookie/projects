#include "ObjectData.h"
#include <glm/glm.hpp>
#include <glm/gtc/constants.hpp>

std::vector<Vertex> cubeVertices = {
    // Front face (z = -0.5f)
    {{-0.5f, -0.5f, -0.5f}, { 0.0f,  0.0f, -1.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f, -0.5f, -0.5f}, { 0.0f,  0.0f, -1.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f,  0.5f, -0.5f}, { 0.0f,  0.0f, -1.0f}, {1.0f, 0.0f, 0.0f}},
    {{-0.5f,  0.5f, -0.5f}, { 0.0f,  0.0f, -1.0f}, {1.0f, 0.0f, 0.0f}},

    // Back face (z = 0.5f)
    {{-0.5f, -0.5f,  0.5f}, { 0.0f,  0.0f,  1.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f, -0.5f,  0.5f}, { 0.0f,  0.0f,  1.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f,  0.5f,  0.5f}, { 0.0f,  0.0f,  1.0f}, {1.0f, 0.0f, 0.0f}},
    {{-0.5f,  0.5f,  0.5f}, { 0.0f,  0.0f,  1.0f}, {1.0f, 0.0f, 0.0f}},

    // Left face (x = -0.5f)
    {{-0.5f, -0.5f, -0.5f}, {-1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{-0.5f, -0.5f,  0.5f}, {-1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{-0.5f,  0.5f,  0.5f}, {-1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{-0.5f,  0.5f, -0.5f}, {-1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},

    // Right face (x = 0.5f)
    {{ 0.5f, -0.5f, -0.5f}, { 1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f, -0.5f,  0.5f}, { 1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f,  0.5f,  0.5f}, { 1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f,  0.5f, -0.5f}, { 1.0f,  0.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},

    // Top face (y = 0.5f)
    {{-0.5f,  0.5f, -0.5f}, { 0.0f,  1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f,  0.5f, -0.5f}, { 0.0f,  1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f,  0.5f,  0.5f}, { 0.0f,  1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{-0.5f,  0.5f,  0.5f}, { 0.0f,  1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},

    // Bottom face (y = -0.5f)
    {{-0.5f, -0.5f, -0.5f}, { 0.0f, -1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f, -0.5f, -0.5f}, { 0.0f, -1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{ 0.5f, -0.5f,  0.5f}, { 0.0f, -1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}},
    {{-0.5f, -0.5f,  0.5f}, { 0.0f, -1.0f,  0.0f}, {1.0f, 0.0f, 0.0f}}
};


std::vector<GLuint> cubeIndices = {
    // Front face
    0, 1, 2,
    2, 3, 0,

    // Back face
    4, 5, 6,
    6, 7, 4,

    // Left face
    8, 9, 10,
    10, 11, 8,

    // Right face
    12, 13, 14,
    14, 15, 12,

    // Top face
    16, 17, 18,
    18, 19, 16,

    // Bottom face
    20, 21, 22,
    22, 23, 20
};



std::vector<Vertex> wallVertices = {
    {{-0.5f, -0.5f, 0.0f}, {0.0f, 0.0f, 1.0f}, {0.8f, 0.8f, 0.8f}},
    {{ 0.5f, -0.5f, 0.0f}, {0.0f, 0.0f, 1.0f}, {0.8f, 0.8f, 0.8f}},
    {{ 0.5f,  0.5f, 0.0f}, {0.0f, 0.0f, 1.0f}, {0.8f, 0.8f, 0.8f}},
    {{-0.5f,  0.5f, 0.0f}, {0.0f, 0.0f, 1.0f}, {0.8f, 0.8f, 0.8f}}
};

std::vector<GLuint> wallIndices = {
    0, 1, 2,
    2, 3, 0
};

// Funkcja skaluj¹ca w pionie (Y)
std::vector<Vertex> scaleVertical(const std::vector<Vertex>& vertices, float scaleFactor) {
    std::vector<Vertex> scaledVertices = vertices; 
    for (auto& vertex : scaledVertices) {
        vertex.position.y *= scaleFactor;  
    }
    return scaledVertices;  
}

// Funkcja skaluj¹ca w poziomie (X)
std::vector<Vertex> scaleHorizontal(const std::vector<Vertex>& vertices, float scaleFactor) {
    std::vector<Vertex> scaledVertices = vertices;  
    for (auto& vertex : scaledVertices) {
        vertex.position.x *= scaleFactor;  
    }
    return scaledVertices; 
}

// Funkcja skaluj¹ca w gruboœci (Z)
std::vector<Vertex> scaleThickness(const std::vector<Vertex>& vertices, float scaleFactor) {
    std::vector<Vertex> scaledVertices = vertices;  
    for (auto& vertex : scaledVertices) {
        vertex.position.z *= scaleFactor; 
    }
    return scaledVertices;  
}

std::vector<Vertex> rotateWall(const std::vector<Vertex>& vertices, float angleDegrees, const glm::vec3& axis) {
    // Przeliczenie k¹ta ze stopni na radiany
    float angleRadians = glm::radians(angleDegrees);

    // Tworzenie macierzy rotacji
    glm::mat4 rotationMatrix = glm::rotate(glm::mat4(1.0f), angleRadians, axis);

    // Kopia wierzcho³ków, które bêdziemy modyfikowaæ
    std::vector<Vertex> rotatedVertices = vertices;

    for (auto& vertex : rotatedVertices) {
        // Rozszerzenie pozycji wierzcho³ka do wspó³rzêdnych jednorodnych (dodanie 1.0 na koñcu)
        glm::vec4 homogenousPosition = glm::vec4(vertex.position, 1.0f);

        // Mno¿enie przez macierz rotacji
        glm::vec4 rotatedPosition = rotationMatrix * homogenousPosition;

        // Aktualizacja pozycji wierzcho³ka
        vertex.position = glm::vec3(rotatedPosition);

        // Aktualizacja normalnej wierzcho³ka (bez translacji, tylko rotacja)
        glm::vec4 homogenousNormal = glm::vec4(vertex.normal, 0.0f);
        glm::vec4 rotatedNormal = rotationMatrix * homogenousNormal;
        vertex.normal = glm::vec3(rotatedNormal);
    }

    return rotatedVertices;
}

std::vector<Vertex> changeColor(const std::vector<Vertex>& vertices, const glm::vec3& newColor) {
    std::vector<Vertex> updatedVertices = vertices;
    for (auto& vertex : updatedVertices) {
        vertex.color = newColor;
    }
    return updatedVertices;
}




// Funkcja generuj¹ca kulê
void generateSphere(float radius, int sectorCount, int stackCount,
    std::vector<Vertex>& vertices, std::vector<GLuint>& indices) {
    // Generowanie wierzcho³ków
    for (int stack = 0; stack <= stackCount; ++stack) {
        float stackAngle = glm::pi<float>() / 2 - stack * glm::pi<float>() / stackCount;
        float xy = radius * cosf(stackAngle);
        float z = radius * sinf(stackAngle);

        for (int sector = 0; sector <= sectorCount; ++sector) {
            float sectorAngle = sector * 2 * glm::pi<float>() / sectorCount;

            Vertex vertex;
            vertex.position = glm::vec3(xy * cosf(sectorAngle), xy * sinf(sectorAngle), z);
            vertex.normal = glm::normalize(vertex.position);
            vertex.color = glm::vec3(1.0f, 0.5f, 0.2f); // kolor obiektu
            vertices.push_back(vertex);
        }
    }

    // Generowanie indeksów
    for (int stack = 0; stack < stackCount; ++stack) {
        int k1 = stack * (sectorCount + 1);
        int k2 = k1 + sectorCount + 1;

        for (int sector = 0; sector < sectorCount; ++sector, ++k1, ++k2) {
            if (stack != 0) {
                indices.push_back(k1);
                indices.push_back(k2);
                indices.push_back(k1 + 1);
            }

            if (stack != (stackCount - 1)) {
                indices.push_back(k1 + 1);
                indices.push_back(k2);
                indices.push_back(k2 + 1);
            }
        }
    }
}