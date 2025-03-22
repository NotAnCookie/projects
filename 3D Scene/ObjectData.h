#pragma once
#ifndef OBJECTDATA_H
#define OBJECTDATA_H

#include <glad/glad.h>
#include <vector>
#include <cmath>
#include <glm/glm.hpp>
#include <glm/gtc/constants.hpp>
#include "Vertex.h"
#include <glm/gtc/matrix_transform.hpp>


#ifndef PI
#define PI 3.14159265358979323846
#endif



// Deklaracje zmiennych globalnych
extern std::vector<Vertex> cubeVertices;
extern std::vector<Vertex> wallVertices;
extern std::vector<GLuint> wallIndices;
extern std::vector<GLuint> cubeIndices;

// Funkcja generuj¹ca kulê
void generateSphere(float radius, int sectorCount, int stackCount,
    std::vector<Vertex>& vertices, std::vector<GLuint>& indices);


std::vector<Vertex> scaleVertical(const std::vector<Vertex>& vertices, float scaleFactor);
std::vector<Vertex> scaleHorizontal(const std::vector<Vertex>& vertices, float scaleFactor);
std::vector<Vertex> scaleThickness(const std::vector<Vertex>& vertices, float scaleFactor);
std::vector<Vertex> rotateWall(const std::vector<Vertex>& vertices, float angleDegrees, const glm::vec3& axis);
std::vector<Vertex> changeColor(const std::vector<Vertex>& vertices, const glm::vec3& newColor);



//// Funkcja generuj¹ca powierzchniê Béziera
//std::vector<GLfloat> generateBezierSurface(const std::vector<glm::vec3>& controlPoints, int resolution) {
//    std::vector<GLfloat> surfaceVertices;
//
//    for (int u = 0; u <= resolution; ++u) {
//        float uNorm = (float)u / resolution;
//        for (int v = 0; v <= resolution; ++v) {
//            float vNorm = (float)v / resolution;
//
//            // Obliczenie wspó³rzêdnych Béziera (dla uproszczenia 4x4 kontrolne)
//            glm::vec3 point(0.0f);
//            for (int i = 0; i < 4; ++i) {
//                for (int j = 0; j < 4; ++j) {
//                    float bernsteinU = bernstein(i, 3, uNorm);
//                    float bernsteinV = bernstein(j, 3, vNorm);
//                    point += controlPoints[i * 4 + j] * bernsteinU * bernsteinV;
//                }
//            }
//
//            // Dodanie wierzcho³ka
//            surfaceVertices.push_back(point.x);
//            surfaceVertices.push_back(point.y);
//            surfaceVertices.push_back(point.z);
//
//            // Normalne (na razie jednostkowe wektory)
//            surfaceVertices.push_back(0.0f);
//            surfaceVertices.push_back(0.0f);
//            surfaceVertices.push_back(1.0f);
//
//            // Kolor
//            surfaceVertices.push_back(0.5f);
//            surfaceVertices.push_back(0.2f);
//            surfaceVertices.push_back(0.7f);
//        }
//    }
//    return surfaceVertices;
//}
//
//// Funkcja pomocnicza dla Béziera
//float bernstein(int i, int n, float t) {
//    float binomialCoeff = 1.0f;
//    for (int k = 0; k < i; ++k)
//        binomialCoeff *= (float)(n - k) / (k + 1);
//    return binomialCoeff * pow(t, i) * pow(1 - t, n - i);
//}

#endif
