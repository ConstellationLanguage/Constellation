using UnityEngine;
using UnityEditor;
using Constellation;

namespace ConstellationEditor {
    [CustomEditor(typeof(ConstellationScript))]
    public class ConstellationScriptInspector : Editor {
        public override void OnInspectorGUI() {
            if (GUILayout.Button("Open in Node Editor")) {
                ConstellationUnityWindow.WindowInstance.Open(AssetDatabase.GetAssetPath(target));
            }

            DrawDefaultInspector();
        }
    }
}