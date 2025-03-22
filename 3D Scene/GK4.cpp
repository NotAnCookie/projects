

#include <glad/glad.h>        
#include <GLFW/glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include "Shader.h"
#include "Camera.h"
#include "Object.h"
#include "Renderer.h"
#include "ObjectData.h" 
#include "Light.h"
#include "Scene.h"
#include "skybox.h"
#include "SkyTexture.h"
#include "Mirror.h"

void processLightInput(GLFWwindow* window, Light& spotlight, float deltaTime) {
    glm::vec3 currentDirection = spotlight.getDirection();
    float angleSpeed = 3.0f * deltaTime;

    if (glfwGetKey(window, GLFW_KEY_6) == GLFW_PRESS) {
        currentDirection.y += angleSpeed;  // Kierowanie w górê
    }
    if (glfwGetKey(window, GLFW_KEY_Y) == GLFW_PRESS) {
        currentDirection.y -= angleSpeed;  // Kierowanie w dó³
    }
    if (glfwGetKey(window, GLFW_KEY_T) == GLFW_PRESS) {
        currentDirection.x -= angleSpeed;  // Kierowanie w lewo
    }
    if (glfwGetKey(window, GLFW_KEY_U) == GLFW_PRESS) {
        currentDirection.x += angleSpeed;  // Kierowanie w prawo
    }

    spotlight.setDirection(glm::normalize(currentDirection));
}

void updateSpotlight(Light& spotlight, const Object& object) {
    glm::vec3 objectPosition = object._position;

    // Offset dla œwiat³a, aby by³o nad lub za obiektem (mo¿esz to dostosowaæ do swoich potrzeb)
    glm::vec3 spotlightOffset(0.0f, 0.0f, 0.0f);  // Œwiat³o za obiektem, nad nim mo¿na zmieniæ wysokoœæ


    float angle = object._rotation;  // K¹t obrotu obiektu w radianach

    // Obliczamy kierunek patrzenia œwiat³a na podstawie rotacji obiektu (wzglêdem osi Y)
    glm::vec3 objectDirection = glm::vec3(
        sin(angle),  // Kierunek w osi X
        0.0f,        // Bez zmiany w osi Y
        -cos(angle)  // Kierunek w osi Z (odwrócony, bo domyœlnie patrzymy w stronê -Z)
    );

    spotlight.setPosition(objectPosition + spotlightOffset);

    // Ustawiamy kierunek œwiat³a, aby pod¹¿a³ za obiektem
    spotlight.setDirection(objectDirection);
}


void updateCamera(Camera& camera, const Object& object) {
    glm::vec3 objectPosition = object._position;

    float angle = object._rotation;
    glm::vec3 objectDirection = glm::vec3(
        sin(angle), 0.0f, -cos(angle) 
    );

    glm::vec3 cameraPosition = objectPosition + objectDirection * -1.0f + glm::vec3(0.0f, 1.0f, 0.0f); 

    camera.setPosition(cameraPosition);

    camera.front = objectDirection;
    camera.updateViewMatrix(); 
}

void updateStaticCamera(Camera& camera, const Object& object) {
    glm::vec3 objectPosition = object._position; 
    glm::vec3 cameraPosition = camera.getPosition();

    camera.front = (objectPosition - cameraPosition);  
    camera.updateViewMatrix();
}



