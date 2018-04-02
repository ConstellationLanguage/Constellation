using Constellation;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConstellationEditor {
    [InitializeOnLoadAttribute]
    public class ConstellationUnityWindow : ConstellationBaseWindow, IUndoable, ICopyable, ICompilable {
        protected NodeEditorPanel nodeEditorPanel;
        protected ConstellationsTabPanel nodeTabPanel;
        private float nodeSelectorWidht = 270;
        private NodeSelectorPanel nodeSelector;
        private string currentPath;
        public static ConstellationUnityWindow WindowInstance;
        private GameObject previousSelectedGameObject;
        private ConstellationBehaviour currentConstellationbehavior;
        Constellation.Constellation constellation;

        [MenuItem ("Window/Constellation Editor")]
        public static void ShowWindow () {
            WindowInstance = EditorWindow.GetWindow (typeof (ConstellationUnityWindow), false, "Constellation") as ConstellationUnityWindow;
        }

        void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
            RefreshNodeEditor ();
        }

        [MenuItem ("File/Constellation/New %&n")]
        static void NewConstellation () {
            if (WindowInstance != null)
                WindowInstance.New ();
            else {
                ShowWindow ();
                NewConstellation ();
            }
        }

        public void CompileScripts () {
            if (WindowInstance != null) {
                if (WindowInstance.ConstellationCompiler != null)
                    WindowInstance.ConstellationCompiler = new ConstellationCompiler ();
                WindowInstance.ConstellationCompiler.UpdateScriptsNodes (WindowInstance.scriptDataService.GetAllScriptsInProject ());
            }
        }

        [MenuItem ("File/Constellation/Save %&s")]
        static void SaveConstellation () {
            if (WindowInstance != null)
                WindowInstance.Save ();
            else
                ShowWindow ();
        }

        static void SaveConstellationInstance () {
            if (WindowInstance != null)
                WindowInstance.SaveInstance ();
            else
                ShowWindow ();
        }

        [MenuItem ("Edit/Constellation/Copy %&c")]
        static void CopyConstellation () {
            if (WindowInstance != null)
                WindowInstance.Copy ();
            else
                ShowWindow ();
        }

        [MenuItem ("Edit/Constellation/Paste %&v")]
        static void PasteConstellation () {
            if (WindowInstance != null)
                WindowInstance.Paste ();
            else
                ShowWindow ();
        }

        [MenuItem ("File/Constellation/Load %&l")]
        static void LoadConstellation () {
            if (WindowInstance != null)
                WindowInstance.Open ();
            else {
                ShowWindow ();
                WindowInstance.Open ();
            }
        }

        [MenuItem ("Edit/Constellation/Undo %&z")]
        static void UndoConstellation () {
            if (WindowInstance != null)
                WindowInstance.Undo ();
            else
                ShowWindow ();
        }

        [MenuItem ("Edit/Constellation/Redo %&y")]
        static void RedoConstellation () {
            if (WindowInstance != null)
                WindowInstance.Redo ();
            else
                ShowWindow ();
        }

        [MenuItem ("Help/Constellation tutorials")]
        static void Help () {
            Application.OpenURL ("https://github.com/ConstellationLanguage/Constellation/wiki");
        }

        public void Undo () {
            scriptDataService.Undo ();
            RefreshNodeEditor ();
        }

        public void Redo () {
            scriptDataService.Redo ();
            RefreshNodeEditor ();
        }

        public void Copy () {
            scriptDataService.GetEditorData ().clipBoard.AddSelection (nodeEditorPanel.GetNodeSelection ().SelectedNodes.ToArray (), nodeEditorPanel.LinksView.GetLinks ());
        }

        public void Paste () {
            var pastedNodes = scriptDataService.GetEditorData ().clipBoard.PasteClipBoard (scriptDataService.GetCurrentScript ());
            RefreshNodeEditor ();
            nodeEditorPanel.SelectNodes (pastedNodes);
        }

        public void Cut () {

        }

        public void AddAction () {
            scriptDataService.AddAction ();
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
                nodeEditorPanel = new NodeEditorPanel (this,
                    this,
                    scriptDataService.GetCurrentScript (),
                    this,
                    scriptDataService.GetEditorData ().clipBoard,
                    scriptDataService.GetLastEditorScrollPositionX (), scriptDataService.GetLastEditorScrollPositionY (), // Editor Position
                    OnLinkAdded, OnLinkRemoved, OnNodeAdded, OnNodeRemoved, OnHelpRequested, SaveConstellationInstance); // CallBacks 
                nodeTabPanel = new ConstellationsTabPanel (this);
            }
        }

        private void OnHelpRequested (string nodeName) {
            if(Application.isPlaying) {
                if(EditorUtility.DisplayDialog("Exit play mode", "You need to exit play mode in order to open a Constellation help.", "Continue", "Stop Playing"))
                    return;

                EditorApplication.isPlaying = false;
                return;
            }
            scriptDataService.GetEditorData ().ExampleData.openExampleConstellation = true;
            scriptDataService.GetEditorData ().ExampleData.constellationName = nodeName;
            EditorApplication.isPlaying = true;
        }

        protected override void Setup () {
            wantsMouseMove = true;
            canDrawUI = false;
            WindowInstance = this as ConstellationUnityWindow;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EditorApplication.playModeStateChanged += OnPlayStateChanged;
            if (scriptDataService != null) {
                nodeEditorPanel = new NodeEditorPanel (this,
                    this,
                    scriptDataService.GetCurrentScript (),
                    this,
                    scriptDataService.GetEditorData ().clipBoard,
                    scriptDataService.GetLastEditorScrollPositionX (), scriptDataService.GetLastEditorScrollPositionY (), // Saved editor position
                    OnLinkAdded, OnLinkRemoved, OnNodeAdded, OnNodeRemoved, OnHelpRequested, // callBacks
                    SaveConstellationInstance);
                nodeTabPanel = new ConstellationsTabPanel (this);
                if (scriptDataService.GetCurrentScript () != null)
                    WindowInstance.titleContent.text = scriptDataService.GetCurrentScript ().name;
                else
                    WindowInstance.titleContent.text = "Constellation";
                scriptDataService.ClearActions ();
            }
            nodeSelector = new NodeSelectorPanel (OnNodeAddRequested);
        }

        void OnGUI () {
            if (Event.current.type == EventType.Layout) {
                canDrawUI = true;
            }

            //Used to hide and show buttons
            if (Event.current.type == EventType.MouseMove) {
                RequestRepaint ();
            }

            if (canDrawUI) {
                if (IsConstellationSelected ()) {
                    DrawGUI ();
                } else if (!IsConstellationSelected ()) {
                    DrawStartGUI ();
                }
            } else {
                GUI.Label (new Rect (0, 0, 500, 500), "Loading");
                Repaint ();
            }
        }

        protected virtual void DrawStartGUI () {
            StartPanel.Draw (this);
            Recover ();
        }

        protected virtual void DrawGUI () {
            TopBarPanel.Draw (this, this, this, this);
            var constellationName = nodeTabPanel.Draw (scriptDataService.currentPath.ToArray (), CurrentEditedInstancesName);
            if (constellationName != null)
                Open (constellationName);

            var constellationToRemove = nodeTabPanel.ConstellationToRemove ();
            scriptDataService.RemoveOpenedConstellation (constellationToRemove);
            if (constellationToRemove != "" && constellationToRemove != null) {
                Recover ();
            }
            

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical ();
            nodeEditorPanel.DrawNodeEditor (new Rect(0,35,position.width - nodeSelectorWidht, position.height - 35));
            EditorGUILayout.EndVertical ();
            nodeSelector.Draw (nodeSelectorWidht, position.height - 50);
            EditorGUILayout.EndHorizontal ();
            RepaintIfRequested ();
        }

        static void OnPlayStateChanged (PlayModeStateChange state) {
            WindowInstance.CompileScripts ();
            WindowInstance.Recover ();
            WindowInstance.previousSelectedGameObject = null;
            WindowInstance.ResetInstances ();
            if (WindowInstance.scriptDataService.GetEditorData ().ExampleData.openExampleConstellation && state == PlayModeStateChange.EnteredPlayMode) {
                var nodeExampleLoader = new ExampleSceneLoader ();
                nodeExampleLoader.RunExample (WindowInstance.scriptDataService.GetEditorData ().ExampleData.constellationName, WindowInstance.scriptDataService);
                WindowInstance.scriptDataService.GetEditorData ().ExampleData.openExampleConstellation = false;
            }

            EditorApplication.playModeStateChanged -= OnPlayStateChanged;
        }

        void Update () {
            if (Application.isPlaying) {
                RequestRepaint ();
                if (nodeEditorPanel != null && previousSelectedGameObject != null && scriptDataService.GetCurrentScript ().IsInstance) {
                    nodeEditorPanel.Update (currentConstellationbehavior.Constellation);
                }

                var selectedGameObjects = Selection.gameObjects;
                if (selectedGameObjects.Length == 0 || selectedGameObjects[0] == previousSelectedGameObject)
                    return;

                var selectedConstellation = selectedGameObjects[0].GetComponent<ConstellationBehaviour> () as ConstellationBehaviour;
                if (selectedConstellation != null) {
                    currentConstellationbehavior = selectedConstellation;
                    previousSelectedGameObject = selectedGameObjects[0];
                    OpenConstellationInstance (selectedConstellation.Constellation, AssetDatabase.GetAssetPath (selectedConstellation.ConstellationData));
                }
            }
        }

        private void OnLinkAdded (LinkData link) {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentConstellationbehavior.AddLink (link);
        }

        private void OnLinkRemoved (LinkData link) {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentConstellationbehavior.RemoveLink (link);
        }

        private void OnNodeAdded (NodeData node) {
            if (Application.isPlaying && previousSelectedGameObject != null) {
                currentConstellationbehavior.AddNode (node);
                currentConstellationbehavior.RefreshConstellationEvents ();
            }
            Repaint ();
        }

        private void OnNodeRemoved (NodeData node) {
            if (Application.isPlaying && previousSelectedGameObject)
                currentConstellationbehavior.RemoveNode (node);

            Repaint ();
        }
        private void OnNodeAddRequested (string nodeName, string _namespace) {
            nodeEditorPanel.AddNode (nodeName, _namespace);
        }

    }
}