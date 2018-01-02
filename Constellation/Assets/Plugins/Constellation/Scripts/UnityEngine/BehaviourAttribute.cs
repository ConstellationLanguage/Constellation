using UnityEngine;

namespace Constellation {
	[System.Serializable]
	public class BehaviourAttribute {
		public Variable Variable;
		public string Name;
		public enum Type { Value, Word, UnityObject };
		public Object UnityObject;

		public Type AttributeType;
		public BehaviourAttribute (Variable _variable, string _name, Type _type) {
			Variable = _variable;
			Name = _name;
			AttributeType = _type;
		}
	}
}