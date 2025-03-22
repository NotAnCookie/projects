#version 460 core

in vec3 TexCoords;

out vec4 FragColor;

uniform samplerCube skybox;

void main() {
    // Pobranie koloru z tekstury cubemap
    FragColor = texture(skybox, TexCoords);
}