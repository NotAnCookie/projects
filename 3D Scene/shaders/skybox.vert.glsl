#version 460 core

layout (location = 0) in vec3 aPos;

out vec3 TexCoords;

uniform mat4 view;
uniform mat4 projection;

void main() {
    // Usunięcie translacji z macierzy widoku (ważne dla skyboxa)
    mat4 viewNoTranslation = view;//mat4(mat3(view));
    gl_Position = projection * viewNoTranslation * vec4(aPos, 1.0);
    TexCoords = aPos;
}