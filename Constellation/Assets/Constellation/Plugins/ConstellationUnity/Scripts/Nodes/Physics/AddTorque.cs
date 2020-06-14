using UnityEngine;

namespace Constellation.Physics {
    public class AddTorque : RigidBodyUtils, INode {

        public const string NAME = "AddTorque";

        public override string NodeName()
        {
            return NAME;
        }

        public override string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        protected override void UpdatePhysics() {
            rigidBody.AddTorque(force, ForceMode.Impulse);
        }
    }
}