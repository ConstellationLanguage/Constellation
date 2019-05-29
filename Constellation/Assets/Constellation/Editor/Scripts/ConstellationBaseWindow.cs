using System;
using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor
{
    public abstract class ConstellationBaseWindow : ExtendedEditorWindow, ILoadable
    {
        protected ConstellationEditorDataService scriptDataService;
        protected ConstellationCompiler ConstellationCompiler;
        static protected bool canDrawUI = false;
        protected ConstellationInstanceObject[] CurrentEditedInstancesName;
        protected GameObject previousSelectedGameObject;
        protected ConstellationEditable currentEditableConstellation;

        public void Awake()
        {
            Setup();
            canDrawUI = false;
        }

        [MenuItem("Help/Constellation tutorials")]
        static void Help()
        {
            Application.OpenURL("https://github.com/ConstellationLanguage/Constellation/wiki");
        }

        protected abstract void Setup();

        public void New()
        {
            scriptDataService.New();
            scriptDataService.RefreshConstellationEditorDataList();
            Setup();
            
        }

        public void Recover()
        {
            try
            {
                scriptDataService = new ConstellationEditorDataService();
                ConstellationCompiler = new ConstellationCompiler();
                scriptDataService.RefreshConstellationEditorDataList();

                if (scriptDataService.OpenEditorData().LastOpenedConstellationPath == null)
                    return;

                if (scriptDataService.OpenEditorData().LastOpenedConstellationPath.Count != 0)
                {
                    var scriptData = scriptDataService.Recover(scriptDataService.OpenEditorData().LastOpenedConstellationPath[0]);
                    if (scriptData != null)
                    {
                        Setup();
                        return;
                    }
                }
            }
            catch (ConstellationError e)
            {
                ShowError(e, e);
            }
            catch (Exception e)
            {
                var formatedError = new UnknowError(this.GetType().Name);
                ShowError(formatedError, e);
            }
        }

        public void ResetInstances()
        {
            scriptDataService.RessetInstancesPath();
        }

        public void OpenConstellationInstance(Constellation.Constellation constellation, string path)
        {
            scriptDataService.OpenConstellationInstance(constellation, path);
            CurrentEditedInstancesName = scriptDataService.currentInstancePath.ToArray();
            Setup();
        }

        public void Open(string _path = "")
        {
            var script = scriptDataService.OpenConstellation(_path);
            if (script == null)
                return;
            Setup();
              
        }

        public void Save()
        {
            scriptDataService.Save();
        }

        public void SaveInstance()
        {
            scriptDataService.SaveInstance();
        }

        protected bool IsConstellationSelected()
        {
            if (scriptDataService != null)
            {
                if (scriptDataService.GetCurrentScript() != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        protected void OnLinkAdded(LinkData link)
        {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentEditableConstellation.AddLink(link);
        }

        protected void OnLinkRemoved(LinkData link)
        {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentEditableConstellation.RemoveLink(link);
        }

        protected void OnNodeAdded(NodeData node)
        {
            if (Application.isPlaying && previousSelectedGameObject != null)
            {
                currentEditableConstellation.AddNode(node);
            }
        }

        protected void OnNodeRemoved(NodeData node)
        {
            if (Application.isPlaying && previousSelectedGameObject)
                currentEditableConstellation.RemoveNode(node);
        }

        protected virtual void ShowEditorWindow() { }

        protected virtual void ShowError(ConstellationError constellationError, Exception exception = null)
        {
            var error = constellationError.GetError();
           
            if (error.IsIgnorable())
            {
                if (EditorUtility.DisplayDialog(error.GetErrorTitle() + " (" + error.GetID() + ") ", error.GetErrorMessage(), "Recover", "Ignore"))
                    UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                if (error.IsReportable())
                {
                    if (EditorUtility.DisplayDialog(error.GetErrorTitle() + " (" + error.GetID() + ") ", error.GetErrorMessage(), "Report and recover", "Recover"))
                    {
                        Application.OpenURL("https://github.com/ConstellationLanguage/Constellation/issues");
                    }
                }
                else
                    EditorUtility.DisplayDialog(error.GetErrorTitle() + " (" + error.GetID() + ") ", error.GetErrorMessage(), "Recover");

                UnityEditor.EditorApplication.isPlaying = false;
                scriptDataService.ResetConstellationEditorData();
                ShowEditorWindow();
            }

            if (exception != null && constellationError != null)
            {
                Debug.LogError(error.GetFormatedError() + exception.StackTrace);
                LogFile.WriteString(constellationError.GetError().GetErrorTitle(), exception.StackTrace);
            } else if (constellationError != null)
            {
                Debug.LogError(constellationError.StackTrace);
                LogFile.WriteString(constellationError.GetError().GetErrorTitle(), constellationError.StackTrace);
            } else if (exception != null) {
                Debug.LogError(exception.StackTrace);
                LogFile.WriteString("Uknown", exception.StackTrace);
            }
        }
    }
}