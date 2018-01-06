using UnityEngine;

namespace Constellation.Experimental {
    public class OSCManager : INode, IReceiver, IGameObject {
        public const string NAME = "OSCManager"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)
        public static OSCComponent.OSC OSC;
        private Variable inPort;
        private Variable outIP;
        private Variable outPort;
        private GameObject currentGameObject;
        private bool isMultipleOSCInstances;
        private ISender sender;
        public void Setup (INodeParameters _node, ILogger _logger) {
            inPort = new Variable ();
            outIP = new Variable ();
            outPort = new Variable ();
            var wordValue = new Variable ();
            _node.AddInput (this, false, "input port"); 
            _node.AddInput (this, false, "output port");
            _node.AddInput (this, false, "output IP");
            sender = _node.AddOutput (true, "On OSC Ready");
            isMultipleOSCInstances = false;
            Application.runInBackground = true;
        }

        //return the node name (used in the factory).
        public string NodeName () {
            return NAME;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Set (GameObject gameObject) {
            currentGameObject = gameObject;

            if (OSC != null) {
                Debug.LogError ("You can only have one instance of the OSC script");
                isMultipleOSCInstances = true;
            }
        }

        //Receive from inputs.
        public void Receive (Variable _value, Input _input) {
            if (isMultipleOSCInstances)
                return;

            if (_input.InputId == 0) {
                if (_value.IsFloat ())
                    inPort.Set (_value);
                else
                    Debug.LogError ("you must set a value that is a number in OSC input 0");
            }

            if (_input.InputId == 1) {
                if (_value.IsFloat ())
                    outPort.Set (_value);
                else
                    Debug.LogError ("you must set a value that is a number in OSC input 1");
            }

            if (_input.InputId == 2) {
                if (_value.IsString ())
                    outIP.Set (_value);
                else
                    Debug.LogError ("you must set a value that is a valid ip in input 2");

                OSC = currentGameObject.AddComponent<OSCComponent.OSC> ();
                OSC.inPort = (int) inPort.GetFloat ();
                OSC.outPort = (int) outPort.GetFloat ();
                OSC.outIP = outIP.GetString ();
                OSC.Initialize ();
                sender.Send(new Variable(), 0);
            }
        }
    }
}