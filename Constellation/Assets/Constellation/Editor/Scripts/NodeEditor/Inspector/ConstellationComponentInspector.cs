using Constellation;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (ConstellationComponent))]
public class ConstellationComponentInpector : Editor {
	private Object source;
	ConstellationComponent ConstellationComponent;

	public override void OnInspectorGUI () {
		ConstellationComponent = (ConstellationComponent) target;

		if (ConstellationComponent == null)
			return;

		if (ConstellationComponent.GetConstellationData() != null)
			source = ConstellationComponent.GetConstellationData();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Script", GUILayout.MaxWidth (100));
		source = EditorGUILayout.ObjectField (source, typeof (ConstellationScript), true);
		ConstellationComponent.SetConstellationScript(source as ConstellationScript);
		NodeData[] nodes = null;
		if (ConstellationComponent.GetConstellationData() != null) {
			nodes = ConstellationComponent.GetConstellationData().GetNodes ();
			ConstellationComponent.UpdateAttributes (nodes);
		}
		EditorGUILayout.EndHorizontal ();

		if (ConstellationComponent.Attributes == null)
			return;
		for (var i = 0; i < ConstellationComponent.Attributes.Count; i++) {
			var attribute = ConstellationComponent.Attributes[i];
            if (attribute.AttributeType == BehaviourAttribute.Type.Value)
                UpdateValueAttribute(attribute, i);
            else if (attribute.AttributeType == BehaviourAttribute.Type.Word)
                UpdateWordAttribute(attribute, i);
            else if (attribute.AttributeType == BehaviourAttribute.Type.UnityObject)
            {
                UpdateObjectAttribute(attribute, i);

            }
		}

		DrawInspectorWarning();
	}
	void UpdateValueAttribute (BehaviourAttribute attribute, int attributeId) {
        var newFloat = EditorGUILayout.FloatField(ConstellationComponent.Attributes[attributeId].Name, ConstellationComponent.Attributes[attributeId].Variable.GetFloat());
        if (newFloat != attribute.Variable.GetFloat()) {
            attribute.Variable.Set(newFloat);
            if (ConstellationComponent.constellation != null) {
                Node<INode> nodeToUpdate = ConstellationComponent.constellation.GetNodeByGUID(attribute.NodeGUID);
                if (nodeToUpdate != null)
                    nodeToUpdate.Receive(attribute.Variable, null);
            }
        }
	}

    void UpdateWordAttribute(BehaviourAttribute attribute, int attributeId)
    {
        var newString = EditorGUILayout.TextField(ConstellationComponent.Attributes[attributeId].Name, ConstellationComponent.Attributes[attributeId].Variable.GetString());
        if (newString != attribute.Variable.GetString())
        {
            attribute.Variable.Set(newString);
            if (ConstellationComponent.constellation != null)
            {
                Node<INode> nodeToUpdate = ConstellationComponent.constellation.GetNodeByGUID(attribute.NodeGUID);
                if (nodeToUpdate != null)
                    nodeToUpdate.Receive(attribute.Variable, null);
            }
        }
    }

    void UpdateObjectAttribute(BehaviourAttribute attribute, int attributeId)
    {
#pragma warning disable 0618
        Object newObject = (EditorGUILayout.ObjectField(ConstellationComponent.Attributes[attributeId].Name, attribute.UnityObject, typeof(Object)));

#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
        if (newObject != attribute.Variable.GetObject())
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
        {
            attribute.UnityObject = newObject;
            attribute.Variable.Set(newObject);
            if (ConstellationComponent.constellation != null)
            {
                Node<INode> nodeToUpdate = ConstellationComponent.constellation.GetNodeByGUID(attribute.NodeGUID);
                if (nodeToUpdate != null)
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