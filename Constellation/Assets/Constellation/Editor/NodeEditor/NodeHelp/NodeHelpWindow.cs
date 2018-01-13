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
        }

        protected override void DrawStartGUI () {
            if (!hasTriedFinddingExample) {
                hasTriedFinddingExample = true;
                scriptDataService = new ConstellationEditorDataService ();
                scriptDataService.OpenConstellation (Application.dataPath + "/Constellation/Examples/Nodes/" + helpName + ".asset", false);
                Background = AssetDatabase.LoadAssetAtPath (editorPath + "background.png", typeof (Texture2D)) as Texture2D;
                Setup ();
            } else {
                if (Background != null)
                    for (var i = 0; i < 50; i++) {
                        for (var j = 0; j < 25; j++) {
                            Rect texRect = new Rect (i * Background.width,
                                j * Background.height,
                                Background.width, Background.height);
                            GUI.DrawTexture (texRect, Background);
                        }
                    }
            }
        }
    }
}