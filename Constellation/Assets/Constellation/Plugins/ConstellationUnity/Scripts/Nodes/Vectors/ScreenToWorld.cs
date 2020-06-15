using UnityEngine;

namespace Constellation.Vectors {
    public class ScreenToWorld : INode, IReceiver {
        private Vector3 movingVector;
        private ISender sender;
        private Vector3 hitPosition;

        private Ray valueX;
        private Ray valueY;
        private Ray valueZ;
        private Ray Result;

        public const string NAME = "ScreenToWorld";

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Position X");
            _nodeParameters.AddInput (this, false, "Position Y");
            _nodeParameters.AddInput (this, false, "Distance");
            _nodeParameters.AddInput (this, true, "Any", "Calculate");
            sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (false,"Vec3", "The hit position");

            valueX = new Ray ().Set (0);
            valueY = new Ray ().Set (0);
            valueZ = new Ray ().Set (0);

        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray value, Input _input) {
            if (_input.InputId == 0)
                valueX.Set (value.GetFloat ());

            if (_input.InputId == 1)
                valueY.Set (value.GetFloat ());

            if (_input.InputId == 2)
                valueZ.Set (value.GetFloat ());

            if (_input.InputId == 3) {
                hitPosition = Camera.main.ScreenToWorldPoint (new Vector3 (valueX.GetFloat (), valueY.GetFloat (), valueZ.GetFloat ()));
                Ray[] newVar = new Ray[3];
                newVar[0] = new Ray().Set(hitPosition.x);
                newVar[1] = new Ray().Set(hitPosition.y);
                newVar[2] =new Ray().Set(hitPosition.z);
                Result = new Ray ().Set (newVar);
                sender.Send(Result, 0);
            }
        }
    }
}