bool processInput(GLFWwindow* window, Object& object, float deltaTime) {
    glm::vec3 direction(0.0f);
    bool did_move = false;
    float speed = 1.0f * deltaTime;
    float angle = object._rotation;  // K¹t obrotu obiektu w radianach

    // Obliczamy kierunek patrzenia obiektu (wektor) przy u¿yciu trygonometrii
    glm::vec3 direction_2 = glm::vec3(
        sin(angle), 0.0f, -cos(angle)  // Obliczamy kierunek na podstawie k¹ta obrotu
    );

    if (glfwGetKey(window, GLFW_KEY_I) == GLFW_PRESS) {
        //direction.z -= speed;  // Poruszanie do przodu
        object.moveForward(speed);
        did_move = true;
    }
    if (glfwGetKey(window, GLFW_KEY_K) == GLFW_PRESS) {
        //direction.z += speed;  // Poruszanie do ty³u
        object.moveBackward(speed);
        did_move = true;
    }
    if (glfwGetKey(window, GLFW_KEY_J) == GLFW_PRESS) {
        //direction.x -= speed;  // Poruszanie w lewo
        object.turn(-0.1);
        did_move = true;
    }
    if (glfwGetKey(window, GLFW_KEY_L) == GLFW_PRESS) {
        //direction.x += speed;  // Poruszanie w prawo
        object.turn(0.1);
        did_move = true;
    }

    glm::vec3 currentPosition = object._position; // Zak³adamy, ¿e taka metoda istnieje
    object.setPosition(currentPosition.x + direction.x, currentPosition.y + direction.y, currentPosition.z + direction.z);
    return did_move;
}





void sendLightsToShader(GLuint shaderProgram, const std::vector<Light>& lights) {
    glUniform1i(glGetUniformLocation(shaderProgram, "numLights"), lights.size());

    for (size_t i = 0; i < lights.size(); i++) {
        lights[i].applyToShader(shaderProgram, "lights[" + std::to_string(i) + "]");
    }
}

void updateLighting(bool isDay, GLuint shaderProgram) {
    if (isDay) {
        // Dzieñ: Œwiat³o s³oneczne i ambientowe
        glUniform3f(glGetUniformLocation(shaderProgram, "ambientLight"), 0.3f, 0.3f, 0.3f); // Rozproszone œwiat³o dzienne
        glUniform3f(glGetUniformLocation(shaderProgram, "lights[0].color"), 1.0f, 1.0f, 0.8f); // Jasne œwiat³o s³oneczne
        glUniform3f(glGetUniformLocation(shaderProgram, "lights[0].direction"), 0.0f, -1.0f, 0.0f); // Kierunek s³oñca
        glUniform1i(glGetUniformLocation(shaderProgram, "lights[0].type"), 1); // Œwiat³o kierunkowe
    }
    else {
        // Noc: Ksiê¿yc i s³abe ambientowe
        glUniform3f(glGetUniformLocation(shaderProgram, "ambientLight"), 0.05f, 0.05f, 0.1f); // Delikatne œwiat³o nocne
        glUniform3f(glGetUniformLocation(shaderProgram, "lights[0].color"), 0.1f, 0.1f, 0.9f); // S³absze, zimne œwiat³o
        glUniform3f(glGetUniformLocation(shaderProgram, "lights[0].direction"), 0.0f, -0.8f, 0.2f); // Kierunek œwiat³a ksiê¿yca
        glUniform1i(glGetUniformLocation(shaderProgram, "lights[0].type"), 1); // Nadal œwiat³o kierunkowe
    }
}

void updateFogSettings(bool enableFog, float density, glm::vec3 color, GLuint shaderProgram) {
    glUniform1i(glGetUniformLocation(shaderProgram, "fogEnabled"), enableFog);
    glUniform1f(glGetUniformLocation(shaderProgram, "fogDensity"), density);
    glUniform3fv(glGetUniformLocation(shaderProgram, "fogColor"), 1, glm::value_ptr(color));
}

void renderSkybox(GLuint skyboxShader, GLuint skyboxVAO, float currentFogDensity, glm::vec3 fogColor) {
    glUseProgram(skyboxShader);
    glUniform1i(glGetUniformLocation(skyboxShader, "fogEnabled"), true);
    glUniform1f(glGetUniformLocation(skyboxShader, "fogDensity"), currentFogDensity);
    glUniform3fv(glGetUniformLocation(skyboxShader, "fogColor"), 1, glm::value_ptr(fogColor));
    glUniform1i(glGetUniformLocation(skyboxShader, "isSkybox"), true);

    glBindVertexArray(skyboxVAO);
    glDrawArrays(GL_TRIANGLES, 0, 36);
    glBindVertexArray(0);

    glUniform1i(glGetUniformLocation(skyboxShader, "isSkybox"), false); // Przywróæ domyœlny stan
}

