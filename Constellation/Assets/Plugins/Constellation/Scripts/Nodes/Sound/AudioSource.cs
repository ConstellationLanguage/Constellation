using UnityEngine;

namespace Constellation.Sound {
    public class AudioSource : INode, IReceiver, IGameObject {
        private ISender sender;
        private Attribute attribute;
        public const string NAME = "AudioSource";
        private GameObject gameObject;
        private UnityEngine.AudioSource audioSource;

        public void Setup (INodeParameters _nodeParameters, ILogger _logger) {
            var wordValue = new Variable ();
            _nodeParameters.AddInput (this, false, "Object", "The GameObject that contains the play sound component");
            _nodeParameters.AddInput (this, true, "Play the current sound attached to the audio source");
            sender = _nodeParameters.AddOutput (false, "get the current state");
            attribute = _nodeParameters.AddAttribute (wordValue.Set ("Var"), Attribute.AttributeType.ReadOnlyValue, "The default word");
        }

        public void Set (GameObject newGameObject) {
            gameObject = newGameObject;
            var newAudioSource = newGameObject.GetComponent<UnityEngine.AudioSource> () as UnityEngine.AudioSource;
            if (newAudioSource != null)                
                audioSource = newAudioSource;

        }

        //return the node name (used in the factory).
        public string NodeName () {
            return NAME;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        //Receive from inputs.
        public void Receive (Variable _value, Input _input) {
            if (_input.InputId == 0) {
                var newGameObject = UnityObjectsConvertions.ConvertToGameObject (_value.GetObject ());
                if (newGameObject != null) {
                    var newAudioSource = newGameObject.GetComponent<UnityEngine.AudioSource> () as UnityEngine.AudioSource;
                    if (audioSource != null)
                        audioSource = newAudioSource;
                }

            }

            if (_input.InputId == 1 && audioSource != null)
                audioSource.Play ();

        }
    }
}