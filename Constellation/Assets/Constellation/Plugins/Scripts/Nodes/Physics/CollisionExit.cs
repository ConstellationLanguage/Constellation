using UnityEngine;

namespace Constellation.Physics
{
    public class CollisionExit : INode, IReceiver, ICollisionExit
    {
        private ISender sender;
        private Variable variable;
        public const string NAME = "CollisionExit";
        public void Setup(INodeParameters _nodeParameters)
        {
            variable = new Variable().Set(null as object);
            sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "Object", "Unity collision object");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnCollisionExit(Collision collision) {
            variable.Set(collision);
            sender.Send(variable, 0);
        }

        public void Receive(Variable _value, Input _input)
        {
            
        }
    }
}
