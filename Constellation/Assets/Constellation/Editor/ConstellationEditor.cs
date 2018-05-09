using System.IO;
using UnityEngine;

namespace ConstellationEditor {
    public static class ConstellationEditor {
        private static string editorPath = "";

        public static string GetEditorPath () {
            return string.IsNullOrEmpty(editorPath) ? InitializeEditorPath() : editorPath;
        }

        public static string GetEditorAssetPath () {
            var path = string.IsNullOrEmpty(editorPath) ? InitializeEditorPath() : editorPath;
            return path + "EditorAssets/";
        }

        private static string InitializeEditorPath () {
            foreach (var directory in Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories))
                if (directory.EndsWith("Constellation\\Editor"))
                    foreach (var file in Directory.GetFiles(directory))
                        if (file.Replace(directory + "\\", "").Equals("ConstellationEditor.cs"))
                            return editorPath = directory.Replace(Application.dataPath, "Assets").Replace('\\', '/') + "/";
            Debug.Log("Error finding Constellation Editor folder");                
            return null;
        }
    }
}
