using UnityEngine;

namespace Constellation {
	[System.Serializable]
	public class ConstellationParameter {
		public Ray Variable;
		public string Name;
		public enum Type { Value, Word, UnityObject };
		public Object UnityObject;
        public string NodeGUID;

		public Type AttributeType;
		public ConstellationParameter (Ray _variable, string _name, Type _type, string _guid) {
			Variable = _variable;
			Name = _name;
			AttributeType = _type;
            NodeGUID = _guid;
		}

        public void SetVariableAndUpdate(Ray variable)
        {
            Variable.Set(variable);
        }
	}
}