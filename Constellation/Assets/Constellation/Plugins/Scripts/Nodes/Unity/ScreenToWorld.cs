using UnityEngine;

namespace Constellation.Unity {
    public class ScreenToWorld : INode, IReceiver {
        private Vector3 movingVector;
        private ISender sender;
        private Vector3 hitPosition;

        private Variable valueX;
        private Variable valueY;
        private Variable valueZ;
        private Variable Result;

        public const string NAME = "ScreenToWorld";

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Position X");
            _nodeParameters.AddInput (this, false, "Position Y");
            _nodeParameters.AddInput (this, false, "Distance");
            _nodeParameters.AddInput (this, true, "Calculate");
            sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (false, "The hit position");

            valueX = new Variable ().Set (0);
            valueY = new Variable ().Set (0);
            valueZ = new Variable ().Set (0);

        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            if (_input.InputId == 0)
                valueX.Set (value.GetFloat ());

            if (_input.InputId == 1)
                valueY.Set (value.GetFloat ());

            if (_input.InputId == 2)
                valueZ.Set (value.GetFloat ());

            if (_input.InputId == 3) {
                hitPosition = Camera.main.ScreenToWorldPoint (new Vector3 (valueX.GetFloat (), valueY.GetFloat (), valueZ.GetFloat ()));
                Variable[] newVar = new Variable[3];
                newVar[0] = new Variable().Set(hitPosition.x);
                newVar[1] = new Variable().Set(hitPosition.y);
                newVar[2] =new Variable().Set(hitPosition.z);
                Result = new Variable ().Set (newVar);
                sender.Send(Result, 0);
            }
        }
    }
}