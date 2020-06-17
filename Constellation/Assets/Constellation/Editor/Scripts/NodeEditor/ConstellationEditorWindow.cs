using UnityEditor;
using UnityEngine;
using Constellation;
using ConstellationEditor;
using Constellation.Unity3D;

public class ConstellationEditorWindow : EditorWindow, ILoadable, ICopyable, IParsable
{
    public NodeWindow NodeWindow;
    public NodeSelectorPanel NodeSelector;
    public ConstellationsTabPanel NodeTabPanel;
    private const string editorPath = "Assets/Constellation/Editor/EditorAssets/";
    public NodesFactory NodesFactory;
    public ConstellationEditorDataService ScriptDataService;
    public ConstellationScript ConstellationScript;
    public float nodeSelectorWidth = 300;
    public ConstellationParser ConstellationParser;
    const float splitThickness = 3;

    public Vector2 nodeWindowSize;
    public Vector2 nodeWindowScrollPosition;

    //Runtime
    public GameObject previousSelectedGameObject;
    ConstellationEditable currentEditableConstellation;
    public static ConstellationEditorWindow ConstellationEditorWindowInstance;

    bool requestRepaint = false;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Constellation Editor")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        ConstellationEditorWindow window = (ConstellationEditorWindow)EditorWindow.GetWindow(typeof(ConstellationEditorWindow), false, "Constellation");
        window.Show();
        ConstellationEditorWindowInstance = window;
    }

    public void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayStateChanged;
        UnityEditor.Undo.undoRedoPerformed += OnUndoPerformed;
        SetupWindow();
    }

    void SetupNodeSelector()
    {
        NodeSelector = new NodeSelectorPanel(ScriptDataService.GetEditorData().ScriptAssembly.GetAllStaticScriptData());
        NodeSelector.SetupNamespaceData((ScriptDataService.GetEditorData().ScriptAssembly.GetAllStaticScriptData()));
    }

    void NodeAdded(string _nodeName, string _namespace)
    {
        NodeWindow.AddNode(_nodeName, _namespace, OnEditorEvent);
        ScriptDataService.SaveScripts();
    }

    void ResetWindow()
    {
        ScriptDataService = null;
        SetupWindow();
    }

    void SetupWindow()
    {
        if (ScriptDataService == null)
        {
            ScriptDataService = new ConstellationEditorDataService();
            ScriptDataService.Initialize();
        }


        if (NodeTabPanel == null)
            NodeTabPanel = new ConstellationsTabPanel();

        if (NodeSelector == null)
        {
            SetupNodeSelector();
        }
    }

    void OnGUI()
    {
        if (!IsConstellationSelected())
        {
            StartPanel.Draw(this);
            return;
        }
        else
        {
            try
            {
                TopBarPanel.Draw(this, this, this);
                var constellationName = NodeTabPanel.Draw(ScriptDataService.OpenedScripts.ToArray());
                if (constellationName != null)
                {
                    var selectedGameObjects = Selection.gameObjects;
                    if (Application.isPlaying)
                    {
                        ScriptDataService.CloseCurrentConstellationInstance();
                        Open(ScriptDataService.OpenedScripts.ToArray()[0]);
                        previousSelectedGameObject = selectedGameObjects[0];
                    }
                    Open(constellationName);
                }
                var constellationToRemove = NodeTabPanel.ConstellationToRemove();
                EditorGUILayout.BeginHorizontal();
                if (NodeWindow == null)
                    ParseScript();
                NodeWindow.UpdateSize(position.width - nodeSelectorWidth - splitThickness, position.height - NodeTabPanel.GetHeight());
                NodeWindow.Draw(RequestRepaint, OnEditorEvent, ScriptDataService.GetConstellationEditorConfig(), out nodeWindowSize, out nodeWindowScrollPosition);
                DrawVerticalSplit();
                NodeSelector.Draw(nodeSelectorWidth, position.height, NodeAdded, ScriptDataService.Script.CanChangeType);
                EditorGUILayout.EndHorizontal();
                if (ScriptDataService.CloseOpenedConstellation(constellationToRemove))
                {
                    if (ScriptDataService.OpenedScripts.Count > 0)
                    {
                        Open(ScriptDataService.OpenedScripts[0]);
                    }
                    else
                    {
                        ScriptDataService.Script = null;
                    }
                }
                DrawInstancePannel();
            }
            catch (System.Exception exception)
            {
                UnexpectedError(exception.StackTrace);
            }
        }

        if (requestRepaint)
            Repaint();
    }

    private void DrawInstancePannel()
    {
        if (ConstellationScript != null && (!ConstellationScript.IsDifferentThanSource || ConstellationScript.GetType() == typeof(ConstellationTutorialScript)))
            return;

        GUI.color = Color.yellow;
        Event e = Event.current;
        var x = 0;
        var y = 40;
        var width = 100;
        var height = 25;

        if (GUI.Button(new Rect(x, y, width, height), "Apply"))
        {
            if (ConstellationScript.IsInstance)
            {
                ConstellationScript.IsDifferentThanSource = false;
            }
            ScriptDataService.SaveInstance(ScriptDataService.GetEditorData().LastOpenedConstellationPath[0]);
        }
        GUI.color = Color.white;
    }

    void Update()
    {
        try
        {
            if (Application.isPlaying && IsConstellationSelected())
            {
                if (NodeWindow != null && previousSelectedGameObject != null && ScriptDataService.GetCurrentScript().IsInstance)
                {
                    NodeWindow.Update(currentEditableConstellation.GetConstellation(), OnEditorEvent);
                }

                var selectedGameObjects = Selection.gameObjects;
                if (selectedGameObjects.Length == 0 || selectedGameObjects[0] == previousSelectedGameObject || selectedGameObjects[0].activeInHierarchy == false)
                    return;
                else if (ScriptDataService.OpenedScripts.Count > 0)
                {
                    ScriptDataService.CloseCurrentConstellationInstance();
                    Open(ScriptDataService.OpenedScripts.ToArray()[0]);
                    previousSelectedGameObject = selectedGameObjects[0];
                }
                else
                {
                    ScriptDataService.Script = null;
                }

                var selectedConstellation = selectedGameObjects[0].GetComponent<ConstellationEditable>() as ConstellationEditable;
                if (selectedConstellation != null && selectedConstellation.ConstellationData != null)
                {
                    currentEditableConstellation = selectedConstellation;
                    previousSelectedGameObject = selectedGameObjects[0];
                    if (selectedConstellation.ConstellationData.GetType() == typeof(ConstellationTutorialScript))
                        OpenConstellationInstance<ConstellationTutorialScript>(selectedConstellation.GetConstellation(), AssetDatabase.GetAssetPath(selectedConstellation.GetConstellationData()));
                    else
                        OpenConstellationInstance<ConstellationScript>(selectedConstellation.GetConstellation(), AssetDatabase.GetAssetPath(selectedConstellation.GetConstellationData()));

                    if (selectedConstellation.GetConstellation() == null)
                    {
                        return;
                    }
                }
            }
            else
            {
                previousSelectedGameObject = null;
            }
        }catch(System.Exception exception)
        {
            UnexpectedError(exception.StackTrace);
        }
    }

    private void DrawVerticalSplit()
    {
        var verticalSplit = new Rect(position.width - nodeSelectorWidth - splitThickness, 30, splitThickness, position.height - 30);
        var newVertical = EditorUtils.VerticalSplit(verticalSplit);
        if (newVertical != verticalSplit.x)
        {
            nodeSelectorWidth -= verticalSplit.x - newVertical;
            nodeSelectorWidth = Mathf.Max(150, nodeSelectorWidth);
            RequestRepaint();
        }
    }

    void OnEditorEvent(ConstellationEditorEvents.EditorEventType eventType, string eventMessage)
    {
        ScriptDataService.SaveScripts();
        if (eventType == ConstellationEditorEvents.EditorEventType.AddToUndo)
        {
            Undo.RegisterCompleteObjectUndo(ConstellationScript, eventMessage);
        }
        if (eventType == ConstellationEditorEvents.EditorEventType.HelpClicked)
        {
            OnHelpRequested(eventMessage);
        }
        if (eventType == ConstellationEditorEvents.EditorEventType.LinkAdded)
        {
            OnLinkAdded(ScriptDataService.GetCurrentScript().GetLinkByGUID(eventMessage));
        }
        if (eventType == ConstellationEditorEvents.EditorEventType.LinkDeleted)
        {
            OnLinkRemoved(ScriptDataService.GetCurrentScript().GetLinkByGUID(eventMessage));
        }
        if (eventType == ConstellationEditorEvents.EditorEventType.NodeAdded)
        {
            OnNodeAdded(ScriptDataService.GetCurrentScript().GetNodeByGUID(eventMessage));
        }
        if (eventType == ConstellationEditorEvents.EditorEventType.NodeDeleted)
        {
            OnNodeRemoved(ScriptDataService.GetCurrentScript().GetNodeByGUID(eventMessage));
        }
        if (eventType == ConstellationEditorEvents.EditorEventType.NodeMoved)
        {
            OnNodeMoved();
        }
    }

    void OnNodeMoved()
    {
        if (Application.isPlaying && previousSelectedGameObject != null && ConstellationScript.IsInstance)
        {
            ConstellationScript.IsDifferentThanSource = true;
        }
    }

    void SetupNodeWindow()
    {
        //SetupWindow();
        NodeWindow = new NodeWindow(editorPath, ScriptDataService, nodeWindowSize, nodeWindowScrollPosition);
    }

    void RequestRepaint()
    {
        requestRepaint = true;

    }

    public void Open(ConstellationScriptInfos _path)
    {
        var openedConstellation = ScriptDataService.OpenConstellation(_path);
        if (openedConstellation != null)
        {
            ConstellationScript = openedConstellation;
            SetupNodeWindow();
            Save();
        }
    }

    public void Save()
    {
        ScriptDataService.SaveScripts();
        AssetDatabase.SaveAssets();
    }

    public void New()
    {
        var newConstellation = ScriptDataService.New();
        if (newConstellation != null)
        {
            ConstellationScript = newConstellation;
            SetupNodeWindow();
            RequestRepaint();
        }
    }

    protected bool IsConstellationSelected()
    {
        if (ScriptDataService != null)
        {
            if (ScriptDataService.GetCurrentScript() != null)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public void OpenConstellationInstance<T>(Constellation.Constellation constellation, string path) where T : ConstellationScript
    {
        ScriptDataService.OpenConstellationInstance<T>(constellation, path);
        ConstellationScript = ScriptDataService.Script;
        ConstellationScript.ScriptAssembly = ScriptDataService.GetEditorData().ScriptAssembly;

        SetupNodeWindow();
    }

    public void Copy()
    {
        ScriptDataService.GetEditorData().clipBoard.AddSelection(NodeWindow.GetSelectedNodes(), ConstellationScript.GetLinks());
    }

    public void Paste()
    {
        var nodes = ScriptDataService.GetEditorData().clipBoard.PasteClipBoard(ConstellationScript);
        if (nodes == null)
            return;
        Open(ScriptDataService.OpenedScripts.ToArray()[0]);
        NodeWindow.SelectNodes(nodes);
    }

    public void Cut()
    {
        throw new System.NotImplementedException();
    }

    public void Export()
    {
        ScriptDataService.Export("");
    }

    public void ResetInstances()
    {
        ScriptDataService.RessetInstancesPath();
    }

    void OnUndoPerformed()
    {
        Open(ScriptDataService.OpenedScripts.ToArray()[0]);
    }

    void UnexpectedError(string Error)
    {
        ResetWindow();
        Debug.LogError("[CONSTELLATION] Something unexpected happened \n" + Error);
        EditorUtility.DisplayDialog("CONSTELLATION", "Something unexpected happened. See the console for more details.", "Ok");
    }

    void OnPlayStateChanged(PlayModeStateChange state)
    {
        try
        {
            Selection.objects = new Object[0];
            previousSelectedGameObject = null;
            if (IsConstellationSelected())
            {
                if (state == PlayModeStateChange.EnteredEditMode)
                {
                    ResetInstances();
                    ScriptDataService.CloseCurrentConstellationInstance();
                    if (ScriptDataService.OpenedScripts.Count > 0)
                    {
                        Open(ScriptDataService.OpenedScripts[0]);
                    }
                }

                if (state == PlayModeStateChange.ExitingEditMode)
                {
                    ScriptDataService.RefreshConstellationEditorDataList();
                    ParseScript();
                }

                if (ScriptDataService.GetEditorData().ExampleData.openExampleConstellation && state == PlayModeStateChange.EnteredPlayMode)
                {
                    var nodeExampleLoader = new ExampleSceneLoader();
                    nodeExampleLoader.RunExample(ScriptDataService.GetEditorData().ExampleData.constellationName, ScriptDataService);
                    ScriptDataService.GetEditorData().ExampleData.openExampleConstellation = false;
                }
            }
        }catch(System.Exception exception)
        {
            UnexpectedError(exception.StackTrace);
        }
    }

    private void OnDestroy()
    {
        EditorApplication.playModeStateChanged -= OnPlayStateChanged;
        UnityEditor.Undo.undoRedoPerformed -= OnUndoPerformed;
    }

    private void OnHelpRequested(string nodeName)
    {
        if (Application.isPlaying)
        {
            if (EditorUtility.DisplayDialog("Exit play mode", "You need to exit play mode in order to open a Constellation help.", "Continue", "Stop Playing"))
                return;

            EditorApplication.isPlaying = false;
            return;
        }
        ScriptDataService.GetEditorData().ExampleData.openExampleConstellation = true;
        ScriptDataService.GetEditorData().ExampleData.constellationName = nodeName;
        EditorApplication.isPlaying = true;
    }

    protected void OnLinkAdded(LinkData link)
    {
        if (Application.isPlaying && previousSelectedGameObject != null && ConstellationScript.IsInstance)
        {
            currentEditableConstellation.AddLink(link);
            ConstellationScript.IsDifferentThanSource = true;
        }
    }

    protected void OnLinkRemoved(LinkData link)
    {
        if (Application.isPlaying && previousSelectedGameObject != null && ConstellationScript.IsInstance)
        {
            currentEditableConstellation.RemoveLink(link);
            ConstellationScript.IsDifferentThanSource = true;
        }
    }

    protected void OnNodeAdded(NodeData node)
    {
        if (node.Name == Constellation.ConstellationTypes.Tutorial.NAME)
        {
            ConstellationScript = ScriptDataService.ConvertCurrentConstellationToTutorial();
        }
        else if (node.Name == Constellation.ConstellationTypes.StaticConstellationNode.NAME)
        {
            ConstellationScript = ScriptDataService.ConvertToConstellationNodeScript();
        }

        if (Application.isPlaying && previousSelectedGameObject != null && ConstellationScript.IsInstance)
        {
            currentEditableConstellation.AddNode(node);
            ConstellationScript.IsDifferentThanSource = true;
        }
    }

    protected void OnNodeRemoved(NodeData node)
    {
        if (node.Name == Constellation.ConstellationTypes.Tutorial.NAME)
        {
            ConstellationScript = ScriptDataService.ConvertToConstellationScript();
        }
        else if (node.Name == Constellation.ConstellationTypes.StaticConstellationNode.NAME)
        {
            ConstellationScript = ScriptDataService.ConvertToConstellationScript();
        }
        if (Application.isPlaying && previousSelectedGameObject != null && ConstellationScript.IsInstance)
        {
            currentEditableConstellation.RemoveNode(node);
            ConstellationScript.IsDifferentThanSource = true;
        }
    }

    private void SetupScriptDataService()
    {
        ScriptDataService = new ConstellationEditorDataService();
        ScriptDataService.Initialize();
        ScriptDataService.ResetConstellationEditorData();
    }

    public bool ParseScript(bool refreshTutorials = false)
    {
        if (ConstellationParser == null)
            ConstellationParser = new ConstellationParser();

        if (ScriptDataService == null)
        {
            SetupScriptDataService();
            if (ScriptDataService.OpenedScripts.ToArray().Length > 0)
                Open(ScriptDataService.OpenedScripts.ToArray()[0]);
        }

        ScriptDataService.RefreshConstellationEditorDataList();
        ScriptDataService.UpdateStaticConstellationNodesNames();
        ConstellationParser.UpdateScriptsNodes(ScriptDataService.GetAllStaticScriptsDataInProject(), ScriptDataService.GetAllScriptDataInProject());


        if (refreshTutorials)
        {
            ConstellationParser.UpdateScriptsNodes(ScriptDataService.GetAllStaticScriptsDataInProject(), ScriptDataService.GetAllTutorialScriptsDataInProject());
        }
        ScriptDataService.SetAllScriptsDirty();
        SetupNodeWindow();
        SetupNodeSelector();

        return true;
    }
}
