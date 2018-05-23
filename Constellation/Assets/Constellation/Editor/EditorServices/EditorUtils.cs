using System.IO;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public static class EditorUtils {

        public static T[] GetAllInstances<T> () where T : ScriptableObject {
            string[] guids = AssetDatabase.FindAssets ("t:" + typeof (T).Name); //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath (guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T> (path);
            }

            return a;
        }

        public static T GetInstanceByName<T> (string name) where T : ScriptableObject {
            string[] guids = AssetDatabase.FindAssets ("t:" + typeof (T).Name); //FindAssets uses tags check documentation for more info
            T a;
            for (int i = 0; i < guids.Length; i++) //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath (guids[i]);
                if (Path.GetFileNameWithoutExtension (path) == name) {
                    a = AssetDatabase.LoadAssetAtPath<T> (path);
                    return a;
                }
            }

            return null;
        }
    }
}