namespace Constellation.Unity {
    public class FindByName : INode, IReceiver, IAwakable {
        public const string NAME = "FindByName";
        private ISender sender;
        public Attribute GameObjectName;
        public Variable GameObject;

        public void Setup (INodeParameters _nodeParameters, ILogger _logger) {
            var newValue = new Variable ().Set("Name");
            sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (true, "Object", "Gameobject name");
            GameObjectName = _nodeParameters.AddAttribute(newValue, Attribute.AttributeType.Word, "Name");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnAwake () {
            GameObject = new Variable().Set(UnityEngine.GameObject.Find(GameObjectName.Value.GetString()));
            sender.Send(GameObject,0);
        }

        public void Receive (Variable value, Input _input) { }
    }
}