using UnityEngine;

namespace Constellation.Vectors
{
    public class Forward : INode, IReceiver, IRequireGameObject
    {
        protected GameObject gameObject;
        protected Vector3 direction;
        protected ISender sender;
        public const string NAME = "Forward";
        public void Setup(INodeParameters _nodeParameters)
        {
            _nodeParameters.AddInput(this, false, "Object", "The game object reference");
            _nodeParameters.AddInput(this, true, "Vec3", "Vec3 to convert");
            _nodeParameters.AddOutput(false, "Vec3", "The converted Vec3");
            sender = _nodeParameters.GetSender();
        }

        public void Set(GameObject _gameObject)
        {
            gameObject = _gameObject;
        }

        public void Receive(Ray value, Input _input)
        {
            if (_input.InputId == 1)
            {
                direction = gameObject.transform.TransformDirection(UnityObjectsConvertions.ConvertToVector3(value));
            }
            else if (_input.InputId == 0)
            {
                var obj = UnityObjectsConvertions.ConvertToGameObject(value.GetObject());
                if (obj is GameObject)
                {
                    Set(obj);
                }
            }

            if(_input.isBright)
            {
                Ray[] newVar = new Ray[3];
                newVar[0] = new Ray(direction.x);
                newVar[1] = new Ray(direction.y);
                newVar[2] = new Ray(direction.z);
                var Result = new Ray().Set(newVar);
                sender.Send(Result, 0);
            }
        }

        public string NodeName()
        {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }
    }
}
