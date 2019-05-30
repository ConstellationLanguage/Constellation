using System.Collections.Generic;
using System.IO;
using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class ConstellationEditorDataService {
        private ConstellationScript Script;
        protected ConstellationEditorData EditorData;
        public List<string> currentPath;
        public List<ConstellationInstanceObject> currentInstancePath;
        private bool isSaved;
        private string tempPath = "Assets/Constellation/Editor/EditorData/Temp/";
        private ExamplePlayer ExamplePlayer;
        

        public ConstellationEditorDataService () {
            OpenEditorData ();
        }

        public void RefreshConstellationEditorDataList()
        {
            EditorData.ScriptAssembly.constellationScripts = new List<ConstellationScript>(SearchAllScriptsInProject());
            EditorData.ScriptAssembly.SetScriptAssembly();
        }

        public NodeNamespacesData [] GetAllCustomNodesNames()
        {
            var addedList = new List<NodeNamespacesData>();
            var nodes = new List<string>();
            foreach (var constellationScript in EditorData.ScriptAssembly.constellationScripts)
            {
                foreach (var node in constellationScript.GetNodes())
                {
                    if(node.Name == "Nestable")
                    {
                        nodes.Add("Constellation.Custom." + constellationScript.name);
                        break;
                    }
                }
            }
            addedList.Add(new NodeNamespacesData("Custom", nodes.ToArray()));
            return addedList.ToArray();
        }

        public ConstellationScriptData[] GetAllNestableScriptsInProject()
        {
            var nodes = new List<ConstellationScriptData>();
            foreach (var constellationScript in EditorData.ScriptAssembly.constellationScripts)
            {

                foreach (var node in constellationScript.GetNodes())
                {
                    if (node.Name == "Nestable")
                    {
                        
                        nodes.Add(constellationScript.script);
                        break;
                    }
                }
            }
            return nodes.ToArray();
        }

        public ConstellationScript[] SearchAllScriptsInProject()
        {
            return EditorUtils.GetAllInstances<ConstellationScript>();
        }

        public ConstellationScript[] GetAllScriptsInProject () {
            if(EditorData.ScriptAssembly.constellationScripts == null || EditorData.ScriptAssembly.constellationScripts.Count == 0)
            {
                EditorData.ScriptAssembly.constellationScripts = new List<ConstellationScript>(SearchAllScriptsInProject());
            }
            return EditorData.ScriptAssembly.constellationScripts.ToArray();
        }

        public ConstellationScript GetConstellationByName (string scriptName) {
            var constellation = EditorUtils.GetInstanceByName<ConstellationScript> (scriptName);
            return (ConstellationScript) AssetDatabase.LoadAssetAtPath (AssetDatabase.GetAssetPath (constellation), typeof (ConstellationScript));
        }

        public void ResetConstellationEditorData () {
            Setup ();
        }

        private ConstellationEditorData Setup () {
            var path = ConstellationEditor.GetEditorPath() + "EditorData.asset";
            var assemblyPath = ConstellationEditor.GetProjectPath() + "ConstellationAssembly.asset";
            AssetDatabase.DeleteAsset(assemblyPath);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();
            EditorData = ScriptableObject.CreateInstance<ConstellationEditorData> ();
            AssetDatabase.CreateAsset (EditorData, path);
            EditorData.ScriptAssembly = ScriptableObject.CreateInstance<ConstellationScriptsAssembly>();
            AssetDatabase.CreateAsset(EditorData.ScriptAssembly, assemblyPath);
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
            var path = ConstellationEditor.GetEditorPath() + "EditorData.asset";
            ConstellationEditorData t = (ConstellationEditorData) AssetDatabase.LoadAssetAtPath (path, typeof (ConstellationEditorData));
            if (t == null) {
                return Setup ();
            }

            EditorData = t;
            if (EditorData.LastOpenedConstellationPath == null)
                EditorData.LastOpenedConstellationPath = new List<string> ();
            return EditorData;
        }

        public void OpenConstellationInstance (Constellation.Constellation constellation, string instanceSourcePath) {
            var constellationScript = ScriptableObject.CreateInstance<ConstellationScript> ();
            constellationScript.IsInstance = true;
            var path = "Assets/Constellation/Editor/EditorData/Temp/" + constellation.Name + "(Instance).asset";

            Script = constellationScript;
            AssetDatabase.CreateAsset (constellationScript, path);

            var nodes = constellation.GetNodes ();
            var links = constellation.GetLinks ();

            if (EditorData.CurrentInstancePath == null)
                EditorData.CurrentInstancePath = new List<ConstellationInstanceObject> ();

            currentInstancePath = new List<ConstellationInstanceObject> (EditorData.CurrentInstancePath);
            var newInstanceObject = new ConstellationInstanceObject (path, instanceSourcePath);
            currentInstancePath.Add (newInstanceObject);

            currentPath = new List<string> (EditorData.LastOpenedConstellationPath);
            if (!currentPath.Contains (path))
                currentPath.Insert (0, path);
            else {
                currentPath.Remove (path);
                currentPath.Insert (0, path);
            }

            foreach (var node in nodes) {
                Script.AddNode (node);
            }

            foreach (var link in links) {
                Script.AddLink (link);
            }

            SaveEditorData ();
        }

        public void SaveInstance () {
            var newScript = ScriptableObject.CreateInstance<ConstellationScript> ();
            var path = "";
            foreach (var instancePath in currentInstancePath) {
                if (instancePath.InstancePath == currentPath[0]) {
                    path = instancePath.ScriptPath;
                }
            }
            if (path == "") {
                path = EditorUtility.SaveFilePanel ("Save Constellation", Application.dataPath, "NewConstellation" + ".asset", "asset");
                if (path.Length == 0)
                    return;

                if (path.StartsWith (Application.dataPath)) {
                    path = "Assets" + path.Substring (Application.dataPath.Length);
                }
            }
            AssetDatabase.CreateAsset (newScript, path);
            newScript.Set (Script.script);

            if (newScript)
                EditorUtility.SetDirty (newScript);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
            Save ();
        }

        public bool CloseOpenedConstellation (string path) {
            if (path == null)
                return false;
            EditorData.LastOpenedConstellationPath.Remove (path);
            currentPath.Remove (path);
            SaveEditorData ();
            return true;
        }

        public bool CloseCurrentConstellationInstance () {
            if (EditorData.CurrentInstancePath == null)
                return false;
            CloseOpenedConstellation (EditorData.CurrentInstancePath[0].InstancePath);
            return true;
        }

        private void SaveEditorData () {
            EditorData.LastOpenedConstellationPath = new List<string> ();
            if (currentPath == null)
                currentPath = new List<string> ();
            foreach (var path in currentPath) {
                if (!EditorData.LastOpenedConstellationPath.Contains (path))
                    EditorData.LastOpenedConstellationPath.Add (path);
            }
            EditorData.CurrentInstancePath = currentInstancePath;
            EditorUtility.SetDirty (EditorData);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }

        public void RessetInstancesPath () {
            var constellationsToRemove = new List<string> ();
            if (currentInstancePath != null)
                foreach (var constellationInstanceObject in currentInstancePath) {
                    AssetDatabase.DeleteAsset (constellationInstanceObject.InstancePath);
                }

            if (currentPath != null) {
                foreach (var path in currentPath) {
                    ConstellationScript t = (ConstellationScript) AssetDatabase.LoadAssetAtPath (path, typeof (ConstellationScript));
                    if (t == null)
                        throw new ConstellationScriptDataDoesNotExist ();
                    if (t.IsInstance) {
                        constellationsToRemove.Add (path);
                    }
                }

                foreach (var constellationToRemove in constellationsToRemove) {
                    currentPath.Remove (constellationToRemove);
                }

            }
            currentInstancePath = new List<ConstellationInstanceObject> ();
            if (Directory.Exists (tempPath)) { Directory.Delete (tempPath, true); }
            Directory.CreateDirectory (tempPath);
            SaveEditorData ();
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
            if (_path == "") {
                path = EditorUtility.OpenFilePanel ("Load constellation", Application.dataPath, "asset");
                if (path.Length == 0)
                    return null;
            } else
                path = _path;

            if (path.StartsWith (Application.dataPath)) {
                path = "Assets" + path.Substring (Application.dataPath.Length);
            }
            ConstellationScript t = (ConstellationScript) AssetDatabase.LoadAssetAtPath (path, typeof (ConstellationScript));

            Script = t;
            if (Script == null)
                throw new ScriptNotFoundAtPath (_path);

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
                throw new ScriptNotFoundAtPath (_path);
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
            if (Script)
                EditorUtility.SetDirty (Script);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();
        }
    }
}