using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constellation.Physics
{
    public abstract class RigidBodyUtils: IReceiver, IRequireGameObject, IFixedUpdate
    {
        protected Rigidbody rigidBody;
        protected GameObject gameObject;
        protected Vector3 force;
        protected bool requestedForce = false;
        public void Setup(INodeParameters _nodeParameters)
        {
            _nodeParameters.AddInput(this, false, "Object", "The game object affected");
            _nodeParameters.AddInput(this, false, "Vec3", "Vec3 world relative");
            force = Vector3.zero;
        }

        public void Set(GameObject _gameObject)
        {
            gameObject = _gameObject;
        }

        public void OnFixedUpdate()
        {
            if (requestedForce)
            {
                if (rigidBody == null)
                {
                    SetRigidBody();
                }
                UpdatePhysics();
                requestedForce = false;
            }
        }

        public void Receive(Ray value, Input _input)
        {
            if (_input.InputId == 1)
            {
                if (!requestedForce)
                {
                    force = UnityObjectsConvertions.ConvertToVector3(value);
                    requestedForce = true;
                }
            }
            else if (_input.InputId == 0)
            {
                var obj = UnityObjectsConvertions.ConvertToGameObject(value.GetObject());
                if (obj is GameObject)
                {
                    Set(obj);
                    SetRigidBody();
                }
            }
        }

        protected abstract void UpdatePhysics();

        public abstract string NodeName();
        public abstract string NodeNamespace();

        protected void SetRigidBody()
        {
            rigidBody = gameObject.GetComponent<Rigidbody>() as Rigidbody;
            if (rigidBody == null)
                rigidBody = gameObject.AddComponent<Rigidbody>();
        }
    }
}
