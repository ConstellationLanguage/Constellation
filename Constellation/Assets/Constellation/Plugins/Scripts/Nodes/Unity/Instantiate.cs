using UnityEngine;

namespace Constellation.Unity
{
    public class Instantiate : INode, IReceiver
    {
        public const string NAME = "Instantiate";
        private ISender sender;
        private Variable UnityObject;

        public void Setup(INodeParameters _node)
        {
			_node.AddInput(this, true, "Object", "GameObject");
            sender = _node.GetSender();
            _node.AddOutput(true, "Object", "Instantiated GameObject");
            UnityObject = new Variable().Set(null as object);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive(Variable _value, Input _input)
        {
            UnityObject.Set(_value.GetObject());
            sender.Send(new Variable().Set(GameObject.Instantiate(_value.GetObject() as GameObject)), 0);
        }
    }
}