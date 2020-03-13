using UnityEditor;
using UnityEngine;
using Constellation;
using ConstellationEditor;

public class ConstellationEditorWindowV2 : EditorWindow, ILoadable, IUndoable, ICopyable, ICompilable
{
    public NodeWindow NodeWindow;
    public NodeSelectorPanel NodeSelector;
    public ConstellationsTabPanel NodeTabPanel;
    private const string editorPath = "Assets/Constellation/Editor/EditorAssets/";
    public NodesFactory NodesFactory;
    public ConstellationEditorDataService ScriptDataService;
    public ConstellationScript ConstellationScript;
    public float nodeSelectorWidth = 300;
    public ConstellationLinter ConstellationCompiler;
    const float splitThickness = 3;

    //Runtime
    public GameObject previousSelectedGameObject;
    ConstellationEditable currentEditableConstellation;


    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ConstellationEditorWindowV2 window = (ConstellationEditorWindowV2)EditorWindow.GetWindow(typeof(ConstellationEditorWindowV2));
        window.Show();
    }

    public void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayStateChanged;
        if (NodeSelector == null)
        {
            NodeSelector = new NodeSelectorPanel(/*, scriptDataService.GetAllCustomNodesNames()*/);
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
        } else
        {
            TopBarPanel.Draw(this, this, this, this);
            var constellationName = NodeTabPanel.Draw(ScriptDataService.currentPath.ToArray(), null);
            if (constellationName != null)
                Open(constellationName);
            var constellationToRemove = NodeTabPanel.ConstellationToRemove();
            EditorGUILayout.BeginHorizontal();
            if (NodeWindow == null)
                SetupNodeWindow();
            NodeWindow.UpdateSize(position.width - nodeSelectorWidth - splitThickness, position.height - NodeTabPanel.GetHeight());
            NodeWindow.Draw(RequestRepaint, OnEditorEvent);
            DrawVerticalSplit();
            NodeSelector.Draw(nodeSelectorWidth, position.height, NodeAdded);
            EditorGUILayout.EndHorizontal();
            if (ScriptDataService.CloseOpenedConstellation(constellationToRemove))
            {
                if (ScriptDataService.currentPath.Count > 0)
                {
                    Open(ScriptDataService.currentPath[0]);
                }else
                {

                }
            }
        }
    }

    void Update()
    {
        if (Application.isPlaying && IsConstellationSelected())
        {
            RequestRepaint();
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
                previousSelectedGameObject = selectedGameObjects[0];
                //RequestSetup();
            }

            var selectedConstellation = selectedGameObjects[0].GetComponent<ConstellationEditable>() as ConstellationEditable;
            if (selectedConstellation != null)
            {
                currentEditableConstellation = selectedConstellation;
                previousSelectedGameObject = selectedGameObjects[0];
                OpenConstellationInstance(selectedConstellation.GetConstellation(), AssetDatabase.GetAssetPath(selectedConstellation.GetConstellationData()));
                if (selectedConstellation.GetConstellation() == null)
                {
                    return;
                }
                selectedConstellation.Initialize();
            }

            /*if (requestSetup == true)
            {
                Setup();
                requestSetup = false;
            }

            if (requestCompilation == true)
            {
                ParseScript();
                requestCompilation = false;
            }*/
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
        if(eventType == ConstellationEditorEvents.EditorEventType.HelpClicked)
        {
            OnHelpRequested(eventMessage);
        }
        if(eventType == ConstellationEditorEvents.EditorEventType.LinkAdded)
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
    }

    void SetupNodeWindow()
    {
        NodeWindow = new NodeWindow(editorPath, ConstellationScript);
    }

    void RequestRepaint()
    {
        Repaint();
    }

    public void Open(string _path)
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

    public void OpenConstellationInstance(Constellation.Constellation constellation, string path)
    {
        ScriptDataService.OpenConstellationInstance(constellation, path);
        ConstellationScript = ScriptDataService.Script;
        SetupNodeWindow();
    }

    public void AddAction()
    {
        throw new System.NotImplementedException();
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public void Redo()
    {
        throw new System.NotImplementedException();
    }

    public void Copy()
    {
        throw new System.NotImplementedException();
    }

    public void Paste()
    {
        throw new System.NotImplementedException();
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

    void OnPlayStateChanged(PlayModeStateChange state)
    {
        /*WindowInstance.RequestSetup();
        WindowInstance.previousSelectedGameObject = null;*/
        previousSelectedGameObject = null;
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            ResetInstances();
            Open(ScriptDataService.currentPath[0]);
        }
        if (ScriptDataService.GetEditorData().ExampleData.openExampleConstellation && state == PlayModeStateChange.EnteredPlayMode)
        {
            var nodeExampleLoader = new ExampleSceneLoader();
            nodeExampleLoader.RunExample(ScriptDataService.GetEditorData().ExampleData.constellationName, ScriptDataService);
            ScriptDataService.GetEditorData().ExampleData.openExampleConstellation = false;
        }

        /*EditorApplication.playModeStateChanged -= OnPlayStateChanged;
        WindowInstance.RequestRepaint();*/
    }

    private void OnDestroy()
    {
        EditorApplication.playModeStateChanged -= OnPlayStateChanged;
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
        if (Application.isPlaying && previousSelectedGameObject != null)
            currentEditableConstellation.RemoveNode(node);
    }

    public void ParseScript()
    {
        if (ConstellationCompiler == null)
            ConstellationCompiler = new ConstellationLinter();

        if (ScriptDataService == null)
        {
            ScriptDataService = new ConstellationEditorDataService();
        }
        //ResetWindow();
        ConstellationCompiler.UpdateScriptsNodes(ScriptDataService.GetAllScriptsInProject(), ScriptDataService.GetAllNestableScriptsInProject());
    }
}
