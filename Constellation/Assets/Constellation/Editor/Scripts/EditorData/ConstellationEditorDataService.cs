using System.Collections.Generic;
using System.IO;
using Constellation;
using UnityEditor;
using UnityEngine;
using Constellation.Unity3D;

namespace ConstellationEditor
{
    [System.Serializable]
    public class ConstellationEditorDataService
    {
        [SerializeField]
        public ConstellationScript Script;
        [SerializeField]
        protected ConstellationEditorData EditorData;
        [SerializeField]
        public List<ConstellationScriptInfos> currentPath;
        [SerializeField]
        private bool isSaved;
        [SerializeField]
        private const string tempPath = "Assets/Constellation/Editor/EditorData/Temp/";
        [SerializeField]
        public ConstellationEditorStyles ConstellationEditorStyles;
        [SerializeField]
        public List<ConstellationScriptInfos> currentInstancePath;

        public ConstellationEditorDataService()
        {

        }

        public void Initialize()
        {
            Setup();
            OpenEditorData();
            RefreshConstellationEditorDataList();
        }

        public ConstellationEditorStyles GetConstellationEditorConfig()
        {
            if (ConstellationEditorStyles == null)
                ConstellationEditorStyles = (ConstellationEditorStyles)AssetDatabase.LoadAssetAtPath(ConstellationEditor.GetEditorPath() + "ConstellationStyle.Asset", typeof(ConstellationEditorStyles));

            return ConstellationEditorStyles;
        }

        public void RefreshConstellationEditorDataList()
        {
            EditorData.ScriptAssembly.constellationScripts = new List<ConstellationScript>(SearchAllScriptsInProject());
            EditorData.ScriptAssembly.SetScriptAssembly();

        }

