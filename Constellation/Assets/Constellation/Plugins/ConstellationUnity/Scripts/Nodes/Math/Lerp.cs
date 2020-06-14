using UnityEngine;

namespace Constellation.Math {
    public class Lerp : INode, IReceiver {
        private ISender sender;
        private Ray startValue;
        private Ray endValue;
        public const string NAME = "Lerp";
        public void Setup (INodeParameters _node) {
            _node.AddInput (this, false, "A");
            _node.AddInput (this, false, "B");
            _node.AddInput (this, true, "T");
            sender = _node.GetSender();
            _node.AddOutput (false, "T=0 output A, T=1 output B");
            startValue = new Ray ().Set (0);
            endValue = new Ray ().Set (0);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray _value, Input _input) {
            if (_input.InputId == 0)
                startValue.Set (_value);
            else if (_input.InputId == 1)
                endValue.Set (_value);

            try {
            if (_input.isBright && startValue != null && endValue != null) {
                if (startValue.GetArray () == null)
                    sender.Send (new Ray ().Set (Mathf.Lerp (startValue.GetFloat (), endValue.GetFloat (), _value.GetFloat ())), 0);
                else if(startValue.GetArray() != null && endValue.GetArray() != null){
                    Ray[] newVars = new Ray[startValue.GetArray ().Length];
                    for (var i = 0; i < startValue.GetArray ().Length; i++) {
                        newVars[i] = new Ray (Mathf.Lerp (startValue.GetArrayVariable (i).GetFloat (), endValue.GetArrayVariable (i).GetFloat (), _value.GetFloat ()));
                    }
                    var Result = new Ray ().Set (newVars);
                    sender.Send (Result, 0);
                }
            }
            } catch {
                Debug.LogWarning("A and B are either not setted or they do not match");
            }
        }
    }
}