using System.Collections.Generic;
using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class ConstellationEditorDataService {
        private ConstellationScript Script;
        protected ConstellationEditorData EditorData;
        public List<string> currentPath;
        private bool isSaved;

        public ConstellationEditorDataService () {
            OpenEditorData ();
        }

        private ConstellationEditorData Setup () {
            var path = "Assets/Constellation/Editor/EditorData/EditorData.asset";
            EditorData = ScriptableObject.CreateInstance<ConstellationEditorData> ();
            AssetDatabase.CreateAsset (EditorData, path);
            return EditorData;
        }

        public void AddAction () {
            EditorData.EditorUndoService.AddAction (Script.script);
        }

        public void Undo () {
            var previous = EditorData.EditorUndoService.Undo ();
            if (previous == null)
                return;

            Script.Set (previous);
        }

        public void Redo () {
            var redo = EditorData.EditorUndoService.Redo ();
            if (redo == null)
                return;
            Script.Set (redo);
        }

        public void ClearActions () {
            EditorData.EditorUndoService.ClearActions ();
        }

        public ConstellationEditorData GetEditorData () {
            return EditorData;
        }

        public ConstellationEditorData OpenEditorData () {
            var path = "Assets/Constellation/Editor/EditorData/EditorData.asset";
            ConstellationEditorData t = (ConstellationEditorData) AssetDatabase.LoadAssetAtPath (path, typeof (ConstellationEditorData));
            if (t == null) {
                return Setup ();
            }

            EditorData = t;
            return EditorData;
        }

        public void OpenConstellationInstance (Constellation.Constellation constellation) {
            var constellationScript = ScriptableObject.CreateInstance<ConstellationScript> ();
            var path = "Assets/Constellation/Editor/EditorData/" + constellation.Name + "_Instance_.asset";
           

            if (path == null || path == "") {
                Script = null;
                return;
            }

            Script = constellationScript;
            AssetDatabase.CreateAsset (constellationScript, path);
            if (currentPath == null)
                currentPath = new List<string> (EditorData.LastOpenedConstellationPath.ToArray ());

            var nodes = constellation.GetNodes ();
            var links = constellation.GetLinks ();

            foreach (var node in nodes) {
                Script.AddNode (node);
            }

            currentPath.Insert (0, path);
            SaveEditorData ();

            foreach (var link in links) {
                Script.AddLink (link);
            }

            SaveEditorData ();
        }

        public bool RemoveOpenedConstellation (string name) {
            if (name == null)
                return false;
            EditorData.LastOpenedConstellationPath.Remove (name);
            currentPath.Remove (name);
            SaveEditorData ();
            return true;
        }

        private void SaveEditorData () {
            EditorData.LastOpenedConstellationPath = new List<string> ();
            foreach (var path in currentPath) {
                if (!EditorData.LastOpenedConstellationPath.Contains (path))
                    EditorData.LastOpenedConstellationPath.Add (path);
            }
            EditorUtility.SetDirty (EditorData);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }

        public void New () {
            Script = ScriptableObject.CreateInstance<ConstellationScript> ();
            var path = EditorUtility.SaveFilePanel ("Save Constellation", Application.dataPath, "NewConstellation" + ".asset", "asset");

            if (path.StartsWith (Application.dataPath)) {
                path = "Assets" + path.Substring (Application.dataPath.Length);
            }
            if (path == null || path == "") {
                Script = null;
                return;
            }
            AssetDatabase.CreateAsset (Script, path);
            if (currentPath == null)
                currentPath = new List<string> (EditorData.LastOpenedConstellationPath.ToArray ());

            currentPath.Insert (0, path);
            SaveEditorData ();
        }

        public ConstellationScript GetCurrentScript () {
            return Script;
        }

        public ConstellationScript OpenConstellation (string _path = "", bool save = true) {
            var path = "";
            if (_path == "")
                path = EditorUtility.OpenFilePanel ("Load constellation", Application.dataPath, "asset");
            else
                path = _path;

            if (path.StartsWith (Application.dataPath)) {
                path = "Assets" + path.Substring (Application.dataPath.Length);
            }
            ConstellationScript t = (ConstellationScript) AssetDatabase.LoadAssetAtPath (path, typeof (ConstellationScript));

            Script = t;

            currentPath = new List<string> (EditorData.LastOpenedConstellationPath);
            if (!currentPath.Contains (path))
                currentPath.Insert (0, path);
            else {
                currentPath.Remove (path);
                currentPath.Insert (0, path);
            }

            if (save)
                SaveEditorData ();

            return t;
        }

        public ConstellationScript Recover (string _path) {
            ConstellationScript t = (ConstellationScript) AssetDatabase.LoadAssetAtPath (_path, typeof (ConstellationScript));
            Script = t;
            if (t != null) {
                currentPath = new List<string> (EditorData.LastOpenedConstellationPath);
                currentPath.Remove (_path);
                currentPath.Insert (0, _path);
                return t;
            } else
                return null;
        }

        public void SetSliderX (float position) {
            EditorData.SliderX = position;
            SaveEditorData ();
        }

        public void SetSliderY (float position) {
            EditorData.SliderY = position;
            SaveEditorData ();
        }

        public float GetLastEditorScrollPositionX () {
            return EditorData.SliderX;
        }

        public float GetLastEditorScrollPositionY () {
            return EditorData.SliderY;
        }

        public void Save () {
            EditorUtility.SetDirty (Script);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }
    }
}