#include "Mirror.h"

Mirror::Mirror(int screenWidth, int screenHeight, glm::vec3 position, glm::vec2 size)
    : screenWidth(screenWidth), screenHeight(screenHeight), position(position), size(size) {
    setupFramebuffer();
    setupGeometry();
}

Mirror::~Mirror() {
    // Zwolnienie zasobów
    glDeleteFramebuffers(1, &framebuffer);
    glDeleteTextures(1, &reflectionTexture);
    glDeleteRenderbuffers(1, &depthBuffer);
    glDeleteVertexArrays(1, &vao);
    glDeleteBuffers(1, &vbo);
    glDeleteBuffers(1, &ebo);
}

void Mirror::setupFramebuffer() {
    glGenFramebuffers(1, &framebuffer);
    glBindFramebuffer(GL_FRAMEBUFFER, framebuffer);

    // Tekstura odbicia
    glGenTextures(1, &reflectionTexture);
    glBindTexture(GL_TEXTURE_2D, reflectionTexture);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, screenWidth, screenHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, reflectionTexture, 0);

    // Bufor g³êbokoœci
    glGenRenderbuffers(1, &depthBuffer);
    glBindRenderbuffer(GL_RENDERBUFFER, depthBuffer);
    glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT, screenWidth, screenHeight);
    glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, depthBuffer);

    if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE) {
        std::cerr << "Framebuffer not complete!" << std::endl;
    }

    glBindFramebuffer(GL_FRAMEBUFFER, 0);
}


void Mirror::setupGeometry() {
    // Wierzcho³ki i indeksy prostok¹ta (lustra)
    float vertices[] = {
        // Pozycje           // Tekstury
        -size.x / 2, -size.y / 2, 0.0f, 0.0f, 0.0f,
         size.x / 2, -size.y / 2, 0.0f, 1.0f, 0.0f,
         size.x / 2,  size.y / 2, 0.0f, 1.0f, 1.0f,
        -size.x / 2,  size.y / 2, 0.0f, 0.0f, 1.0f
    };

    unsigned int indices[] = {
        0, 1, 2,
        2, 3, 0
    };

    // Tworzenie VAO, VBO i EBO
    glGenVertexArrays(1, &vao);
    glGenBuffers(1, &vbo);
    glGenBuffers(1, &ebo);

    glBindVertexArray(vao);

    glBindBuffer(GL_ARRAY_BUFFER, vbo);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

    // Atrybuty wierzcho³ków
    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);
    glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(3 * sizeof(float)));
    glEnableVertexAttribArray(1);

    glBindVertexArray(0);
}


void Mirror::renderReflection(Camera& camera, bool usePerspective,
    const std::function<void()>& renderScene,
    const std::function<void()>& renderSkybox, Shader& shader, const std::function<void()>& renderCar) {

    glBindFramebuffer(GL_FRAMEBUFFER, framebuffer);
    glViewport(0, 0, screenWidth, screenHeight);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    // ignorujemy y -> bo mamy tylko 1 punkt lustra
    glm::vec3 mirrorNormal(0.0f, 1.0f, 0.0f);  // Normalna p³aszczyzny (Y-up)
    glm::vec3 mirrorPoint = glm::vec3(position.x, 0.0f, position.z);  
    glm::vec3 cameraPos = glm::vec3(camera.getPosition().x, 0.0f, camera.getPosition().z);
    glm::vec3 cameraFront = camera.front;

    glm::vec3 incident = cameraPos - mirrorPoint;
    float length = glm::length(incident);
    glm::vec3 incident_save = incident;
    incident = glm::normalize(incident);

    // Obliczanie odbicia wektora wzglêdem p³aszczyzny
    glm::vec3 reflectedVector = -incident + 2.0f * glm::dot(incident, mirrorNormal) * mirrorNormal;
    glm::vec3 reflectedCameraPos = mirrorPoint + reflectedVector * length;

    //glm::vec3 reflectedCameraFront = glm::reflect(cameraFront, mirrorNormal);
    glm::vec3 reflectedCameraUp(0.0f, -1.0f, 0.0f);

    glm::mat4 reflectionView = glm::lookAt(
        reflectedCameraPos,                           // Odbita pozycja kamery
        reflectedCameraPos + 2.0f * incident_save,    // Punkt, na który kamera patrzy (odbity kierunek)
        reflectedCameraUp                             // Odbita oœ "up"
    );
    camera.setViewMatrix(reflectionView);

    if (renderSkybox) {
        renderSkybox();
    }

    if (renderScene) {
        renderScene();
    }

    renderCar();

    camera.resetViewMatrix();
    glBindFramebuffer(GL_FRAMEBUFFER, 0);
}












void Mirror::renderMirror(Camera& camera, bool usePerspective, GLuint shaderProgram) {
    glUseProgram(shaderProgram);

    glm::mat4 model = glm::translate(glm::mat4(1.0f), position);
    glm::mat4 view = camera.getViewMatrix();
    glm::mat4 projection = camera.getProjectionMatrix(usePerspective);

    glUniformMatrix4fv(glGetUniformLocation(shaderProgram, "view"), 1, GL_FALSE, &view[0][0]);
    glUniformMatrix4fv(glGetUniformLocation(shaderProgram, "projection"), 1, GL_FALSE, &projection[0][0]);
    glUniformMatrix4fv(glGetUniformLocation(shaderProgram, "model"), 1, GL_FALSE, &model[0][0]);

    // Ustawienie tekstury odbicia
    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, reflectionTexture);
    glUniform1i(glGetUniformLocation(shaderProgram, "reflectionTexture"), 0);

    // Renderowanie geometrii lustra
    glBindVertexArray(vao);
    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
    glBindVertexArray(0);
}

