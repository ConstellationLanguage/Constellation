using UnityEditor;
using UnityEngine;
using Constellation;
using ConstellationEditor;
using Constellation.Unity3D;

public class ConstellationEditorWindow : EditorWindow, ILoadable, ICopyable, ICompilable
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

    //Parsing
    public bool SkipNextScriptsParsing = false;

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
        if (NodeSelector == null)
        {
            NodeSelector = new NodeSelectorPanel();
            NodeSelector.SetupNamespaceData();
        }

        if (ScriptDataService == null)
        {
            ScriptDataService = new ConstellationEditorDataService();
            ScriptDataService.Initialize();
        }

        if (NodeTabPanel == null)
            NodeTabPanel = new ConstellationsTabPanel();
    }

    void NodeAdded(string _nodeName, string _namespace)
    {
        NodeWindow.AddNode(_nodeName, _namespace, OnEditorEvent);
        ScriptDataService.SaveScripts();
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
            TopBarPanel.Draw(this, this, this);
            var constellationName = NodeTabPanel.Draw(ScriptDataService.OpenedScripts.ToArray());
            if (constellationName != null)
                Open(constellationName);
            var constellationToRemove = NodeTabPanel.ConstellationToRemove();
            EditorGUILayout.BeginHorizontal();
            if (NodeWindow == null)
                ParseScript();
            NodeWindow.UpdateSize(position.width - nodeSelectorWidth - splitThickness, position.height - NodeTabPanel.GetHeight());
            NodeWindow.Draw(RequestRepaint, OnEditorEvent, ScriptDataService.GetConstellationEditorConfig(), out nodeWindowSize, out nodeWindowScrollPosition);
            DrawVerticalSplit();
            NodeSelector.Draw(nodeSelectorWidth, position.height, NodeAdded);
            EditorGUILayout.EndHorizontal();
            if (ScriptDataService.CloseOpenedConstellation(constellationToRemove))
            {
                if (ScriptDataService.OpenedScripts.Count > 0)
                {
                    Open(ScriptDataService.OpenedScripts[0]);
                }
            }
            DrawInstancePannel();
        }

        if (requestRepaint)
            Repaint();
    }

    private void DrawInstancePannel()
    {
        if (!ConstellationScript.IsDifferentThanSource || ConstellationScript.GetType() == typeof(ConstellationTutorialScript))
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
        if (Application.isPlaying && IsConstellationSelected())
        {
            if (NodeWindow != null && previousSelectedGameObject != null && ScriptDataService.GetCurrentScript().IsInstance)
            {
                NodeWindow.Update(currentEditableConstellation.GetConstellation(), OnEditorEvent);
            }

            var selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length == 0 || selectedGameObjects[0] == previousSelectedGameObject || selectedGameObjects[0].activeInHierarchy == false)
                return;
            else if (ScriptDataService.GetCurrentScript().IsInstance)
            {
                ScriptDataService.CloseCurrentConstellationInstance();
                Open(ScriptDataService.OpenedScripts.ToArray()[0]);
                previousSelectedGameObject = selectedGameObjects[0];
            }

            var selectedConstellation = selectedGameObjects[0].GetComponent<ConstellationEditable>() as ConstellationEditable;
            if (selectedConstellation != null)
            {
                currentEditableConstellation = selectedConstellation;
                previousSelectedGameObject = selectedGameObjects[0];
                if(selectedConstellation.ConstellationData.GetType() == typeof(ConstellationTutorialScript))
                    OpenConstellationInstance <ConstellationTutorialScript> (selectedConstellation.GetConstellation(), AssetDatabase.GetAssetPath(selectedConstellation.GetConstellationData()));
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
        if (Application.isPlaying && previousSelectedGameObject != null)
        {
            ConstellationScript.IsDifferentThanSource = true;
        }
    }

    void SetupNodeWindow()
    {
        NodeWindow = new NodeWindow(editorPath, ScriptDataService, nodeWindowSize, nodeWindowScrollPosition);
    }

    void RequestRepaint()
    {
        requestRepaint = true;

    }

    public void Open(ConstellationScriptInfos _path)
    {
        ConstellationScript = ScriptDataService.OpenConstellation(_path);
        SetupNodeWindow();
        Save();
    }

    public void Save()
    {
        ScriptDataService.SaveScripts();
    }

    public void New()
    {
        ConstellationScript = ScriptDataService.New();
        SetupNodeWindow();
        RequestRepaint();
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

    public void OpenConstellationInstance <T> (Constellation.Constellation constellation, string path) where T: ConstellationScript
    {
        ScriptDataService.OpenConstellationInstance <T> (constellation, path);
        ConstellationScript = ScriptDataService.Script;
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

    void OnPlayStateChanged(PlayModeStateChange state)
    {
        previousSelectedGameObject = null;
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            ResetInstances();
            ScriptDataService.CloseCurrentConstellationInstance();
            Open(ScriptDataService.OpenedScripts[0]);
        }

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            ScriptDataService.RefreshConstellationEditorDataList();
            SkipNextScriptsParsing = true;
        }

        if (ScriptDataService.GetEditorData().ExampleData.openExampleConstellation && state == PlayModeStateChange.EnteredPlayMode)
        {
            var nodeExampleLoader = new ExampleSceneLoader();
            nodeExampleLoader.RunExample(ScriptDataService.GetEditorData().ExampleData.constellationName, ScriptDataService);
            ScriptDataService.GetEditorData().ExampleData.openExampleConstellation = false;
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
        if (Application.isPlaying && previousSelectedGameObject != null)
        {
            currentEditableConstellation.AddLink(link);
            ConstellationScript.IsDifferentThanSource = true;
        }
    }

    protected void OnLinkRemoved(LinkData link)
    {
        if (Application.isPlaying && previousSelectedGameObject != null)
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
        if (Application.isPlaying && previousSelectedGameObject != null)
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
        if (Application.isPlaying && previousSelectedGameObject != null)
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

    public bool ParseScript()
    {
        if (ConstellationParser == null)
            ConstellationParser = new ConstellationParser();

        if (ScriptDataService == null)
        {
            SetupScriptDataService();
            if (ScriptDataService.OpenedScripts.ToArray().Length > 0)
                Open(ScriptDataService.OpenedScripts.ToArray()[0]);
        }

        if (SkipNextScriptsParsing)
        {
            SkipNextScriptsParsing = false;
            SetupNodeWindow();
            return true;
        }
        ScriptDataService.RefreshConstellationEditorDataList();
        ConstellationParser.UpdateScriptsNodes(ScriptDataService.GetAllScriptDataInProject(), ScriptDataService.GetAllNestableScriptsInProject());
        ScriptDataService.SetAllScriptsDirty();
        SetupNodeWindow();

        return true;
    }
}