        public NodeNamespacesData[] GetAllCustomNodesNames()
        {
            var addedList = new List<NodeNamespacesData>();
            var nodes = new List<string>();
            foreach (var constellationScript in EditorData.ScriptAssembly.constellationScripts)
            {
                foreach (var node in constellationScript.GetNodes())
                {
                    if (node.Name == "Nestable")
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

        public ConstellationScript[] GetAllScriptsInProject()
        {
            if (EditorData.ScriptAssembly.constellationScripts == null || EditorData.ScriptAssembly.constellationScripts.Count == 0)
            {
                EditorData.ScriptAssembly.constellationScripts = new List<ConstellationScript>(SearchAllScriptsInProject());
            }
            return EditorData.ScriptAssembly.constellationScripts.ToArray();
        }

        public ConstellationScriptData[] GetAllScriptDataInProject()
        {
            if (EditorData.ScriptAssembly.constellationScripts == null || EditorData.ScriptAssembly.constellationScripts.Count == 0)
            {
                EditorData.ScriptAssembly.constellationScripts = new List<ConstellationScript>(SearchAllScriptsInProject());
            }
            return EditorData.ScriptAssembly.GetAllScriptData();
        }

        public void SetAllScriptsDirty()
        {

            foreach (var constellationScript in GetAllScriptsInProject())
            {
                EditorUtility.SetDirty(constellationScript);
            }
        }

        public ConstellationScript GetConstellationByName(string scriptName)
        {
            var constellation = EditorUtils.GetInstanceByName<ConstellationScript>(scriptName);
            return (ConstellationScript)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(constellation), typeof(ConstellationScript));
        }

        public void ResetConstellationEditorData()
        {
            Setup();
        }

        private ConstellationEditorData Setup()
        {
            var path = ConstellationEditor.GetEditorPath() + "EditorData.asset";
            var assemblyPath = ConstellationEditor.GetProjectPath() + "ConstellationAssembly.asset";
            AssetDatabase.DeleteAsset(assemblyPath);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();
            EditorData = ScriptableObject.CreateInstance<ConstellationEditorData>();
            AssetDatabase.CreateAsset(EditorData, path);
            EditorData.ScriptAssembly = ScriptableObject.CreateInstance<ConstellationScriptsAssembly>();
            AssetDatabase.CreateAsset(EditorData.ScriptAssembly, assemblyPath);
            return EditorData;
        }

        public ConstellationEditorData GetEditorData()
        {
            return EditorData;
        }

        public ConstellationEditorData OpenEditorData()
        {
            var path = ConstellationEditor.GetEditorPath() + "EditorData.asset";
            ConstellationEditorData t = (ConstellationEditorData)AssetDatabase.LoadAssetAtPath(path, typeof(ConstellationEditorData));
            if (t == null)
            {
                return Setup();
            }

            EditorData = t;
            if (EditorData.LastOpenedConstellationPath == null)
                EditorData.LastOpenedConstellationPath = new List<ConstellationScriptInfos>();
            return EditorData;
        }

        public void OpenConstellationInstance(Constellation.Constellation constellation, string instanceSourcePath)
        {
            var constellationScript = ScriptableObject.CreateInstance<ConstellationScript>();
            constellationScript.IsInstance = true;
            var path = "Assets/Constellation/Editor/EditorData/Temp/" + constellation.Name + "(Instance).asset";

            Script = constellationScript;
            AssetDatabase.CreateAsset(constellationScript, path);

            var nodes = constellation.GetNodes();
            var links = constellation.GetLinks();

            if (EditorData.CurrentInstancePath == null)
                EditorData.CurrentInstancePath = new List<ConstellationScriptInfos>();

            currentInstancePath = new List<ConstellationScriptInfos>(EditorData.CurrentInstancePath);
            var newInstanceObject = new ConstellationScriptInfos(path, ConstellationScriptInfos.ConstellationScriptTag.NoTag, true, instanceSourcePath);
            currentInstancePath.Add(newInstanceObject);

            currentPath = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath);
            if (!currentPath.Contains(newInstanceObject))
                currentPath.Insert(0, newInstanceObject);
            else
            {
                currentPath.Remove(newInstanceObject);
                currentPath.Insert(0, newInstanceObject);
            }

            foreach (var node in nodes)
            {
                Script.AddNode(node);
            }

            foreach (var link in links)
            {
                Script.AddLink(link);
            }

            SaveEditorData();
        }

        public void SaveInstance()
        {
            var newScript = ScriptableObject.CreateInstance<ConstellationScript>();
            var path = "";
            foreach (var instancePath in currentInstancePath)
            {
                if (instancePath == currentPath[0])
                {
                    path = instancePath.ScriptPath;
                }
            }
            if (path == "")
            {
                path = EditorUtility.SaveFilePanel("Save Constellation", Application.dataPath, "NewConstellation" + ".asset", "asset");
                if (path.Length == 0)
                    return;

                if (path.StartsWith(Application.dataPath))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                }
            }
            AssetDatabase.CreateAsset(newScript, path);
            newScript.Set(Script.script);
            SaveAll();
        }

        public bool CloseOpenedConstellation(ConstellationScriptInfos path)
        {
            if (path == null)
                return false;
            EditorData.LastOpenedConstellationPath.Remove(path);
            currentPath.Remove(path);
            //Script = null;
            return true;
        }

        public void CloseAllOpenedConstellation()
        {
            EditorData.LastOpenedConstellationPath.Clear();
        }

        public bool CloseCurrentConstellationInstance()
        {
            if (EditorData.CurrentInstancePath == null)
                return false;
            CloseOpenedConstellation(EditorData.CurrentInstancePath[0]);
            return true;
        }

        private void SaveEditorData()
        {
            EditorData.LastOpenedConstellationPath = new List<ConstellationScriptInfos>();
            if (currentPath == null)
                currentPath = new List<ConstellationScriptInfos>();
            foreach (var path in currentPath)
            {
                if (!EditorData.LastOpenedConstellationPath.Contains(path))
                    EditorData.LastOpenedConstellationPath.Add(path);
            }
            EditorData.CurrentInstancePath = currentInstancePath;
            EditorUtility.SetDirty(EditorData);
            EditorUtility.SetDirty(EditorData.ScriptAssembly);
            //AssetDatabase.SaveAssets ();
            //AssetDatabase.Refresh ();
        }

