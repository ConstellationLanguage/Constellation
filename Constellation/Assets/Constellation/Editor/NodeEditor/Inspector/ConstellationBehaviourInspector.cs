using Constellation;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (ConstellationBehaviour))]
public class ConstellationBehaviourInspector : Editor {
	private Object source;
	ConstellationBehaviour ConstellationBehaviour;

	public override void OnInspectorGUI () {
		ConstellationBehaviour = (ConstellationBehaviour) target;

		if (ConstellationBehaviour == null)
			return;

		if (ConstellationBehaviour.ConstellationData != null)
			source = ConstellationBehaviour.ConstellationData;

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Script", GUILayout.MaxWidth (100));
		source = EditorGUILayout.ObjectField (source, typeof (ConstellationScript), true);
		ConstellationBehaviour.ConstellationData = source as ConstellationScript;
		NodeData[] nodes = null;
		if (ConstellationBehaviour.ConstellationData != null) {
			nodes = ConstellationBehaviour.ConstellationData.GetNodes ();
			ConstellationBehaviour.UpdateAttributes (nodes);
		}
		EditorGUILayout.EndHorizontal ();

		if (ConstellationBehaviour.Attributes == null)
			return;
		for (var i = 0; i < ConstellationBehaviour.Attributes.Count; i++) {
			var attribute = ConstellationBehaviour.Attributes[i];
			if (attribute.AttributeType == BehaviourAttribute.Type.Value) {
				UpdateValueAttribute(attribute, i);
			} else if (attribute.AttributeType == BehaviourAttribute.Type.Word)
				attribute.Variable.Set (EditorGUILayout.TextField (ConstellationBehaviour.Attributes[i].Name, ConstellationBehaviour.Attributes[i].Variable.GetString ()));
			else if (attribute.AttributeType == BehaviourAttribute.Type.UnityObject) {
#pragma warning disable 0618
				attribute.UnityObject = (EditorGUILayout.ObjectField (ConstellationBehaviour.Attributes[i].Name, attribute.UnityObject, typeof (Object)));
#pragma warning restore 0618

			}
		}
	}
	void UpdateValueAttribute (BehaviourAttribute attribute, int attributeId) {
		var previousVariable = attribute.Variable.GetFloat();
		attribute.Variable.Set (EditorGUILayout.FloatField (ConstellationBehaviour.Attributes[attributeId].Name, ConstellationBehaviour.Attributes[attributeId].Variable.GetFloat ()));
		if(previousVariable != attribute.Variable.GetFloat() && Application.isPlaying){
			
		}
	}
}