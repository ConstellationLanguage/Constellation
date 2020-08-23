using UnityEngine;

namespace Constellation.Physics {
    public class AddForce: RigidBodyUtils, INode {
        public const string NAME = "AddForce";

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
            rigidBody.AddForce(force);
        }
    }
}