using System.IO;
using UnityEngine;

namespace ConstellationUnityEditor
{
    [System.Serializable]
    public static class ConstellationEditor
    {
        const string packagePath = "Packages/com.unity.constellation-unity";
        const string assetsPath = "Assets/";
        const string constellationDataFolder = "ConstellationData";
        const string editorAssetsPath = "Editor/EditorAssets";
        const string editorDataPath = "Editor/EditorData";

        public static string GetPackagePath()
        {
            return packagePath;
        }

        public static string GetEditorAssetsPath()
        {
            return GetPackagePath() + "/" + editorAssetsPath;
        }

        public static string GetEditorConfigPath()
        {
            return GetPackagePath() + "/" + editorDataPath;
        }

        public static string GetEditorDataFolderPath()
        {
            var path = Directory.Exists(Application.dataPath + constellationDataFolder) ? assetsPath + constellationDataFolder : InitializeEditorPath();
            return path;
        }

        public static string GetEditorDataPath()
        {
            return GetEditorDataFolderPath() + "/" + constellationDataFolder + "/";
        }

        public static string GetProjectPath()
        {
            var directory = string.IsNullOrEmpty(assetsPath) ? InitializeEditorPath() : assetsPath;
            return directory;
        }

        private static string InitializeEditorPath()
        {
            var editorDirectory = Directory.CreateDirectory(Application.dataPath + "/" + constellationDataFolder);
            return assetsPath + "/" + constellationDataFolder;
        }
    }
}
