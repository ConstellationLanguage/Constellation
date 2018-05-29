using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor
{
    [CustomEditor(typeof(ConstellationScript))]
    public class ConstellationScriptInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open in Node Editor"))
            {
                if (ConstellationUnityWindow.WindowInstance == null)
                    ConstellationUnityWindow.ShowWindow();
                ConstellationUnityWindow.WindowInstance.Open(AssetDatabase.GetAssetPath(target));
            }
            base.OnInspectorGUI();
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
        {
            Texture2D newIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Constellation/EditorAssets/ConstellationScript.png", typeof(Texture2D));
            return newIcon;
        }
    }
}