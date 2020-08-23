using UnityEngine;

namespace Constellation.Physics {
    public class AddImpulseForce : RigidBodyUtils, INode {
        public const string NAME = "AddImpulseForce";
        public override string NodeName()
        {
            return NAME;
        }

        public override string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        protected override void UpdatePhysics()
        {
            rigidBody.AddForce(force, ForceMode.Impulse);
        }
    }
}