namespace Constellation.Unity {
    public class MousePosition : INode, IReceiver, IUpdatable {
        public const string NAME = "MousePosition";
        public Transform transform;
        private ISender Sender;
        private ISender YPositionSender;
        private Variable XPosition;
        private Variable YPosition;
        private Variable keyState;
        public void Setup (INodeParameters _nodeParameters) {
            Sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (true, "Mouse position X");
            _nodeParameters.AddOutput (true, "Mosue position Y");
            XPosition = new Variable(UnityEngine.Input.mousePosition.x);
            YPosition = new Variable(UnityEngine.Input.mousePosition.y);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnUpdate () {
            Sender.Send (XPosition.Set (UnityEngine.Input.mousePosition.x), 0);
            Sender.Send (YPosition.Set (UnityEngine.Input.mousePosition.y), 1);
        }

        public void Receive (Variable value, Input _input) { }
    }
}