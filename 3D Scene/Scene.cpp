#include "Scene.h"


Scene::Scene() {
	std::vector<Vertex> sphereVertices;
	std::vector<GLuint> sphereIndices;
	// Generowanie kuli
	generateSphere(0.2f, 36, 18, sphereVertices, sphereIndices);

	// Inicjalizacja generatora liczb losowych
	srand(static_cast<unsigned int>(time(0)));

	// Wygenerowanie kul o ró¿nych rozmiarach i pozycjach
	for (int i = 0; i < 10; ++i) {
		// Losowanie promienia kuli (od 0.5 do 2.0)
		float radius = 0.5f + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX) / 1.5f);
		float xPos = -5.0f + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX) / 10.0f);
		float zPos = -5.0f + static_cast<float>(rand()) / (static_cast<float>(RAND_MAX) / 10.0f);

		float yPos = -5.0f + radius;  
		generateSphere(radius, 36, 18, sphereVertices, sphereIndices);
		Object sphere(sphereVertices, sphereIndices);
		sphere.setPosition(xPos, yPos, zPos); 

		objectList.push_back(sphere);
	}


	// œciany
	std::vector<Vertex> cubeVert = scaleHorizontal(cubeVertices, 20);
	cubeVert = scaleVertical(cubeVert, 10);
	cubeVert = rotateWall(cubeVert, 90, glm::vec3(0, 1, 0));

	Object cube(cubeVert,cubeIndices);
	cube.setPosition(-5.0f, 0.0f, 0.0f);
	objectList.push_back(cube);

	Object cube2(cubeVert, cubeIndices);
	cube2.setPosition(5.0f, 0.0f, 0.0f);
	objectList.push_back(cube2);

	cubeVert = rotateWall(cubeVert, 90, glm::vec3(0, 0, 1));
	Object cube3(cubeVert, cubeIndices);
	cube3.setPosition(0.0f, -5.0f, 0.0f);
	objectList.push_back(cube3);

}

void Scene::renderAll(Renderer& renderer, Shader& shader, Camera& camera, bool usePerspective) {
	for (auto& obj : objectList) {
		renderer.render(obj, shader, camera, usePerspective);
	}
}

