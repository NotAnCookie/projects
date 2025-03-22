#include "skybox.h"
#include <iostream>
#include "stb_image.h"

// Konstruktor
Skybox::Skybox() {
    setupSkybox();  // Tworzymy VAO i VBO
}

// Destruktor
Skybox::~Skybox() {
    if (cubemapTexture != 0) {
        glDeleteTextures(1, &cubemapTexture);
    }
    if (vao != 0) {
        glDeleteVertexArrays(1, &vao);
    }
    if (skyboxVBO != 0) {
        glDeleteBuffers(1, &skyboxVBO);
    }
}

// Funkcja ³aduj¹ca tekstury dla Skyboxa
void Skybox::loadSkyboxTextures(const std::vector<std::string>& faces) {
    glGenTextures(1, &cubemapTexture);
    glBindTexture(GL_TEXTURE_CUBE_MAP, cubemapTexture);

    int width, height, nrChannels;
    for (unsigned int i = 0; i < faces.size(); i++) {
        unsigned char* data = stbi_load(faces[i].c_str(), &width, &height, &nrChannels, 0);
        if (data) {
            glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, data);
            stbi_image_free(data);
        }
        else {
            std::cerr << "Failed to load cubemap texture at " << faces[i] << std::endl;
            stbi_image_free(data);
        }
    }

    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);
}

// Funkcja tworz¹ca VAO i VBO dla Skyboxa
void Skybox::setupSkybox() {
    float skyboxVertices[] = {
        // Right
        -100.0f,  100.0f, -100.0f, -100.0f, -100.0f, -100.0f,  100.0f, -100.0f, -100.0f,
         100.0f, -100.0f, -100.0f,  100.0f,  100.0f, -100.0f, -100.0f,  100.0f, -100.0f,
         // Left
         -100.0f, -100.0f,  100.0f, -100.0f, -100.0f, -100.0f, -100.0f,  100.0f, -100.0f,
         -100.0f,  100.0f, -100.0f, -100.0f,  100.0f,  100.0f, -100.0f, -100.0f,  100.0f,
         // Top
          100.0f,  100.0f,  100.0f,  100.0f,  100.0f, -100.0f, -100.0f,  100.0f, -100.0f,
         -100.0f,  100.0f,  100.0f, -100.0f,  100.0f, -100.0f,  100.0f,  100.0f,  100.0f,
         // Bottom
         -100.0f, -100.0f,  100.0f, -100.0f, -100.0f, -100.0f,  100.0f, -100.0f, -100.0f,
          100.0f, -100.0f,  100.0f,  100.0f, -100.0f, -100.0f, -100.0f, -100.0f,  100.0f,
          // Front
          -100.0f, -100.0f,  100.0f, -100.0f,  100.0f,  100.0f,  100.0f,  100.0f,  100.0f,
           100.0f, -100.0f,  100.0f,  100.0f,  100.0f,  100.0f, -100.0f, -100.0f,  100.0f,
           // Back
           -100.0f, -100.0f, -100.0f, -100.0f,  100.0f, -100.0f,  100.0f,  100.0f, -100.0f,
            100.0f, -100.0f, -100.0f,  100.0f,  100.0f, -100.0f, -100.0f, -100.0f, -100.0f
    };


    // Utworzenie VAO
    glGenVertexArrays(1, &vao);
    glBindVertexArray(vao);

    // Utworzenie VBO
    glGenBuffers(1, &skyboxVBO);
    glBindBuffer(GL_ARRAY_BUFFER, skyboxVBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(skyboxVertices), skyboxVertices, GL_STATIC_DRAW);

    // Ustawienia atrybutów wierzcho³ków
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);

    glBindVertexArray(0);
}


void Skybox::render(Shader& shaderSkybox, Camera& camera, bool usePerspective) {
    glDepthFunc(GL_LEQUAL);

    shaderSkybox.use();

    glm::mat4 view = glm::mat4(1.0f); 
    glm::mat4 projection = glm::perspective(glm::radians(45.0f), 1.0f, 0.1f, 100.0f); 

    GLuint viewLoc = glGetUniformLocation(shaderSkybox.getProgram(), "view");
    glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));

    GLuint projectionLoc = glGetUniformLocation(shaderSkybox.getProgram(), "projection");
    glUniformMatrix4fv(projectionLoc, 1, GL_FALSE, glm::value_ptr(projection));

    glBindVertexArray(vao);
    glBindTexture(GL_TEXTURE_CUBE_MAP, cubemapTexture);
    glDrawArrays(GL_TRIANGLES, 0, 36);
    glBindVertexArray(0);

    glDepthFunc(GL_LESS);
}





void Skybox::changeTexture(const std::vector<std::string>& newFaces) {
    glDeleteTextures(1, &cubemapTexture);
    loadSkyboxTextures(newFaces);  
}

void Skybox::createSolidColorSkybox(const glm::vec3& color) {
    unsigned char solidColor[3] = {
        static_cast<unsigned char>(color.r * 255),
        static_cast<unsigned char>(color.g * 255),
        static_cast<unsigned char>(color.b * 255)
    };

    glGenTextures(1, &cubemapTexture);
    glBindTexture(GL_TEXTURE_CUBE_MAP, cubemapTexture);

    for (unsigned int i = 0; i < 6; i++) {
        glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, GL_RGB, 1, 1, 0, GL_RGB, GL_UNSIGNED_BYTE, solidColor);
    }

    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);
}

void Skybox::updateSkybox(bool isDay,bool fogEnabled,float fogDensity,const glm::vec3& fogColor) {
    glm::vec3 daySkyColor(0.2f, 0.6f, 1.0f);    // Kolor skyboxa w dzieñ
    glm::vec3 nightSkyColor(0.02f, 0.02f, 0.1f); // Kolor skyboxa w nocy

    glm::vec3 baseSkyColor = isDay ? daySkyColor : nightSkyColor;

    fogDensity = 2,5 * fogDensity;
    glm::vec3 finalColor = baseSkyColor;
    if (fogEnabled) {
        float fogFactor = 1.0f - exp(-fogDensity * 10.0f); 
        finalColor = glm::mix(baseSkyColor, fogColor, fogFactor);
    }
    createSolidColorSkybox(finalColor);
}
