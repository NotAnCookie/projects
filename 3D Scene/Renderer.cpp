#include "Renderer.h"
#include <iostream>

void Renderer::render(Object& object, Shader& shader, Camera& camera, bool usePerspective) {
    shader.use();

    glm::mat4 view = camera.getViewMatrix();
    glm::mat4 projection = camera.getProjectionMatrix(usePerspective);

    glUniformMatrix4fv(glGetUniformLocation(shader.programID, "view"), 1, GL_FALSE, glm::value_ptr(view));
    glUniformMatrix4fv(glGetUniformLocation(shader.programID, "projection"), 1, GL_FALSE, glm::value_ptr(projection));

    object.draw(shader.programID);
}

