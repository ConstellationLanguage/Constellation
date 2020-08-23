using Constellation;
using Constellation.Unity3D;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor (typeof (ConstellationComponent))]
public class ConstellationComponentInpector : Editor {
	private Object source;
    private bool isSetup = false;
	ConstellationComponent ConstellationComponent;

    public override void OnInspectorGUI () {
		ConstellationComponent = (ConstellationComponent) target;

		if (ConstellationComponent == null)
			return;

		if (ConstellationComponent.GetConstellationData() != null)
			source = ConstellationComponent.GetConstellationData();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Script", GUILayout.MaxWidth (100));

        var currentSource = source;
		source = EditorGUILayout.ObjectField (source, typeof (ConstellationBehaviourScript), true);

        if(currentSource != source && !Application.isPlaying)
        {
            var scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }

		ConstellationComponent.SetConstellationScript(source as ConstellationScript);
		NodeData[] nodes = null;
		if (ConstellationComponent.GetConstellationData() != null) {
			nodes = ConstellationComponent.GetConstellationData().GetNodes ();
			ConstellationComponent.UpdateParameters (nodes);
		}
		EditorGUILayout.EndHorizontal ();

		if (ConstellationComponent.Parameters == null)
			return;
		for (var i = 0; i < ConstellationComponent.Parameters.Count; i++) {
			var attribute = ConstellationComponent.Parameters[i];
            if (attribute.AttributeType == ConstellationParameter.Type.Value)
                UpdateValueAttribute(attribute, i);
            else if (attribute.AttributeType == ConstellationParameter.Type.Word)
                UpdateWordAttribute(attribute, i);
            else if (attribute.AttributeType == ConstellationParameter.Type.UnityObject)
            {
                UpdateObjectAttribute(attribute, i);

            }
		}

		DrawInspectorWarning();
        isSetup = true;

    }
	void UpdateValueAttribute (ConstellationParameter attribute, int attributeId) {
        var newFloat = EditorGUILayout.FloatField(ConstellationComponent.Parameters[attributeId].Name, ConstellationComponent.Parameters[attributeId].Variable.GetFloat());
        if (newFloat != attribute.Variable.GetFloat()) {
            if (!Application.isPlaying)
            {
                var scene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
            }
            attribute.Variable.Set(newFloat);
            if (ConstellationComponent.constellation != null) {
                Node<INode> nodeToUpdate = ConstellationComponent.constellation.GetNodeByGUID(attribute.NodeGUID);
                if (nodeToUpdate != null && isSetup)
                    nodeToUpdate.Receive(attribute.Variable, null);
            }
        }
	}

    void UpdateWordAttribute(ConstellationParameter attribute, int attributeId)
    {
        var newString = EditorGUILayout.TextField(ConstellationComponent.Parameters[attributeId].Name, ConstellationComponent.Parameters[attributeId].Variable.GetString());
        if (newString != attribute.Variable.GetString())
        {
            if (!Application.isPlaying)
            {
                var scene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
            }
            attribute.Variable.Set(newString);
            if (ConstellationComponent.constellation != null)
            {
                Node<INode> nodeToUpdate = ConstellationComponent.constellation.GetNodeByGUID(attribute.NodeGUID);
                if (nodeToUpdate != null && isSetup)
                    nodeToUpdate.Receive(attribute.Variable, null);
            }
        }
    }

    void UpdateObjectAttribute(ConstellationParameter attribute, int attributeId)
    {
#pragma warning disable 0618
        Object newObject = (EditorGUILayout.ObjectField(ConstellationComponent.Parameters[attributeId].Name, attribute.UnityObject, typeof(Object)));

#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
        if (newObject != attribute.Variable.GetObject())
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
        {
            if (!Application.isPlaying)
            {
                var scene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
            }
            attribute.UnityObject = newObject;
            attribute.Variable.Set(newObject);
            if (ConstellationComponent.constellation != null)
            {
                Node<INode> nodeToUpdate = ConstellationComponent.constellation.GetNodeByGUID(attribute.NodeGUID);
                if (nodeToUpdate != null && isSetup)
                    nodeToUpdate.Receive(attribute.Variable, null);
            }
        }
#pragma warning restore 0618
    }

    protected virtual void DrawInspectorWarning()
	{
		if(ConstellationComponent.GetConstellationData() == null && ConstellationComponent.isActiveAndEnabled && Application.isPlaying == false)
			EditorGUILayout.HelpBox("No constellation script is attached. You need to add one or disable this component otherwise you will have an error at runtime", MessageType.Warning);

		if(ConstellationComponent.GetConstellationData() == null && !ConstellationComponent.isActiveAndEnabled && Application.isPlaying == false)
			EditorGUILayout.HelpBox("No constellation script attached. This will trigger an error if you enable the component before attaching a constellation.", MessageType.Info);

		if(ConstellationComponent.GetLastError() != null)
			EditorGUILayout.HelpBox(ConstellationComponent.GetLastError().GetError().GetFormatedError(), MessageType.Error);

	}
}