using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public static class TopBarPanel {
        public static bool Draw (ILoadable loadable, IUndoable undoable, ICopyable copyable, ICompilable compilable) {
            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("File", EditorStyles.toolbarButton, GUILayout.Width (35))) {
                GenericMenu menu = new GenericMenu ();
                menu.AddItem (new GUIContent ("New: Ctrl+Alt+N "), false, OnNew, loadable);
                menu.AddItem (new GUIContent ("Load: Ctrl+Alt+L"), false, OnLoad, loadable);
                menu.AddItem (new GUIContent ("Save: Ctrl+Alt+S"), false, OnSave, loadable);
                menu.ShowAsContext ();
                return true;
            }

            if (GUILayout.Button ("Edit", EditorStyles.toolbarButton, GUILayout.Width (35))) {
                GenericMenu menu = new GenericMenu ();
                menu.AddItem (new GUIContent ("Undo: Ctrl+Alt+Z"), false, OnUndo, undoable);
                menu.AddItem (new GUIContent ("Redo: Ctrl+Alt+Y"), false, OnRedo, undoable);
                menu.AddItem (new GUIContent ("Copy: Ctrl+Alt+C"), false, Copy, copyable);
                menu.AddItem (new GUIContent ("Past: Ctrl+Alt+V"), false, Paste, copyable);
                menu.ShowAsContext ();
                return true;
            }

            if (GUILayout.Button ("Refresh", EditorStyles.toolbarButton, GUILayout.Width (70))) {
                compilable.CompileScripts();
                return true;
            }
            
            GUILayout.Label ("", EditorStyles.toolbarButton);
            EditorGUILayout.EndHorizontal ();
            return false;
        }

        static void OnUndo (object undoable) {
            var iundoable = undoable as IUndoable;
            iundoable.Undo ();
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

        static void OnRedo (object undoable) {
            var iundoable = undoable as IUndoable;
            iundoable.Redo ();
        }

        static void OnNew (object loadable) {
            var iloadable = loadable as ILoadable;
            iloadable.New ();
        }

        static void OnSave (object loadable) {
            var iloadable = loadable as ILoadable;
            iloadable.Save ();
        }

        static void OnLoad (object loadable) {
            var iloadable = loadable as ILoadable;
            iloadable.Open ("");
        }
    }
}