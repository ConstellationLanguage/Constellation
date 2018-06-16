using System;
using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class ConstellationBaseWindow : ExtendedEditorWindow, ILoadable {
        protected ConstellationEditorDataService scriptDataService;
        protected ConstellationCompiler ConstellationCompiler;
        static protected bool canDrawUI = false;
        protected ConstellationInstanceObject[] CurrentEditedInstancesName;
        protected GameObject previousSelectedGameObject;
        protected ConstellationEditable currentEditableConstellation;

        public void Awake () {
            Setup ();
            canDrawUI = false;
        }

        [MenuItem ("Help/Constellation tutorials")]
        static void Help () {
            Application.OpenURL ("https://github.com/ConstellationLanguage/Constellation/wiki");
        }

        protected virtual void Setup () { }

        public void New () {
            scriptDataService = new ConstellationEditorDataService ();
            scriptDataService.New ();
            Setup ();
        }

        public void Recover () {
            scriptDataService = new ConstellationEditorDataService ();
            ConstellationCompiler = new ConstellationCompiler ();
            if (scriptDataService.OpenEditorData ().LastOpenedConstellationPath == null)
                return;

            if (scriptDataService.OpenEditorData ().LastOpenedConstellationPath.Count != 0) {
                var scriptData = scriptDataService.Recover (scriptDataService.OpenEditorData ().LastOpenedConstellationPath[0]);
                if (scriptData != null) {
                    Setup ();
                    return;
                }
            }
        }

        public void ResetInstances () {
            scriptDataService.RessetInstancesPath ();
        }

        public void OpenConstellationInstance (Constellation.Constellation constellation, string path) {
            scriptDataService = new ConstellationEditorDataService ();
            scriptDataService.OpenConstellationInstance (constellation, path);
            CurrentEditedInstancesName = scriptDataService.currentInstancePath.ToArray ();
            Setup ();
        }

        public void Open (string _path = "") {
            scriptDataService = new ConstellationEditorDataService ();
            var script = scriptDataService.OpenConstellation (_path);
            if (script == null)
                return;
            Setup ();
            Repaint();
        }

        public void Save () {
            scriptDataService.Save ();
        }

        public void SaveInstance () {
            scriptDataService.SaveInstance ();
        }

        protected bool IsConstellationSelected () {
            if (scriptDataService != null) {
                if (scriptDataService.GetCurrentScript () != null)
                    return true;
                else
                    return false;
            } else
                return false;
        }

        protected void OnLinkAdded (LinkData link) {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentEditableConstellation.AddLink (link);
        }

        protected void OnLinkRemoved (LinkData link) {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentEditableConstellation.RemoveLink (link);
        }

        protected void OnNodeAdded (NodeData node) {
            if (Application.isPlaying && previousSelectedGameObject != null) {
                currentEditableConstellation.AddNode (node);
            }
            Repaint ();
        }

        protected void OnNodeRemoved (NodeData node) {
            if (Application.isPlaying && previousSelectedGameObject)
                currentEditableConstellation.RemoveNode (node);

            Repaint ();
        }

        protected virtual void ShowEditorWindow () { }

        protected virtual void ShowError (ConstellationError e = null, Exception exception = null) {
            var error = e.GetError ();
            if (error.IsIgnorable ()) {
                if (EditorUtility.DisplayDialog (error.GetErrorTitle () + " (" + error.GetID () + ") ", error.GetErrorMessage (), "Recover", "Ignore")) {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            } else {
                if (EditorUtility.DisplayDialog (error.GetErrorTitle () + " (" + error.GetID () + ") ", error.GetErrorMessage (), "Recover")) {
                    UnityEditor.EditorApplication.isPlaying = false;
                    scriptDataService.ResetConstellationEditorData ();
                    ShowEditorWindow ();
                }
            }

            if (exception != null && e != null)
                Debug.LogError (error.GetFormatedError () + exception.StackTrace);
            else if (e != null)
                Debug.LogError (error.GetFormatedError () + exception.StackTrace);
        }
    }
}