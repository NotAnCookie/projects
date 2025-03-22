#pragma once
#ifndef SHADER_H
#define SHADER_H


#include <glad/glad.h>
#include <GLFW/glfw3.h>
#include <string>

class Shader {
public:
    GLuint programID;

    Shader(const char* vertexPath, const char* fragmentPath);
    void use();
    GLuint loadShader(const char* shaderPath, GLenum shaderType);

    // Dodajemy metodê getProgram, która zwróci program shader
    GLuint getProgram() const { return programID; }

private:
    void checkCompileErrors(GLuint shader, std::string type);

    GLuint getUniformLocation(const char* uniformName);
};

#endif

