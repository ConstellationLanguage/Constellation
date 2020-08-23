using UnityEngine;

namespace Constellation.Physics
{
    public class CollisionEnter : INode, IReceiver, ICollisionEnter
    {
        private ISender sender;
        private Ray variable;
        public const string NAME = "CollisionEnter";
        public void Setup(INodeParameters _nodeParameters)
        {
            variable = new Ray().Set(null as object);
            sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput(true, "Object", "Unity collision object");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnCollisionEnter(Collision collision) {
            variable.Set(collision);
            sender.Send(variable, 0);
        }

        public void Receive(Ray _value, Input _input)
        {
            
        }
    }
}
