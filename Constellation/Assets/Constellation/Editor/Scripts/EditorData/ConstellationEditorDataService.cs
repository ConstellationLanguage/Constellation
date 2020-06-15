using System.Collections.Generic;
using System.IO;
using Constellation;
using UnityEditor;
using UnityEngine;
using Constellation.Unity3D;
using Constellation.ConstellationTypes;
using System.Linq;

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

        public void UpdateStaticConstellationNodesNames()
        {
            var nodes = new List<ConstellationScriptData>();
            foreach (var constellationScript in EditorData.ScriptAssembly.ConstellationStaticNodes)
            {
                string assetPath = AssetDatabase.GetAssetPath(constellationScript.GetInstanceID());
                var name = Path.GetFileNameWithoutExtension(assetPath.Split('/').Last());
                constellationScript.script.Name = name;
            }
        }

        public ConstellationScriptData[] GetAllStaticScriptsDataInProject()
        {
            var nodes = new List<ConstellationScriptData>();
            foreach (var constellationScript in EditorData.ScriptAssembly.ConstellationStaticNodes)
            {
                foreach (var node in constellationScript.GetNodes())
                {
                    if (node.Name == StaticConstellationNode.NAME)
                    {
                        nodes.Add(constellationScript.script);
                        break;
                    }
                }
            }
            return nodes.ToArray();
        }

        public ConstellationScriptData[] GetAllTutorialScriptsDataInProject()
        {
            var nodes = new List<ConstellationScriptData>();
            foreach (var constellationTutorialScript in EditorData.ScriptAssembly.ConstellationTutorials)
            {
                foreach (var node in constellationTutorialScript.GetNodes())
                {
                    if (node.Name == Tutorial.NAME)
                    {
                        nodes.Add(constellationTutorialScript.script);
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

        public ConstellationStaticNodeScript[] SearchAllConstellationNodeScriptsInProject()
        {
            return EditorUtils.GetAllInstances<ConstellationStaticNodeScript>();
        }

        public ConstellationScript[] GetAllScriptsInProject()
        {
            if (EditorData.ScriptAssembly.ConstellationScripts == null || EditorData.ScriptAssembly.ConstellationScripts.Count == 0)
            {
                SetupScriptAssembly();
            }
            return EditorData.ScriptAssembly.ConstellationScripts.ToArray();
        }

        public ConstellationScript[] GetAllTutorialScriptsInProject()
        {
            if (EditorData.ScriptAssembly.ConstellationTutorials == null || EditorData.ScriptAssembly.ConstellationTutorials.Count == 0)
            {
                SetupScriptAssembly();
            }
            return EditorData.ScriptAssembly.ConstellationTutorials.ToArray();
        }

        public ConstellationScript[] GetAllStaticScriptsInProject()
        {
            if (EditorData.ScriptAssembly.ConstellationStaticNodes == null || EditorData.ScriptAssembly.ConstellationStaticNodes.Count == 0)
            {
                SetupScriptAssembly();
            }
            return EditorData.ScriptAssembly.ConstellationStaticNodes.ToArray();
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
            EditorData.ScriptAssembly.ConstellationStaticNodes = new List<ConstellationStaticNodeScript>(SearchAllConstellationNodeScriptsInProject());
        }

        public void SetAllScriptsDirty()
        {

            foreach (var constellationScript in GetAllScriptsInProject())
            {
                EditorUtility.SetDirty(constellationScript);
            }

            foreach(var constellationTutorial in GetAllTutorialScriptsInProject())
            {
                EditorUtility.SetDirty(constellationTutorial);
            }

            foreach (var constellationStatics in GetAllStaticScriptsInProject ())
            {
                EditorUtility.SetDirty(constellationStatics);
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
            constellationScript.CanChangeType = false;
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
            newScript.script = Script.script;
            AssetDatabase.CreateAsset(newScript, path);
            SaveAll();
            RefreshConstellationEditorDataList();
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
            EditorUtility.SetDirty(EditorData);
            EditorUtility.SetDirty(EditorData.ScriptAssembly);
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
                return null;
            }
            AssetDatabase.CreateAsset(Script, path);
            if (OpenedScripts == null)
                OpenedScripts = new List<ConstellationScriptInfos>(EditorData.LastOpenedConstellationPath.ToArray());

            OpenedScripts.Insert(0, new ConstellationScriptInfos(path, ConstellationScriptInfos.ConstellationScriptTag.NoTag, false));
            var name = Path.GetFileNameWithoutExtension(path.Split('/').Last());
            Script.script.Name = name;
            SaveEditorData();
            SaveScripts();
            RefreshConstellationEditorDataList();
            Script.CanChangeType = true;
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

            var name = Path.GetFileNameWithoutExtension(newScriptInfos.ScriptPath.Split('/').Last());
            Script.script.Name = name;

            if (save)
                SaveEditorData();

            return t;
        }

        public void Export(string _path)
        {
            string exportedConstellation = JsonUtility.ToJson(Script.script);
            var path = EditorUtility.SaveFilePanel("Export as Constellation file", Application.dataPath, "Constellation" + ".const", "const");
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
            Script.CanChangeType = false;
            RefreshConstellationEditorDataList();
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
            Script.CanChangeType = true;
            RefreshConstellationEditorDataList();
            return Script;
        }

        public ConstellationScript ConvertToConstellationNodeScript()
        {
            OpenedScripts[0].ScriptTag = ConstellationScriptInfos.ConstellationScriptTag.Tutorial;
            var scriptPath = OpenedScripts[0].ScriptPath;
            var tutorialAsset = ScriptableObject.CreateInstance<ConstellationStaticNodeScript>();
            tutorialAsset.script = Script.script;
            AssetDatabase.DeleteAsset(scriptPath);
            AssetDatabase.CreateAsset(tutorialAsset, scriptPath);
            Script = tutorialAsset;
            Script.CanChangeType = false;
            RefreshConstellationEditorDataList();
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