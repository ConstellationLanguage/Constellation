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
			if (attribute.AttributeType == BehaviourAttribute.Type.Value) {
				UpdateValueAttribute (attribute, i);
			} else if (attribute.AttributeType == BehaviourAttribute.Type.Word)
				attribute.Variable.Set (EditorGUILayout.TextField (ConstellationComponent.Attributes[i].Name, ConstellationComponent.Attributes[i].Variable.GetString ()));
			else if (attribute.AttributeType == BehaviourAttribute.Type.UnityObject) {
#pragma warning disable 0618
				attribute.UnityObject = (EditorGUILayout.ObjectField (ConstellationComponent.Attributes[i].Name, attribute.UnityObject, typeof (Object)));
#pragma warning restore 0618
			}
		}

		DrawInspectorWarning();
	}
	void UpdateValueAttribute (BehaviourAttribute attribute, int attributeId) {
		var previousVariable = attribute.Variable.GetFloat ();
		attribute.Variable.Set (EditorGUILayout.FloatField (ConstellationComponent.Attributes[attributeId].Name, ConstellationComponent.Attributes[attributeId].Variable.GetFloat ()));
		if (previousVariable != attribute.Variable.GetFloat () && Application.isPlaying) {

		}
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