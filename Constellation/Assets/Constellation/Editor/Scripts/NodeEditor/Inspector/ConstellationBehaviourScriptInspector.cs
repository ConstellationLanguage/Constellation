using Constellation.Unity3D;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor
{
    [CustomEditor(typeof(ConstellationBehaviourScript))]
    public class ConstellationBehaviourScriptInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Behaviour node");
            if (GUILayout.Button("Open in Node Editor"))
            {
                if (ConstellationEditorWindow.ConstellationEditorWindowInstance == null)
                    ConstellationEditorWindow.Init();
                ConstellationEditorWindow.ConstellationEditorWindowInstance.Open(new ConstellationScriptInfos(AssetDatabase.GetAssetPath(target), 
                    ConstellationScriptInfos.ConstellationScriptTag.NoTag,
                    false));
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