void moveLightInCircle(Light& light, float radius, float speed, float time) {
    glm::vec3 dir = light.getDirection();

    light.setDirection(dir + glm::vec3(
        radius * cos(speed * time),   // X
        0.0f,                         // Y (mo¿esz zmieniæ jeœli chcesz, ¿eby zmienia³a siê wysokoœæ)
        radius * sin(speed * time)    // Z
    ));
}




int main() {
    // Inicjalizacja GLFW
    if (!glfwInit()) {
        std::cerr << "GLFW initialization failed!" << std::endl;
        return -1;
    }

    // Tworzenie okna GLFW
    GLFWwindow* window = glfwCreateWindow(800, 600, "OpenGL Project", nullptr, nullptr);
    if (!window) {
        std::cerr << "GLFW window creation failed!" << std::endl;
        glfwTerminate();
        return -1;
    }

    // Ustawienie kontekstu OpenGL
    glfwMakeContextCurrent(window);

    // Inicjalizacja GLAD
    if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress)) {
        std::cerr << "GLAD initialization failed!" << std::endl;
        return -1;
    }

    // Tworzenie kamer
    Camera camera1(800, 600, glm::vec3(0.0f, 10.0f, 5.0f));
    Camera camera2(800, 600, glm::vec3(0.0f, 0.0f, 5.0f));
    Camera carCamera(800, 600, glm::vec3(0.0f, 1.0f, -2.0f)); // Kamera na samochodzie
    float globalTime = 0.0f;

    // Tworzenie obiektów
    //Object cube(cubeVertices);

    std::vector<Vertex> wallVert = scaleHorizontal(wallVertices, 10);
    wallVert = scaleVertical(wallVert, 10);
    //Object wall(wallVertices, wallIndices);
    Object wall(wallVert, wallIndices);
    wall.setPosition(0.0f, 0.0f, -4.1f);



    std::vector<Vertex> sphereVertices;
    std::vector<GLuint> sphereIndices;

    // Generowanie kuli
    generateSphere(1.0f, 36, 18, sphereVertices, sphereIndices);

    Object sphere1(sphereVertices, sphereIndices);
    sphere1.setPosition(-1.5f, 0.0f, 0.0f);

    Object sphere2(sphereVertices, sphereIndices);
    sphere2.setPosition(1.5f, 0.0f, 0.0f);

    Scene scene;


    // Shader
    Shader shader("shaders/vertex_shader.glsl", "shaders/fragment_shader.glsl");

    // Renderer
    Renderer renderer;


    Shader shaderSkybox("shaders/skybox.vert.glsl", "shaders/skybox.frag.glsl");
    Skybox skybox;

    std::vector<std::string> faces = nightSkyboxFaces;

    skybox.loadSkyboxTextures(faces);

    Shader mirrorShader("shaders/mirror_vertex.glsl", "shaders/mirror_fragment.glsl");
    mirrorShader.use();



    // Ustawienia OpenGL
    glEnable(GL_DEPTH_TEST);

    // Flaga do prze³¹czania kamer
    bool useCamera1 = true;

    // Zmienna do liczenia czasu pomiêdzy klatkami
    float lastFrame = 0.0f;
    float deltaTime = 0.0f;




    shader.use();

    // Sprawdzanie lokalizacji uniformów
    GLuint lightPosLoc = glGetUniformLocation(shader.getProgram(), "lightPos");
    //GLuint lightColorLoc = glGetUniformLocation(shader.getProgram(), "lightColor");
    GLuint viewPosLoc = glGetUniformLocation(shader.getProgram(), "viewPos");

    if (lightPosLoc == -1) {
        std::cerr << "Uniform 'lightPos' not found!" << std::endl;
    }
    else {
        std::cout << "Uniform 'lightPos' found!" << std::endl;
    }

    if (viewPosLoc == -1) {
        std::cerr << "Uniform 'viewPos' not found!" << std::endl;
    }
    else {
        std::cout << "Uniform 'viewPos' found!" << std::endl;
    }

    bool isDay = false;
    bool usePerspective = true;
    Light light(Light::LightType::POINT, glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(1.0f, 1.0f, 1.0f));
    glm::vec3 lightPos2 = light.getPosition();
    std::cerr << "light pos: "<<(int)lightPos2.x << std::endl;


    bool fogEnabled = false;
    float fogDensity = 0.02f;          // Gêstoœæ mg³y (im wiêksza, tym gêstsza)
    glm::vec3 fogColor = glm::vec3(0.7f, 0.7f, 0.7f); // Kolor mg³y

    glUniform1i(glGetUniformLocation(shader.getProgram(), "fogEnabled"), fogEnabled);
    glUniform3f(glGetUniformLocation(shader.getProgram(), "fogColor"), fogColor.r, fogColor.g, fogColor.b);
    glUniform1f(glGetUniformLocation(shader.getProgram(), "fogDensity"), fogDensity);



    int screenWidth = 800;
    int screenHeight = 600;
    glm::vec3 mirrorPosition(0.0f, 0.0f, -4.0f); // Pozycja lustra
    glm::vec2 mirrorSize(8.0f, 8.0f);           // Rozmiar lustra

    Mirror mirror(screenWidth, screenHeight, mirrorPosition, mirrorSize);





    std::vector<Light> lights;
    lights.push_back(Light(Light::LightType::POINT, glm::vec3(0.0f, -3.0f, 0.0f), glm::vec3(1.0f, 0.8f, 0.7f)));
    //lights.push_back(Light(Light::LightType::DIRECTIONAL, glm::vec3(1.0f), glm::vec3(1.0f, 1.0f, 1.0f), glm::vec3(-0.5f, -1.0f, -0.3f)));
    lights.push_back(Light(Light::LightType::SPOTLIGHT, glm::vec3(0.0f, 1.0f, 0.0f), glm::vec3(0.0f, 0.9f, 0.5f), glm::vec3(0.0f, -1.0f, 0.0f), glm::radians(15.0f), glm::radians(20.0f)));
    lights.push_back(Light(Light::LightType::DIRECTIONAL, glm::vec3(0.0f, 0.0f, 10.0f), glm::vec3(0.0f, 0.9f, 0.0f), glm::vec3(0.0f, -1.0f, 0.0f)));
    lights.push_back(Light(Light::LightType::DIRECTIONAL, glm::vec3(1.0f), glm::vec3(0.0f, 0.0f, 1.0f), glm::vec3(0.0f, 0.0f, -1.0f)));




    // Obiekt samochodu
    std::vector<Vertex> carVertices = sphereVertices; // Wczytaj siatkê samochodu
    std::vector<GLuint> carIndices = sphereIndices; // Indeksy dla samochodu
    //loadCarModel(carVertices, carIndices); // Funkcja wczytuj¹ca model
    Object car(carVertices, carIndices);
    car.setPosition(0.0f, -4.0f, 0.0f); // Ustaw pozycjê pocz¹tkow¹
    car.setRotation(0,0.0f, 0.0f, 0.0f);

    // Reflektory samochodu
    Light carHeadlightLeft(Light::LightType::SPOTLIGHT, glm::vec3(-0.5f, 0.2f, 0.8f), glm::vec3(1.0f, 0.0f, 0.8f), glm::vec3(0.0f, -0.1f, -1.0f), glm::radians(5.0f), glm::radians(10.0f));
    Light carHeadlightRight(Light::LightType::SPOTLIGHT, glm::vec3(0.5f, 0.2f, 0.8f), glm::vec3(1.0f, 0.0f, 0.8f), glm::vec3(0.0f, -0.1f, -1.0f), glm::radians(5.0f), glm::radians(10.0f));

    lights.push_back(carHeadlightLeft);
    lights.push_back(carHeadlightRight);

    // Flaga do prze³¹czania kamer
    int activeCamera = 1;
    Camera* currentCamera = &camera1;

    // Pêtla renderowania
    while (!glfwWindowShouldClose(window)) {
        // Obliczanie czasu miêdzy klatkami
        float currentFrame = glfwGetTime();
        deltaTime = currentFrame - lastFrame;
        lastFrame = currentFrame;
        globalTime = globalTime + deltaTime;

        // Sprawdzenie zdarzeñ
        glfwPollEvents();

        // Obs³uga prze³¹czania kamer
        if (glfwGetKey(window, GLFW_KEY_1) == GLFW_PRESS) activeCamera = 1;
        if (glfwGetKey(window, GLFW_KEY_2) == GLFW_PRESS) activeCamera = 2;
        if (glfwGetKey(window, GLFW_KEY_3) == GLFW_PRESS) activeCamera = 3;

        //Camera* currentCamera = nullptr;
        switch (activeCamera) {
        case 1: currentCamera = &camera1; break;
        case 2: currentCamera = &camera2; break;
        case 3:
            currentCamera = &carCamera;
            break;
        }

        // Aktualizacja pozycji reflektorów
        glm::vec3 carPosition = car._position;

        bool moved = processInput(window, car, deltaTime);
        updateCamera(carCamera, car);

        updateStaticCamera(camera1, car);

        if (moved) {
            updateSpotlight(carHeadlightLeft, car);
            updateSpotlight(carHeadlightRight, car);
        }
        /*updateSpotlight(carHeadlightLeft, car);
        updateSpotlight(carHeadlightRight, car);*/
        processLightInput(window, carHeadlightLeft, deltaTime);
        processLightInput(window, carHeadlightRight, deltaTime);
        lights[4] = carHeadlightLeft;
        lights[5] = carHeadlightRight;




        if (glfwGetKey(window, GLFW_KEY_P) == GLFW_PRESS) {
            usePerspective = true;
            std::cout << "Switched to perspective projection" << std::endl;
        }
        if (glfwGetKey(window, GLFW_KEY_O) == GLFW_PRESS) {
            usePerspective = false;
            std::cout << "Switched to orthographic projection" << std::endl;
        }

        if (glfwGetKey(window, GLFW_KEY_M) == GLFW_PRESS) { // Klawisz 'M' w³¹cza dzieñ
            if (!isDay) {
                isDay = true;
                updateLighting(isDay, shader.getProgram());
                //skybox.changeTexture(daySkyboxFaces);
                std::cout << "Switched to day" << std::endl;
            }
        }

        if (glfwGetKey(window, GLFW_KEY_N) == GLFW_PRESS) { // Klawisz 'N' w³¹cza noc
            if (isDay) {
                isDay = false;
                updateLighting(isDay, shader.getProgram());
                //skybox.changeTexture(nightSkyboxFaces);
                std::cout << "Switched to night" << std::endl;
            }
        }


        // Pêtla renderowania
        if (glfwGetKey(window, GLFW_KEY_F) == GLFW_PRESS) {
            fogEnabled = true;
            fogDensity = glm::min(fogDensity + 0.001f, 0.1f); // Powolne zwiêkszanie gêstoœci
            //skybox.createSolidColorSkybox(fogColor);
            glUniform1f(glGetUniformLocation(shader.getProgram(), "fogDensity"), fogDensity);
        }
        else if (glfwGetKey(window, GLFW_KEY_G) == GLFW_PRESS) {
            fogEnabled = true;
            fogDensity = glm::max(fogDensity - 0.001f, 0.0f); // Powolne zmniejszanie gêstoœci
            //skybox.createSolidColorSkybox(fogColor);
            glUniform1f(glGetUniformLocation(shader.getProgram(), "fogDensity"), fogDensity);
        }
        else if (glfwGetKey(window, GLFW_KEY_H) == GLFW_PRESS) {
            fogEnabled = false; // Wy³¹czenie mg³y
        }

        skybox.updateSkybox(isDay, fogEnabled, fogDensity, fogColor);

        glUniform1i(glGetUniformLocation(shader.getProgram(), "isSkybox"), false);
        glUniform1i(glGetUniformLocation(shader.getProgram(), "fogEnabled"), fogEnabled);
        

        // Aktualizacja w shaderze
        updateFogSettings(fogEnabled, fogDensity, fogColor, shader.getProgram());


   
        currentCamera->processKeyboard(window, deltaTime);
        currentCamera->updateViewMatrix();
        shader.use();
        //shaderSkybox.use();

        moveLightInCircle(lights[2], 10, 1, globalTime);
        sendLightsToShader(shader.getProgram(), lights);


        // Czyszczenie buforów
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // Ustawienie widoku i projekcji
        //shader.use();
        glm::mat4 view = currentCamera->getViewMatrix();
        glm::mat4 projection = currentCamera->getProjectionMatrix(usePerspective);
        glUniformMatrix4fv(glGetUniformLocation(shader.getProgram(), "view"), 1, GL_FALSE, glm::value_ptr(view));
        glUniformMatrix4fv(glGetUniformLocation(shader.getProgram(), "projection"), 1, GL_FALSE, glm::value_ptr(projection));

        glm::vec3 lightPos = light.getPosition();
        glm::vec3 lightColor = light.getColor();
        glm::vec3 viewPos = currentCamera->getPosition();  

        glUniform3fv(glGetUniformLocation(shader.getProgram(), "lightPos"), 1, glm::value_ptr(lightPos));
        glUniform3fv(glGetUniformLocation(shader.getProgram(), "lightPos3"), 1, glm::value_ptr(lightPos));
        //glUniform3fv(glGetUniformLocation(shader.getProgram(), "lightColor"), 1, glm::value_ptr(lightColor));
        glUniform3fv(glGetUniformLocation(shader.getProgram(), "viewPos"), 1, glm::value_ptr(viewPos));

        


        GLint lightPosLocation = glGetUniformLocation(shader.getProgram(), "lightPos3");
        glUniform3f(lightPosLocation, lightPos.x, lightPos.y, lightPos.z);
        if (lightPosLocation == -1) {
            std::cerr << "eeeeeeeeeeeeerrr loc"<< std::endl;
        }


        glEnable(GL_DEPTH_TEST);
        glEnable(GL_CULL_FACE);
        glCullFace(GL_BACK);

        glUseProgram(mirrorShader.getProgram());
        // Ustawienie parametrów mg³y (np. w g³ównym programie)
        glUniform1i(glGetUniformLocation(mirrorShader.getProgram(), "fogEnabled"), fogEnabled ? 1 : 0);
        glUniform3fv(glGetUniformLocation(mirrorShader.getProgram(), "fogColor"), 1, glm::value_ptr(fogColor));
        glUniform1f(glGetUniformLocation(mirrorShader.getProgram(), "fogDensity"), fogDensity);  // Wartoœæ zale¿na od potrzeb
        glm::vec3 cameraPosition = currentCamera->position;
        glUniform3fv(glGetUniformLocation(mirrorShader.getProgram(), "viewPos"), 1, glm::value_ptr(cameraPosition));

        

        mirror.renderReflection(*currentCamera, usePerspective, [&]() {
            scene.renderAll(renderer, shader, *currentCamera, usePerspective);
            }, [&]() {
                skybox.render(shaderSkybox, *currentCamera, usePerspective);
                }, shader, [&]() {
                    renderer.render(car,shader,*currentCamera,usePerspective);
                    });


        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        mirror.renderMirror(*currentCamera, usePerspective, mirrorShader.getProgram());




        skybox.render(shaderSkybox, *currentCamera, usePerspective);

        scene.renderAll(renderer, shader, *currentCamera,usePerspective);


        renderer.render(wall, shader, *currentCamera,usePerspective);
  

        renderer.render(car, shader, *currentCamera, usePerspective);


        glfwSwapBuffers(window);
    }

    glfwTerminate();
    return 0;
}


