using System.IO;
using UnityEngine;

namespace ConstellationEditor {
    [System.Serializable]
    public static class ConstellationEditor {
        [SerializeField]
        private static string editorPath = "";

        public static string GetEditorPath () {
            return string.IsNullOrEmpty(editorPath) ? InitializeEditorPath() : editorPath;
        }

        public static string GetEditorAssetPath () {
            var path = string.IsNullOrEmpty(editorPath) ? InitializeEditorPath() : editorPath;
            return path.Replace("EditorData", "EditorAssets");
        }

        public static string GetProjectPath ()
        {
            var directory = string.IsNullOrEmpty(editorPath) ? InitializeEditorPath() : editorPath;
            directory = directory.Replace("/Editor/EditorData/", "/");
            return directory;
        } 

        private static string InitializeEditorPath () {
            foreach (var directory in Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories))
                if (directory.EndsWith("Constellation\\Editor\\Scripts"))
                    foreach (var file in Directory.GetFiles(directory))
                        if (file.Replace(directory + "\\", "").Equals("ConstellationEditor.cs"))
                            return editorPath = directory.Replace(Application.dataPath, "Assets").Replace('\\', '/').Replace("Scripts", "EditorData") + "/";
            Debug.Log("Error finding Constellation Editor folder");                
            return null;
        }
    }
}
