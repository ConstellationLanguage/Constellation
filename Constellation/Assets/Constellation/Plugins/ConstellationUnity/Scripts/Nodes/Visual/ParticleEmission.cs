using UnityEngine;

namespace Constellation.Visual
{
    public class ParticlesEmission : INode, IReceiver, IRequireGameObject
    {
        ParticleSystem particleSystem;
        public const string NAME = "ParticlesEmission";

        public void Setup(INodeParameters _nodeParameters)
        {
            _nodeParameters.AddInput(this, false, "Object", "ParticlesSystem object");
            _nodeParameters.AddInput(this, false, "Enable emission");
        }

        public string NodeName()
        {
            return NAME;
        }

        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        public void Set(GameObject _gameObject)
        {
            var particleComponent = _gameObject.GetComponent<UnityEngine.ParticleSystem>();
            if (particleComponent != null)
                particleSystem = particleComponent;
        }

        public void Receive(Ray value, Input _input)
        {
            if (_input.InputId == 0)
                Set(UnityObjectsConvertions.ConvertToGameObject(value.GetObject()));
            else
            {
                if (particleSystem != null)
                {
                    var emission = particleSystem.emission;
                    if (_input.InputId == 1)
                    {
                        if (value.GetFloat() == 0)
                        {
                            emission.enabled = false;
                        }
                        else
                        {
                            emission.enabled = true;
                        }
                        //particleSystem.emission = emission;
                    }
                }
            }
        }
    }
}