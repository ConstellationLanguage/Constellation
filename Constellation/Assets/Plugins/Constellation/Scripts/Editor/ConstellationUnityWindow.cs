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
        Constellation.Constellation constellation;

        [MenuItem ("Constellation/Editor")]
        public static void ShowWindow () {
            WindowInstance = EditorWindow.GetWindow (typeof (ConstellationUnityWindow), false, "Constellation") as ConstellationUnityWindow;
            
        }

        void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
            RefreshNodeEditor();
        }

        [MenuItem ("Constellation/Files/New %&n")]
        static void NewConstellation () {
            if (WindowInstance != null)
                WindowInstance.New ();
            else {
                ShowWindow ();
                NewConstellation ();
            }
        }

        [MenuItem ("Constellation/Files/Save %&s")]
        static void SaveConstellation () {
            if (WindowInstance != null)
                WindowInstance.Save ();
            else
                ShowWindow ();
        }

        [MenuItem ("Constellation/Files/Copy %&c")]
        static void CopyConstellation () {
            if (WindowInstance != null)
                WindowInstance.Copy ();
            else
                ShowWindow ();
        }

        [MenuItem ("Constellation/Files/Paste %&v")]
        static void PasteConstellation () {
            if (WindowInstance != null)
                WindowInstance.Paste ();
            else
                ShowWindow ();
        }

        [MenuItem ("Constellation/Files/Load %&l")]
        static void LoadConstellation () {
            if (WindowInstance != null)
                WindowInstance.Open ();
            else {
                ShowWindow ();
                WindowInstance.Open ();
            }
        }

        [MenuItem ("Constellation/Edit/Undo %&z")]
        static void UndoConstellation () {
            if (WindowInstance != null)
                WindowInstance.Undo ();
            else
                ShowWindow ();
        }

        [MenuItem ("Constellation/Edit/Redo %&y")]
        static void RedoConstellation () {
            if (WindowInstance != null)
                WindowInstance.Redo ();
            else
                ShowWindow ();
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
            SceneManager.sceneLoaded -=  OnSceneLoaded;
        }

        protected void RefreshNodeEditor () {
            if (scriptDataService != null) {
                previousSelectedGameObject = null;
                nodeEditorPanel = new NodeEditorPanel (this, this, scriptDataService.GetCurrentScript (), this, scriptDataService.GetEditorData ().clipBoard, scriptDataService.GetLastEditorScrollPositionX (), scriptDataService.GetLastEditorScrollPositionY (), OnLinkAdded, OnNodeAdded, OnNodeRemoved);
                nodeTabPanel = new ConstellationTabPanel (this);
            }
        }

        protected override void Setup () {
            WindowInstance = this as ConstellationUnityWindow;
            SceneManager.sceneLoaded +=  OnSceneLoaded;
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
            try {
                if (IsConstellationSelected () && canDrawUI) {
                    DrawGUI ();
                } else if (!IsConstellationSelected () && canDrawUI) {
                    DrawStartGUI ();
                } else {
                    canDrawUI = true;
                    Repaint ();
                }
            } catch {
                Repaint ();
            }
        }

        protected virtual void DrawStartGUI () {
            StartPanel.Draw (this);
            Recover ();
        }

        protected virtual void DrawGUI () {
            EditorGUILayout.BeginVertical ();
            TopBarPanel.Draw (this, this, this);
            var constellationName = nodeTabPanel.Draw (scriptDataService.currentPath.ToArray ());
            if (constellationName != null)
                Open (constellationName);
            scriptDataService.RemoveOpenedConstellation (nodeTabPanel.NodeToRemove ());
            EditorGUILayout.EndVertical ();
            EditorGUILayout.BeginHorizontal ();
            EditorGUILayout.BeginVertical ();
            nodeEditorPanel.DrawNodeEditor (position.width - nodeSelectorWidht, position.height - 35);
            EditorGUILayout.EndVertical ();
            nodeSelector.Draw (nodeSelectorWidht, position.height - 50);
            EditorGUILayout.EndHorizontal ();
        }

        static void OnPlayStateChanged (PlayModeStateChange state) {
            WindowInstance.Recover ();
            EditorApplication.playModeStateChanged -= OnPlayStateChanged;
        }

        void Update () {
            if (Application.isPlaying) {
                if (nodeEditorPanel != null && previousSelectedGameObject != null) {
                    nodeEditorPanel.Update (previousSelectedGameObject.GetComponent<ConstellationBehaviour> ().Constellation);
                }

                var selectedGameObjects = Selection.gameObjects;
                if (selectedGameObjects.Length == 0 || selectedGameObjects[0] == previousSelectedGameObject)
                    return;

                var selectedConstellation = selectedGameObjects[0].GetComponent<ConstellationBehaviour> () as ConstellationBehaviour;
                if (selectedConstellation != null) {
                    previousSelectedGameObject = selectedGameObjects[0];
                    Open (AssetDatabase.GetAssetPath (selectedConstellation.ConstellationData));
                }
            }
        }

        private void OnLinkAdded (LinkData link) {
            if (Application.isPlaying)
                previousSelectedGameObject.GetComponent<ConstellationBehaviour> ().AddLink (link);
        }

        private void OnLinkRemoved (LinkData link) {
            if (Application.isPlaying)
                previousSelectedGameObject.GetComponent<ConstellationBehaviour> ().RemoveLink (link);
        }

        private void OnNodeAdded (NodeData node) {
            if (Application.isPlaying)
                previousSelectedGameObject.GetComponent<ConstellationBehaviour> ().AddNode (node);
        }

        private void OnNodeRemoved (NodeData node) {
            if (Application.isPlaying)
                previousSelectedGameObject.GetComponent<ConstellationBehaviour> ().RemoveNode (node);
        }
        private void OnNodeAddRequested (string nodeName, string _namespace) {
            nodeEditorPanel.AddNode (nodeName, _namespace);
        }

    }
}