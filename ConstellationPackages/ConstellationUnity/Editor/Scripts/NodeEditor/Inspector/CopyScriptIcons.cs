using Constellation;
using UnityEditor;
using UnityEngine;

// makes sure that the static constructor is always called in the editor.
namespace ConstellationUnityEditor
{
    [InitializeOnLoad]
    public class CopyScriptIcons : Editor
    {
        private static bool isDraggable;
        static CopyScriptIcons()
        {
            Copy();
        }

        public static void Copy()
        {
        
            var source = (Texture2D)AssetDatabase.LoadAssetAtPath(ConstellationEditor.GetEditorDataFolderPath() + "ConstellationScript.png", typeof(Texture2D));
            var target = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/ConstellationBehaviourScript Icon.png", typeof(Texture2D));
            if (source != null && target == null)
            {
                AssetDatabase.CreateFolder("Assets", "Gizmos");
                AssetDatabase.CopyAsset(ConstellationEditor.GetEditorDataFolderPath() + "ConstellationScript.png", "Assets/Gizmos/ConstellationBehaviourScript Icon.png");
            }
        }
    }
}