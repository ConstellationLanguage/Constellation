using UnityEngine;
using UnityEditor;

namespace ConstellationEditor
{
    public class NodeHelpWindow : ConstellationUnityWindow
    {
        private static string helpName;
        private static bool hasTriedFinddingExample;
        [MenuItem("Window/Constellation Helper")]
        public static void ShowHelpWindow(string help = "")
        {
            hasTriedFinddingExample = false;
            helpName = help;
            EditorWindow.GetWindow(typeof(NodeHelpWindow), false, "Constellation Help");
        }

        protected override void DrawGUI()
        {
            nodeEditorPanel.DrawNodeEditor(position.width, position.height);
        }


        protected override void DrawStartGUI()
        {
            if (!hasTriedFinddingExample) {
                hasTriedFinddingExample = true;
                scriptDataService = new ConstellationEditorDataService();
                scriptDataService.OpenConstellation(Application.dataPath + "/Plugins/Constellation/Examples/" + helpName + ".asset", false);
                Setup();
            } else {

            }
        }
    }
}
