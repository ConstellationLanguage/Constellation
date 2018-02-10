using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeHelpWindow : ConstellationUnityWindow {
        private static string helpName;
        private static bool hasTriedFinddingExample;
        private Texture2D Background;
        private const string editorPath = "Assets/Constellation/Editor/EditorAssets/";
        PlayBar playBar;

        [MenuItem ("Window/Constellation Helper")]
        public static void ShowHelpWindow (string help = "") {
            hasTriedFinddingExample = false;
            helpName = help;
            EditorWindow.GetWindow (typeof (NodeHelpWindow), false, "Constellation Help");
        }

        protected override void DrawGUI () {
            GUILayout.BeginVertical ();
            if (playBar == null)
                playBar = new PlayBar ();

            playBar.Draw ();
            nodeEditorPanel.DrawNodeEditor (position.width, position.height - 20);
            GUILayout.EndVertical ();

            RepaintIfRequested();
        }

        protected override void DrawStartGUI () {
            wantsMouseMove = true;
            if (!hasTriedFinddingExample) {
                hasTriedFinddingExample = true;
                scriptDataService = new ConstellationEditorDataService ();
                scriptDataService.OpenConstellation (Application.dataPath + "/Constellation/Examples/Nodes/" + helpName + ".asset", false);
                Background = AssetDatabase.LoadAssetAtPath (editorPath + "background.png", typeof (Texture2D)) as Texture2D;
                Setup ();
            } else {
                DrawBackgroundGrid(Screen.width, Screen.height);
            }
        }

        private void DrawBackgroundGrid(float _width, float _height)
        {
            if (Background != null)
            {
                //Background location based of current location allowing unlimited background
                //How many background are needed to fill the background
                var xCount = Mathf.Round(_width / Background.width) + 2;
                var yCount = Mathf.Round(_height / Background.height) + 2;

                var texRect = new Rect(0, 0, Background.width, Background.height);

                for (var i = 0; i < xCount; i++) {
                    for (var j = 0; j < yCount; j++) {
                        texRect.x = i * Background.width;
                        texRect.y = j * Background.height;
                        GUI.DrawTexture(texRect, Background);
                    }
                }
            }
        }
    }
}