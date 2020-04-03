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
        public List<ConstellationScriptInfos> OpenedScripts;
        [SerializeField]
        private bool isSaved;
        [SerializeField]
        private const string tempPath = "Assets/Constellation/Editor/EditorData/Temp/";
        [SerializeField]
        public ConstellationEditorStyles ConstellationEditorStyles;

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
            SetupScriptAssembly();
            EditorData.ScriptAssembly.SetScriptAssembly();
        }

        public NodeNamespacesData[] GetAllCustomNodesNames()
        {
            var addedList = new List<NodeNamespacesData>();
            var nodes = new List<string>();
            foreach (var constellationScript in EditorData.ScriptAssembly.ConstellationScripts)
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
            foreach (var constellationScript in EditorData.ScriptAssembly.ConstellationScripts)
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

        public ConstellationBehaviourScript[] SearchAllConstellationBehaviourScriptsInProject()
        {
            return EditorUtils.GetAllInstances<ConstellationBehaviourScript>();
        }

        public ConstellationTutorialScript[] SearchAllConstellationTutorialScriptsInProject()
        {
            return EditorUtils.GetAllInstances<ConstellationTutorialScript>();
        }

        public ConstellationScript[] GetAllScriptsInProject()
        {
            if (EditorData.ScriptAssembly.ConstellationScripts == null || EditorData.ScriptAssembly.ConstellationScripts.Count == 0)
            {
                SetupScriptAssembly();
            }
            return EditorData.ScriptAssembly.ConstellationScripts.ToArray();
        }

        public ConstellationScriptData[] GetAllScriptDataInProject()
        {
            if (EditorData.ScriptAssembly.ConstellationScripts == null || EditorData.ScriptAssembly.ConstellationScripts.Count == 0)
            {
                SetupScriptAssembly();
            }
            return EditorData.ScriptAssembly.GetAllScriptData();
        }

        public void SetupScriptAssembly()
        {
            EditorData.ScriptAssembly.ConstellationScripts = new List<ConstellationBehaviourScript>(SearchAllConstellationBehaviourScriptsInProject());
            EditorData.ScriptAssembly.ConstellationTutorials = new List<ConstellationTutorialScript>(SearchAllConstellationTutorialScriptsInProject());
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

        public void OpenConstellationInstance<T> (Constellation.Constellation constellation, string instanceSourcePath) where T : ConstellationScript
        {
            var constellationScript = ScriptableObject.CreateInstance<T>();
            constellationScript.IsInstance = true;
            var path = "Assets/Constellation/Editor/EditorData/Temp/" + constellation.Name + "(Instance).asset";

            Script = constellationScript;
            AssetDatabase.CreateAsset(constellationScript, path);

            var nodes = constellation.GetNodes();
            var links = constellation.GetLinks();

            var newInstanceObject = new ConstellationScriptInfos(instanceSourcePath, ConstellationScriptInfos.ConstellationScriptTag.NoTag, true, path);

            OpenedScripts = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath);
            if (!OpenedScripts.Contains(newInstanceObject))
                OpenedScripts.Insert(0, newInstanceObject);
            else
            {
                OpenedScripts.Remove(newInstanceObject);
                OpenedScripts.Insert(0, newInstanceObject);
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

        public void SaveInstance(ConstellationScriptInfos scriptInfos)
        {
            var newScript = ScriptableObject.CreateInstance<ConstellationBehaviourScript>();
            var path = "";

            if (scriptInfos.IsIstance)
            {
                path = scriptInfos.ScriptPath;
            }else
            {
                return;
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

        public bool CloseOpenedConstellation(ConstellationScriptInfos scriptInfos)
        {
            if (scriptInfos == null)
                return false;
            EditorData.LastOpenedConstellationPath.Remove(scriptInfos);
            OpenedScripts.Remove(scriptInfos);
            //Script = null;
            return true;
        }

        public void CloseAllOpenedConstellation()
        {
            EditorData.LastOpenedConstellationPath.Clear();
        }

        public bool CloseCurrentConstellationInstance()
        {
            foreach (var script in EditorData.LastOpenedConstellationPath)
            {
                if (script.IsIstance)
                {
                    CloseOpenedConstellation(script);

                    return true;
                }
            }
            return false;
        }

        private void SaveEditorData()
        {
            EditorData.LastOpenedConstellationPath = new List<ConstellationScriptInfos>();
            if (OpenedScripts == null)
                OpenedScripts = new List<ConstellationScriptInfos>();
            foreach (var path in OpenedScripts)
            {
                if (!EditorData.LastOpenedConstellationPath.Contains(path))
                    EditorData.LastOpenedConstellationPath.Add(path);
            }
            //EditorData.CurrentInstancePath = currentInstancePath;
            EditorUtility.SetDirty(EditorData);
            EditorUtility.SetDirty(EditorData.ScriptAssembly);
            //AssetDatabase.SaveAssets ();
            //AssetDatabase.Refresh ();
        }

        public void RessetInstancesPath()
        {
            var constellationsToRemove = new List<ConstellationScriptInfos>();
            foreach (var constellationScriptInfo in OpenedScripts)
            {
                if (constellationScriptInfo.IsIstance)
                    AssetDatabase.DeleteAsset(constellationScriptInfo.InstancePath);
            }

            if (OpenedScripts != null)
            {
                foreach (var path in OpenedScripts)
                {
                    ConstellationScript t = (ConstellationScript)AssetDatabase.LoadAssetAtPath(path.ScriptPath, typeof(ConstellationScript));
                    if (t == null)
                        constellationsToRemove.Add(path);
                }

                foreach (var constellationToRemove in constellationsToRemove)
                {
                    OpenedScripts.Remove(constellationToRemove);
                }

            }
            if (Directory.Exists(tempPath)) { Directory.Delete(tempPath, true); }
            Directory.CreateDirectory(tempPath);
            SaveEditorData();
        }

        public ConstellationScript New()
        {
            Script = ScriptableObject.CreateInstance<ConstellationBehaviourScript>();
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
            if (OpenedScripts == null)
                OpenedScripts = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath.ToArray());

            OpenedScripts.Insert(0, new ConstellationScriptInfos(path, ConstellationScriptInfos.ConstellationScriptTag.NoTag, false));
            SaveEditorData();
            SaveScripts();
            RefreshConstellationEditorDataList();
            return Script;
        }

        public ConstellationScript GetCurrentScript()
        {
            return Script;
        }

        public ConstellationScript OpenConstellation(ConstellationScriptInfos _path = null, bool save = true)
        {
            ConstellationScriptInfos newScriptInfos;
            if (_path == null)
            {
                newScriptInfos = new ConstellationScriptInfos(EditorUtility.OpenFilePanel("Load constellation", Application.dataPath, "asset"),
                    ConstellationScriptInfos.ConstellationScriptTag.NoTag,
                    false);
                if (newScriptInfos.ScriptPath.Length == 0)
                    return null;
            }
            else
                newScriptInfos = _path;

            if (newScriptInfos.ScriptPath.StartsWith(Application.dataPath))
            {
                newScriptInfos.ScriptPath = "Assets" + newScriptInfos.ScriptPath.Substring(Application.dataPath.Length);
            }
            ConstellationScript t = (ConstellationScript)AssetDatabase.LoadAssetAtPath(newScriptInfos.ScriptPath, typeof(ConstellationScript));

            Script = t;
            if (Script == null)
                throw new ScriptNotFoundAtPath(_path.ScriptPath);

            OpenedScripts = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath);
            var isAlreadyOpened = false;
            var openedIndex = 0;
            foreach (var openedScripts in OpenedScripts)
            {
                if (openedScripts.ScriptPath == newScriptInfos.ScriptPath)
                {
                    isAlreadyOpened = true;
                    break;
                }
                openedIndex++;
            }
            //Debug.Log(openedIndex);
            if (!isAlreadyOpened)
            {
                OpenedScripts.Insert(0, newScriptInfos);
            }
            else
            {
                OpenedScripts.Remove(OpenedScripts[openedIndex]);
                OpenedScripts.Insert(0, newScriptInfos);
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
                OpenedScripts = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath);
                OpenedScripts.Remove(_scriptInfos);
                OpenedScripts.Insert(0, _scriptInfos);
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

        public ConstellationScript ConvertCurrentConstellationToTutorial()
        {
            OpenedScripts[0].ScriptTag = ConstellationScriptInfos.ConstellationScriptTag.Tutorial;
            var scriptPath = OpenedScripts[0].ScriptPath;
            var tutorialAsset = ScriptableObject.CreateInstance<ConstellationTutorialScript>();
            tutorialAsset.script = Script.script;
            AssetDatabase.DeleteAsset(scriptPath);
            AssetDatabase.CreateAsset(tutorialAsset, scriptPath);
            Script = tutorialAsset;
            return Script;
        }

        public ConstellationScript ConvertToConstellationScript()
        {
            OpenedScripts[0].ScriptTag = ConstellationScriptInfos.ConstellationScriptTag.Tutorial;
            var scriptPath = OpenedScripts[0].ScriptPath;
            var tutorialAsset = ScriptableObject.CreateInstance<ConstellationBehaviourScript>();
            tutorialAsset.script = Script.script;
            AssetDatabase.DeleteAsset(scriptPath);
            AssetDatabase.CreateAsset(tutorialAsset, scriptPath);
            Script = tutorialAsset;
            return Script;
        }

        public void SaveAll()
        {
            if (Script)
                EditorUtility.SetDirty(Script);

            SaveEditorData();
        }
    }
}