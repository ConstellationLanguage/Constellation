using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constellation.Physics
{
    public class AddImpulseTorque : RigidBodyUtils, INode
    {
        public const string NAME = "AddImpulseTorque";
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
            rigidBody.AddRelativeTorque(force, ForceMode.Impulse);
        }
    }
}
