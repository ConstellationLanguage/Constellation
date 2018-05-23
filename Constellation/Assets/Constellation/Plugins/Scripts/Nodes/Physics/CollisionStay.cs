using UnityEngine;

namespace Constellation.Physics
{
    public class CollisionStay : INode, IReceiver, ICollisionStay
    {
        private ISender sender;
        private Variable variable;
        public const string NAME = "CollisionStay";
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

        public void OnCollisionStay(Collision collision) {
            variable.Set(collision);
            sender.Send(variable, 0);
        }

        public void Receive(Variable _value, Input _input)
        {
            
        }
    }
}
