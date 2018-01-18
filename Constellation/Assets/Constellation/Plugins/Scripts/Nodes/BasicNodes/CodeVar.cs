using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Constellation.BasicNodes {
    public class CodeVar : INode, IReceiver {
        private ISender sender;
        private Attribute VarName;
        public const string NAME = "CodeVar";
        private Variable currentObject;
        private object currentReflectedVar;
        private object currentReflectedObject;
        private PropertyInfo property;

        public void Setup (INodeParameters _node, ILogger _logger) {
            var newValue = new Variable ("VarName");
            sender = _node.AddOutput (false, "The value");
            _node.AddInput (this, false, "Object", "Object which contains the var");
            _node.AddInput (this, false, "Set Var");
            _node.AddInput (this, true, "Push var");
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
                currentReflectedVar = _value.GetObject ().GetType ().GetProperty (VarName.Value.GetString ()).GetValue (_value.GetObject (), null);
                currentReflectedObject = _value.GetObject ();
                Type myType = currentReflectedObject.GetType ();
                property = myType.GetProperty (VarName.Value.GetString ());
                GetVarInCurrentObject ();
            }

            if (_input.InputId == 1) {
                SetVarInCurrentObject (_value);
            }

            if (_input.isWarm) {
                sender.Send (currentObject, 0);
            }
        }

        private void SetVarInCurrentObject (Variable variable) {
            property.SetValue (currentReflectedObject, variable.GetObject (), null);
        }

        private void GetVarInCurrentObject () {
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
                    newVar[0] = new Variable ().Set (vec3.x);
                    newVar[1] = new Variable ().Set (vec3.y);
                    newVar[2] = new Variable ().Set (vec3.z);
                    currentObject.Set (newVar);
                } else
                    currentObject.Set (currentObject);

            } catch {
                Debug.LogWarning ("Constellation node: Get var has invalid attribute");
            }
        }
    }
}