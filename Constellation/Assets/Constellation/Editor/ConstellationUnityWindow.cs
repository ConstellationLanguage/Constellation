using Constellation;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConstellationEditor {
    [InitializeOnLoadAttribute]
    public class ConstellationUnityWindow : ConstellationBaseWindow, IUndoable, ICopyable {

        protected NodeEditorPanel nodeEditorPanel;
        protected ConstellationTabPanel nodeTabPanel;
        private float nodeSelectorWidht = 270;
        private NodeSelectorPanel nodeSelector;
        private string currentPath;
        public static ConstellationUnityWindow WindowInstance;
        private GameObject previousSelectedGameObject;
        private ConstellationBehaviour currentConstellation;
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

        [MenuItem ("File/Constellation/Save %&s")]
        static void SaveConstellation () {
            if (WindowInstance != null)
                WindowInstance.Save ();
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
        static void Help() {
            Application.OpenURL("https://www.constellationeditor.com/beginner");
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
                nodeEditorPanel = new NodeEditorPanel (this, this, scriptDataService.GetCurrentScript (), this, scriptDataService.GetEditorData ().clipBoard, scriptDataService.GetLastEditorScrollPositionX (), scriptDataService.GetLastEditorScrollPositionY (), OnLinkAdded, OnNodeAdded, OnNodeRemoved);
                nodeTabPanel = new ConstellationTabPanel (this);
            }
        }

        protected override void Setup () {
            wantsMouseMove = true;
            canDrawUI = false;
            WindowInstance = this as ConstellationUnityWindow;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EditorApplication.playModeStateChanged += OnPlayStateChanged;
            if (scriptDataService != null) {
                nodeEditorPanel = new NodeEditorPanel (this, this, scriptDataService.GetCurrentScript (), this, scriptDataService.GetEditorData ().clipBoard, scriptDataService.GetLastEditorScrollPositionX (), scriptDataService.GetLastEditorScrollPositionY (), OnLinkAdded, OnNodeAdded, OnNodeRemoved);
                nodeTabPanel = new ConstellationTabPanel (this);
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
            if (Event.current.type == EventType.MouseMove && !(Event.current.type == EventType.MouseDrag)) {
                RequestRepaint();
            }

            if (canDrawUI) {
                if (IsConstellationSelected ()) {
                    DrawGUI ();
                } else if (!IsConstellationSelected ()) {
                    DrawStartGUI ();
                }
            }else 
            {
                GUI.Label(new Rect(0,0,500,500), "Loading");
                Repaint();
            }
        }

        protected virtual void DrawStartGUI () {
            StartPanel.Draw (this);
            Recover ();
        }

        protected virtual void DrawGUI () {
            TopBarPanel.Draw (this, this, this);
            var constellationName = nodeTabPanel.Draw (scriptDataService.currentPath.ToArray ());
            if (constellationName != null)
                Open (constellationName);

            scriptDataService.RemoveOpenedConstellation (nodeTabPanel.ConstellationToRemove ());

            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical ();
            nodeEditorPanel.DrawNodeEditor (position.width - nodeSelectorWidht, position.height - 35);
            EditorGUILayout.EndVertical ();
            nodeSelector.Draw (nodeSelectorWidht, position.height - 50);
            EditorGUILayout.EndHorizontal ();
            if (shouldRepaint) {
                shouldRepaint = false;
                Repaint ();
            }
        }

        static void OnPlayStateChanged (PlayModeStateChange state) {
            WindowInstance.Recover ();
            WindowInstance.previousSelectedGameObject = null;
        
            EditorApplication.playModeStateChanged -= OnPlayStateChanged;
        }

        void Update () {
            if (Application.isPlaying) {
                RequestRepaint();
                if (nodeEditorPanel != null && previousSelectedGameObject != null) {
                    nodeEditorPanel.Update (currentConstellation.Constellation);
                }

                var selectedGameObjects = Selection.gameObjects;
                if (selectedGameObjects.Length == 0 || selectedGameObjects[0] == previousSelectedGameObject)
                    return;

                var selectedConstellation = selectedGameObjects[0].GetComponent<ConstellationBehaviour> () as ConstellationBehaviour;
                if (selectedConstellation != null) {
                    currentConstellation = selectedConstellation;
                    previousSelectedGameObject = selectedGameObjects[0];
                    Open (AssetDatabase.GetAssetPath (selectedConstellation.ConstellationData));
                }
            }
        }

        private void OnLinkAdded (LinkData link) {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentConstellation.AddLink (link);
        }

        private void OnLinkRemoved (LinkData link) {
            if (Application.isPlaying && previousSelectedGameObject != null)
                currentConstellation.RemoveLink (link);
        }

        private void OnNodeAdded (NodeData node) {
            if (Application.isPlaying && previousSelectedGameObject != null) {
                currentConstellation.AddNode (node);
                currentConstellation.RefreshConstellationEvents ();
            }
            Repaint ();
        }

        private void OnNodeRemoved (NodeData node) {
            if (Application.isPlaying && previousSelectedGameObject)
                currentConstellation.RemoveNode (node);

            Repaint ();
        }
        private void OnNodeAddRequested (string nodeName, string _namespace) {
            nodeEditorPanel.AddNode (nodeName, _namespace);
        }

    }
}