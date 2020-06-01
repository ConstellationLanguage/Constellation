using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public static class TopBarPanel {
        public static bool Draw (ILoadable loadable, ICopyable copyable, IParsable compilable) {
            try
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("File", EditorStyles.toolbarButton, GUILayout.Width(35)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("New "), false, OnNew, loadable);
                    menu.AddItem(new GUIContent("Load"), false, OnLoad, loadable);
                    menu.AddItem(new GUIContent("Save"), false, OnSave, loadable);
                    menu.AddItem(new GUIContent("Export as constellation file"), false, OnExportAsCL, loadable);
                    menu.ShowAsContext();
                    return true;
                }

                if (GUILayout.Button("Edit", EditorStyles.toolbarButton, GUILayout.Width(35)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Copy"), false, Copy, copyable);
                    menu.AddItem(new GUIContent("Past"), false, Paste, copyable);
                    menu.ShowAsContext();
                    return true;
                }

                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
                {
                    compilable.ParseScript(true);
                    return true;
                }

                GUILayout.Label("", EditorStyles.toolbarButton);
                EditorGUILayout.EndHorizontal();
            }catch
            {
            }
            return false;
        }

        static void Copy (object copyable) {
            var icopyable = copyable as ICopyable;
            icopyable.Copy ();
        }

        static void Cut (object copyable) {
            var icopyable = copyable as ICopyable;
            icopyable.Cut ();
        }

        static void Paste (object copyable) {
            var icopyable = copyable as ICopyable;
            icopyable.Paste ();
        }

        static void OnNew (object loadable) {
            var iloadable = loadable as ILoadable;
            iloadable.New ();
        }

        static void OnSave (object loadable) {
            var iloadable = loadable as ILoadable;
            iloadable.Save ();
        }

        static void OnExportAsCL(object loadable)
        {
            var iloadable = loadable as ILoadable;
            iloadable.Export ();
        }

        static void OnLoad (object loadable) {
            var iloadable = loadable as ILoadable;
            iloadable.Open (null);
        }
    }
}