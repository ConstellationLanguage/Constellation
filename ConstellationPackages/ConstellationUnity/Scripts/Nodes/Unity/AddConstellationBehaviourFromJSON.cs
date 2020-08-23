
using UnityEngine;
using Constellation.Unity3D;

namespace Constellation.Unity
{
    public class AddConstellationBehaviourFromJSON : INode, IReceiver, IRequireGameObject

    {
        public const string NAME = "AddConstellationBehaviourFromJSON";
        private ISender sender;
        private GameObject gameObject;
        private Ray GameObject;

        public void Setup(INodeParameters _node)
        {
            sender = _node.GetSender();
            _node.AddInput(this, false, "Object", "Constellation JSON");
            _node.AddInput(this, false, "Const JSON");
            GameObject = new Ray().Set(null as object);
        }

        public string NodeName()
        {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Receive(Ray value, Input _input)
        {
            if (_input.InputId == 0)
            {
                var obj = UnityObjectsConvertions.ConvertToGameObject(value.GetObject());
                if (obj is GameObject)
                {
                    Set(obj);
                }
            }else if(_input.InputId == 1)
            {
                var ConstellationBehaviour = gameObject.AddComponent(typeof(ConstellationBehaviour)) as ConstellationBehaviour;
                var constellationScript = JsonUtility.FromJson<ConstellationScriptData>(value.GetString());
                ConstellationBehaviour.SetConstellation(constellationScript);
            }
        }

        public void Set(GameObject _gameObject)
        {
            gameObject = _gameObject;
            GameObject.Set(gameObject);
        }
    }
}