        public void RessetInstancesPath()
        {
            var constellationsToRemove = new List<ConstellationScriptInfos>();
            foreach (var constellationScriptInfo in currentPath)
            {
                if (constellationScriptInfo.IsIstance)
                    AssetDatabase.DeleteAsset(constellationScriptInfo.InstancePath);
            }

            if (currentPath != null)
            {
                foreach (var path in currentPath)
                {
                    ConstellationScript t = (ConstellationScript)AssetDatabase.LoadAssetAtPath(path.ScriptPath, typeof(ConstellationScript));
                    if (t == null)
                        constellationsToRemove.Add(path);
                }

                foreach (var constellationToRemove in constellationsToRemove)
                {
                    currentPath.Remove(constellationToRemove);
                }

            }
            currentInstancePath = new List<ConstellationScriptInfos>();
                if (Directory.Exists(tempPath)) { Directory.Delete(tempPath, true); }
                Directory.CreateDirectory(tempPath);
            SaveEditorData();
        }

        public ConstellationScript New()
        {
            Script = ScriptableObject.CreateInstance<ConstellationScript>();
            var path = EditorUtility.SaveFilePanel("Save Constellation", Application.dataPath, "NewConstellation" + ".asset", "asset");

            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            if (path == null || path == "")
            {
                Script = null;
                return Script;
            }
            AssetDatabase.CreateAsset(Script, path);
            if (currentPath == null)
                currentPath = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath.ToArray());

            currentPath.Insert(0, new ConstellationScriptInfos(path, ConstellationScriptInfos.ConstellationScriptTag.NoTag, false));
            SaveEditorData();
            return Script;
        }

        public ConstellationScript GetCurrentScript()
        {
            return Script;
        }

        public ConstellationScript OpenConstellation(ConstellationScriptInfos _path = null, bool save = true)
        {
            ConstellationScriptInfos scriptInfos;
            if (_path == null)
            {
                scriptInfos = new ConstellationScriptInfos(EditorUtility.OpenFilePanel("Load constellation", Application.dataPath, "asset"),
                    ConstellationScriptInfos.ConstellationScriptTag.NoTag,
                    false);
                if (scriptInfos.ScriptPath.Length == 0)
                    return null;
            }
            else
                scriptInfos = _path;

            if (scriptInfos.ScriptPath.StartsWith(Application.dataPath))
            {
                scriptInfos.ScriptPath = "Assets" + scriptInfos.ScriptPath.Substring(Application.dataPath.Length);
            }
            ConstellationScript t = (ConstellationScript)AssetDatabase.LoadAssetAtPath(scriptInfos.ScriptPath, typeof(ConstellationScript));

            Script = t;
            if (Script == null)
                throw new ScriptNotFoundAtPath(_path.ScriptPath);

            currentPath = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath);
            if (!currentPath.Contains(scriptInfos))
                currentPath.Insert(0, scriptInfos);
            else
            {
                currentPath.Remove(scriptInfos);
                currentPath.Insert(0, scriptInfos);
            }

            if (save)
                SaveEditorData();

            return t;
        }

        public void Export(string _path)
        {
            string exportedConstellation = JsonUtility.ToJson(Script.script);
            var path = EditorUtility.SaveFilePanel("Export as Constellation file", Application.dataPath, "Constellation" + ".const", "const");

            if (path.StartsWith(Application.dataPath))
            {
                //path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            System.IO.File.WriteAllText(path, exportedConstellation);
            SaveEditorData();
        }

        public ConstellationScript Recover(ConstellationScriptInfos _scriptInfos)
        {
            ConstellationScript t = (ConstellationScript)AssetDatabase.LoadAssetAtPath(_scriptInfos.ScriptPath, typeof(ConstellationScript));
            Script = t;
            if (t != null)
            {
                currentPath = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath);
                currentPath.Remove(_scriptInfos);
                currentPath.Insert(0, _scriptInfos);
                return t;
            }
            else
                throw new ScriptNotFoundAtPath(_scriptInfos.ScriptPath);
        }

        public void SetSliderX(float position)
        {
            EditorData.SliderX = position;
        }

        public void SetSliderY(float position)
        {
            EditorData.SliderY = position;
        }

        public float GetLastEditorScrollPositionX()
        {
            return EditorData.SliderX;
        }

        public float GetLastEditorScrollPositionY()
        {
            return EditorData.SliderY;
        }

        public void SaveScripts()
        {
            if (Script)
                EditorUtility.SetDirty(Script);
        }

        public void SaveAll()
        {
            if (Script)
                EditorUtility.SetDirty(Script);

            SaveEditorData();
        }
    }
}