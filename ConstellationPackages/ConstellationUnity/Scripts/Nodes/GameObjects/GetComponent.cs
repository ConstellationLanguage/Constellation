using UnityEngine;
namespace Constellation.GameObjects
{
    public class GetComponent : INode, IReceiver, IRequireGameObject {
        private ISender sender;
        private Parameter ComponentName;
        public const string NAME = "GetComponent";
        private GameObject gameObject;
        private Ray componentObject;

        public void Setup (INodeParameters _node) {
            var newValue = new Ray ("ComponentName");
            sender = _node.GetSender();
            _node.AddOutput (false, "Object", "The Component");
            _node.AddInput (this, true, "Object", "Object which contain the component");
            _node.AddInput (this, true, "Send the component");
            ComponentName = _node.AddParameter (newValue, Parameter.ParameterType.Word, "ComponentName");
            componentObject = new Ray (null as object);
        }

        public void Set (GameObject _gameObject) {
            gameObject = _gameObject;

        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray _value, Input _input) {
            if (_input.InputId == 0) {
                gameObject = UnityObjectsConvertions.ConvertToGameObject (_value.GetObject ());
            }

            if (_input.isBright) {
                var component = gameObject.GetComponent (ComponentName.Value.GetString ());
                componentObject.Set (component);
                sender.Send (componentObject, 0);
            }
        }
    }
}