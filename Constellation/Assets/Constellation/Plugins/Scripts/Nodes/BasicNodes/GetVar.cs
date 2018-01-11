using UnityEngine;
namespace Constellation.BasicNodes {
    public class GetVar : INode, IReceiver {
        private ISender sender;
        private Attribute VarName;
        public const string NAME = "GetVar";
        private Variable currentObject;
        private object currentReflectedVar;

        public void Setup (INodeParameters _node, ILogger _logger) {
            var newValue = new Variable ("VarName");
            sender = _node.AddOutput (false, "The value");
            _node.AddInput (this, true, "Object", "Object which contains the var");
            VarName = _node.AddAttribute (newValue, Attribute.AttributeType.Word, "VarName");
            currentObject = new Variable ();
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable _value, Input _input) {
            if (_input.InputId == 0) {
                SetCurrentObject ();
                currentReflectedVar = _value.GetObject ().GetType ().GetProperty (VarName.Value.GetString ()).GetValue (_value.GetObject (), null);
            }

            if (_input.isWarm) {
                sender.Send (currentObject, 0);
            }
        }

        private void SetCurrentObject () {
            try {
                if (currentReflectedVar is float)
                    currentObject.Set ((float) currentReflectedVar);
                else if (currentReflectedVar is int)
                    currentObject.Set ((float) currentReflectedVar);
                else if (currentReflectedVar is string)
                    currentObject.Set ((string) currentReflectedVar);
                else if (currentReflectedVar is bool) {
                    var boolean = (bool) currentReflectedVar;
                    if (boolean == true)
                        currentObject.Set (1);
                    else
                        currentObject.Set (0);
                } else if (currentReflectedVar is Vector3) {
                    var vec3 = (Vector3) currentReflectedVar;
                    Variable[] newVar = new Variable[3];
                    newVar[0] = new Variable().Set(vec3.x);
                    newVar[1] = new Variable().Set(vec3.y);
                    newVar[2] = new Variable().Set(vec3.z);
                    currentObject.Set (newVar);
                } else
                    currentObject.Set (currentObject);

            } catch {
                Debug.LogWarning ("Constellation node: Get var has invalid attribute");
            }
        }
    }
}