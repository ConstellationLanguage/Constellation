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

        private static bool dragging = false;
        public static float VerticalSplit(Rect _rect) {
            var color = GUI.backgroundColor;
            var isMoving = false;
            GUI.backgroundColor = dragging ? new Color(0.173f, 0.169f, 0.173f) : new Color(0.635f, 0.635f, 0.635f);
            EditorGUILayout.BeginVertical(GUILayout.Width(_rect.width));
            EditorGUILayout.BeginHorizontal();
            GUI.Box(_rect, "");

            if (dragging)
                EditorGUIUtility.AddCursorRect(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)), MouseCursor.ResizeHorizontal);
            else
                EditorGUIUtility.AddCursorRect(_rect, MouseCursor.ResizeHorizontal);

            if (EventUtils.MouseButtonDown(Event.current, 0))
                if (_rect.Contains(Event.current.mousePosition))
                    dragging = true;
            if (dragging) {
                if (EventUtils.MouseButtonDrag(Event.current, 0))
                    isMoving = true;
                if (EventUtils.MouseButtonUp(Event.current, 0))
                    dragging = false;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = color;

            if(isMoving)
                return _rect.x - Event.current.delta.x;

            //_rect.x should be left side width.
            //Screen.width - (_rect.x + _rect.width) should be right side width
            return _rect.x; 
        }
    }
}