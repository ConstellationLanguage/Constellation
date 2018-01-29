namespace Constellation.Physics {
    public class PhysicsNodeFactory : INodeGetter {
        public Node<INode> GetNode (string nodeName) {
            switch (nodeName) {
                case AddForce.NAME:
                    INode nodeAddForce = new AddForce () as INode;
                    return new Node<INode> (nodeAddForce);
                case AddTorque.NAME:
                    INode nodeAddTorque = new AddTorque () as INode;
                    return new Node<INode> (nodeAddTorque);
                case RigidBody.NAME:
                    INode nodeRigidBody = new RigidBody () as INode;
                    return new Node<INode> (nodeRigidBody);
                case Velocity.NAME:
                    INode nodeVelocity = new Velocity () as INode;
                    return new Node<INode> (nodeVelocity);
                case CollisionEnter.NAME:
                    INode nodeCollisionEnter = new CollisionEnter () as INode;
                    return new Node<INode> (nodeCollisionEnter);
                case CollisionStay.NAME:
                    INode nodeCollisionStay = new CollisionEnter () as INode;
                    return new Node<INode> (nodeCollisionStay);
                case CollisionExit.NAME:
                    INode nodeCollisionExit = new CollisionEnter () as INode;
                    return new Node<INode> (nodeCollisionExit);
                case FixedUpdate.NAME:
                    INode nodeFixedUpdate = new FixedUpdate () as INode;
                    return new Node<INode> (nodeFixedUpdate);
                case AddImpulse.NAME:
                    INode nodeImpulse = new AddImpulse () as INode;
                    return new Node<INode> (nodeImpulse);
                case Character.NAME:
                    var nodeCharacter = new Character () as INode;
                    return new Node<INode> (nodeCharacter);
                case CharacterForward.NAME:
                    var nodeCharacterForward = new CharacterForward() as INode;
                    return new Node<INode> (nodeCharacterForward);
                case CameraRaycast.NAME:
                    var nodeCameraRayCast = new CameraRaycast() as INode;
                    return new Node<INode> (nodeCameraRayCast);
                default:
                    return null;
            }
        }
    }
}