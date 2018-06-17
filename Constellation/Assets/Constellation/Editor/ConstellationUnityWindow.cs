using System;
using Constellation;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConstellationEditor
{
    [InitializeOnLoadAttribute]
    public class ConstellationUnityWindow : ConstellationBaseWindow, IUndoable, ICopyable, ICompilable
    {
        protected NodeEditorPanel nodeEditorPanel;
        protected ConstellationsTabPanel nodeTabPanel;
        private float nodeSelectorWidth = 270;
        private NodeSelectorPanel nodeSelector;
        private string currentPath;
        public static ConstellationUnityWindow WindowInstance;
        Constellation.Constellation constellation;
        private int splitThickness = 5;

        [MenuItem("Window/Constellation Editor")]
        public static void ShowWindow () {
        	CopyScriptIcons.Copy();
            WindowInstance = EditorWindow.GetWindow(typeof(ConstellationUnityWindow), false, "Constellation") as ConstellationUnityWindow;
        }

        protected override void ShowEditorWindow () {
            ShowWindow();
        }

        void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
            RefreshNodeEditor();
        }

        [MenuItem("File/Constellation/New %&n")]
        static void NewConstellation () {
            if (WindowInstance != null)
                WindowInstance.New();
            else
            {
                ShowWindow();
                NewConstellation();
            }
        }

        public void CompileScripts () {
            if (WindowInstance.ConstellationCompiler == null)
                WindowInstance.ConstellationCompiler = new ConstellationCompiler();

            WindowInstance.ConstellationCompiler.UpdateScriptsNodes(WindowInstance.scriptDataService.GetAllScriptsInProject());
            Recover();
        }

        [MenuItem("File/Constellation/Save %&s")]
        static void SaveConstellation () {
            if (WindowInstance != null)
                WindowInstance.Save();
            else
                ShowWindow();
        }

        static void SaveConstellationInstance () {
            if (WindowInstance != null)
                WindowInstance.SaveInstance();
            else
                ShowWindow();
        }

        [MenuItem("Edit/Constellation/Copy %&c")]
        static void CopyConstellation () {
            if (WindowInstance != null)
                WindowInstance.Copy();
            else
                ShowWindow();
        }

        [MenuItem("Edit/Constellation/Paste %&v")]
        static void PasteConstellation () {
            if (WindowInstance != null)
                WindowInstance.Paste();
            else
                ShowWindow();
        }

        [MenuItem("File/Constellation/Load %&l")]
        static void LoadConstellation () {
            if (WindowInstance != null)
                WindowInstance.Open();
            else {
                ShowWindow();
                WindowInstance.Open();
            }
        }

        [MenuItem("Edit/Constellation/Undo %&z")]
        static void UndoConstellation () {
            if (WindowInstance != null)
                WindowInstance.Undo();
            else
                ShowWindow();
        }

        [MenuItem("Edit/Constellation/Redo %&y")]
        static void RedoConstellation () {
            if (WindowInstance != null)
                WindowInstance.Redo();
            else
                ShowWindow();
        }

        public void Undo () {
            scriptDataService.Undo();
            RefreshNodeEditor();
        }

        public void Redo () {
            scriptDataService.Redo();
            RefreshNodeEditor();
        }

        public void Copy () {
            scriptDataService.GetEditorData().clipBoard.AddSelection(nodeEditorPanel.GetNodeSelection().SelectedNodes.ToArray(), nodeEditorPanel.GetLinks());
        }

        public void Paste () {
            var pastedNodes = scriptDataService.GetEditorData().clipBoard.PasteClipBoard(scriptDataService.GetCurrentScript());
            RefreshNodeEditor();
            nodeEditorPanel.SelectNodes(pastedNodes);
        }

        public void Cut()
        {

        }

        public void AddAction () {
            scriptDataService.AddAction();
        }

        void OnDestroy () {
            WindowInstance = null;
            EditorApplication.playModeStateChanged -= OnPlayStateChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected void RefreshNodeEditor () {
            canDrawUI = false;
            if (scriptDataService != null) {
                previousSelectedGameObject = null;
                nodeEditorPanel = new NodeEditorPanel(this,
                    this,
                    scriptDataService.GetCurrentScript(),
                    this,
                    scriptDataService.GetEditorData().clipBoard,
                    scriptDataService.GetLastEditorScrollPositionX(), scriptDataService.GetLastEditorScrollPositionY(), // Editor Position
                    OnLinkAdded, OnLinkRemoved, OnNodeAdded, OnNodeRemoved, OnHelpRequested, SaveConstellationInstance); // CallBacks 
                nodeTabPanel = new ConstellationsTabPanel(this);
            }
        }

        private void OnHelpRequested (string nodeName) {
            if (Application.isPlaying) {
                if (EditorUtility.DisplayDialog("Exit play mode", "You need to exit play mode in order to open a Constellation help.", "Continue", "Stop Playing"))
                    return;

                EditorApplication.isPlaying = false;
                return;
            }
            scriptDataService.GetEditorData().ExampleData.openExampleConstellation = true;
            scriptDataService.GetEditorData().ExampleData.constellationName = nodeName;
            EditorApplication.isPlaying = true;
        }

        protected override void Setup () {
            wantsMouseMove = true;
            canDrawUI = false;
            WindowInstance = this as ConstellationUnityWindow;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EditorApplication.playModeStateChanged += OnPlayStateChanged;
            if (scriptDataService != null) {
                nodeEditorPanel = new NodeEditorPanel(this,
                    this,
                    scriptDataService.GetCurrentScript(),
                    this,
                    scriptDataService.GetEditorData().clipBoard,
                    scriptDataService.GetLastEditorScrollPositionX(), scriptDataService.GetLastEditorScrollPositionY(), // Saved editor position
                    OnLinkAdded, OnLinkRemoved, OnNodeAdded, OnNodeRemoved, OnHelpRequested, // callBacks
                    SaveConstellationInstance);
                nodeTabPanel = new ConstellationsTabPanel(this);
                if (scriptDataService.GetCurrentScript() != null)
                    WindowInstance.titleContent.text = scriptDataService.GetCurrentScript().name;
                else
                    WindowInstance.titleContent.text = "Constellation";
                scriptDataService.ClearActions();
            }
            nodeSelector = new NodeSelectorPanel(OnNodeAddRequested);
        }

        void OnGUI () {
            try {
                if (Event.current.type == EventType.Layout) {
                    canDrawUI = true;
                }

                if (Event.current.type == EventType.MouseMove) {
                    RequestRepaint();
                }

                if (canDrawUI) {
                    if (IsConstellationSelected()) {
                        DrawGUI();
                    }
                    else if (!IsConstellationSelected()) {
                        DrawStartGUI();
                    }
                } else {
                    GUI.Label(new Rect(0, 0, 500, 500), "Loading");
                    Repaint();
                }
            }
            catch (ConstellationError e)
            {
                ShowError(e);
            }
            catch
            {
                var e = new UnknowError(this.GetType().Name);
                ShowError(e);
            }
        }

        protected virtual void DrawStartGUI () {
            StartPanel.Draw(this);
            Recover();
        }

        protected void OnNodeAddRequested (string nodeName, string _namespace) {
            nodeEditorPanel.AddNode(nodeName, _namespace);
        }

        protected virtual void DrawGUI () {
            TopBarPanel.Draw(this, this, this, this);
            var constellationName = nodeTabPanel.Draw(scriptDataService.currentPath.ToArray(), CurrentEditedInstancesName);
            if (constellationName != null)
                Open(constellationName);

            var constellationToRemove = nodeTabPanel.ConstellationToRemove();
            scriptDataService.CloseOpenedConstellation(constellationToRemove);
            if (constellationToRemove != "" && constellationToRemove != null) {
                Recover();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            nodeEditorPanel.DrawNodeEditor(new Rect(0, 35, position.width - nodeSelectorWidth - splitThickness, position.height - 35));
            EditorGUILayout.EndVertical();
            DrawVerticalSplit();
            nodeSelector.Draw(nodeSelectorWidth, position.height - 50);
            EditorGUILayout.EndHorizontal();
            RepaintIfRequested();
        }

        private void DrawVerticalSplit () {
            var verticalSplit = new Rect(position.width - nodeSelectorWidth - splitThickness, 30, splitThickness, position.height - 30);
            var newVertical = EditorUtils.VerticalSplit(verticalSplit);
            if (newVertical != verticalSplit.x) {
                NodeSelectorWidth -= verticalSplit.x - newVertical;
                RequestRepaint();
            }
        }

        static void OnPlayStateChanged (PlayModeStateChange state) {
            if (Application.isPlaying) {
                ConstellationUnityWindow.ShowWindow();
                WindowInstance.Recover();
                WindowInstance.CompileScripts();
            }

            WindowInstance.Recover();
            WindowInstance.previousSelectedGameObject = null;
            WindowInstance.ResetInstances();
            if (WindowInstance.scriptDataService.GetEditorData().ExampleData.openExampleConstellation && state == PlayModeStateChange.EnteredPlayMode) {
                var nodeExampleLoader = new ExampleSceneLoader();
                nodeExampleLoader.RunExample(WindowInstance.scriptDataService.GetEditorData().ExampleData.constellationName, WindowInstance.scriptDataService);
                WindowInstance.scriptDataService.GetEditorData().ExampleData.openExampleConstellation = false;
            }

            EditorApplication.playModeStateChanged -= OnPlayStateChanged;
            WindowInstance.Repaint();
        }

        void Update () {
            try {
                if (Application.isPlaying && IsConstellationSelected()) {
                    RequestRepaint();
                    if (nodeEditorPanel != null && previousSelectedGameObject != null && scriptDataService.GetCurrentScript().IsInstance) {
                        nodeEditorPanel.Update(currentEditableConstellation.GetConstellation());
                    }

                    var selectedGameObjects = Selection.gameObjects;
                    if (selectedGameObjects.Length == 0 || selectedGameObjects[0] == previousSelectedGameObject || selectedGameObjects[0].activeInHierarchy == false)
                        return;
                    else if (scriptDataService.GetCurrentScript().IsInstance) {
                        scriptDataService.CloseCurrentConstellationInstance();
                        previousSelectedGameObject = selectedGameObjects[0];
                        Recover();
                    }

                    var selectedConstellation = selectedGameObjects[0].GetComponent<ConstellationEditable>() as ConstellationEditable;
                    if (selectedConstellation != null) {
                        currentEditableConstellation = selectedConstellation;
                        previousSelectedGameObject = selectedGameObjects[0];
                        OpenConstellationInstance(selectedConstellation.GetConstellation(), AssetDatabase.GetAssetPath(selectedConstellation.GetConstellationData()));
                        if (selectedConstellation.GetConstellation() == null)
                        {
                            return;
                        }
                        selectedConstellation.Initialize();
                    }
                }
            } catch (ConstellationError e) {
                ShowError(e);
            } catch (Exception e) {
                var unknowError = new UnknowError(this.GetType().Name);
                ShowError(unknowError, e);
            }
        }

        protected virtual void OnLostFocus () {
            EditorApplication.playModeStateChanged -= OnPlayStateChanged;
            EditorApplication.playModeStateChanged += OnPlayStateChanged;
        }

        private float NodeSelectorWidth {
            get {
                return nodeSelectorWidth;
            }

            set {
                if(value > 137 + splitThickness && value < 590) {
                    nodeSelectorWidth = value;
                }
            }
        }
    }
}