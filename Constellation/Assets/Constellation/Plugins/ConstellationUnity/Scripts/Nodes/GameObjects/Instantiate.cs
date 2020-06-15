using UnityEngine;

namespace Constellation.GameObjects
{
    public class Instantiate : INode, IReceiver
    {
        public const string NAME = "Instantiate";
        private ISender sender;
        private Ray UnityObject;

        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "Object", "GameObject");
            sender = _node.GetSender();
            _node.AddOutput(true, "Object", "Instantiated GameObject");
            UnityObject = new Ray().Set(null as object);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Ray _value, Input _input)
        {
            UnityObject.Set(_value.GetObject());
            sender.Send(new Ray().Set(GameObject.Instantiate(_value.GetObject() as GameObject)), 0);
        }
    }
}