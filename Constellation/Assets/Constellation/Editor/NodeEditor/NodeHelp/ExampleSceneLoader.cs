using Constellation;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConstellationEditor {
    public class ExampleSceneLoader {
        public ExampleSceneLoader () { }

        public void RunExample (string name, ConstellationEditorDataService constellationEditorDataService) {
            SceneManager.CreateScene ("Example");
            UnloadAllScenesExcept ("Example");

            GameObject light = new GameObject ("Light");
            light.AddComponent<Light> ().type = LightType.Directional;

            GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
            cube.transform.position = new Vector3 (0, 0, 0);
            cube.gameObject.SetActive (false);
            var behaviour = cube.AddComponent<ConstellationBehaviour> () as ConstellationBehaviour;
            var exampleConstellation = constellationEditorDataService.GetConstellationByName (name);
            if (exampleConstellation == null) {
                EditorUtility.DisplayDialog ("Oops...",
                    "The example you are trying to open does not exist... If you need more info on " + name + ", you can still double right click on either input or outputs.",
                    "Go back");
                EditorApplication.isPlaying = false;
                return;
            }
            behaviour.ConstellationData = exampleConstellation;
            Selection.activeGameObject = cube;
            cube.gameObject.SetActive (true);

            GameObject camera = new GameObject ("Camera");
            camera.transform.position = new Vector3 (0, 0, -10);
            camera.AddComponent<Camera> ();

        }

        void UnloadAllScenesExcept (string sceneName) {
            int c = SceneManager.sceneCount;
            Scene[] scenesIdToRemove = new Scene[c - 1];
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