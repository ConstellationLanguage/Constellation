using Constellation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConstellationEditor {
    public class ExampleSceneLoader {
        public ExampleSceneLoader () { }

        public void RunExample (string name, ConstellationEditorDataService constellationEditorDataService) {
            var newScene = SceneManager.CreateScene ("Example");
            UnloadAllScenesExcept("Example");
            GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
            cube.transform.position = new Vector3 (0, 0, 0);
            cube.gameObject.SetActive(false);
            var behaviour = cube.AddComponent<ConstellationBehaviour> () as ConstellationBehaviour;
            behaviour.ConstellationData = constellationEditorDataService.GetConstellationByName (name);
            Selection.activeGameObject = cube;
            cube.gameObject.SetActive(true);
            
            GameObject camera = new GameObject("Camera");
            camera.transform.position = new Vector3(0,0,-10);
            camera.AddComponent<Camera>();

            GameObject light = new GameObject("Light");
            light.AddComponent<Light>().type = LightType.Directional;
        }

        void UnloadAllScenesExcept (string sceneName) {
            int c = SceneManager.sceneCount;
            Scene [] scenesIdToRemove = new Scene[c - 1];
            for (int i = 0; i < c; i++) {
                Scene scene = SceneManager.GetSceneAt (i);
                if (scene.name != sceneName) {
                    scenesIdToRemove[i] = scene;
                }
            }

            foreach (var scene in scenesIdToRemove) {
                 SceneManager.UnloadScene (scene);
            }
        }
    }
}