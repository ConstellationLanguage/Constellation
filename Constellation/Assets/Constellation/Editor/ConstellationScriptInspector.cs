using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    [CustomEditor(typeof(ConstellationScript))]
    public class ConstellationScriptInspector : Editor {
        public override void OnInspectorGUI () {
            if (GUILayout.Button("Open in Node Editor")) {
                if (ConstellationUnityWindow.WindowInstance == null)
                    ConstellationUnityWindow.ShowWindow();
                ConstellationUnityWindow.WindowInstance.Open(AssetDatabase.GetAssetPath(target));
            }
            base.OnInspectorGUI();
        }
    }
}