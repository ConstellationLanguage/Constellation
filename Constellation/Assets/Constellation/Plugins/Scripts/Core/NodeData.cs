using System.Collections.Generic;

namespace Constellation {
    [System.Serializable]
    public class NodeData {
        public string Name;
        public string Namespace;
        public string Guid;
        public float XPosition;
        public float YPosition;
        public List<InputData> Inputs;
        public List<OutputData> Outputs;
        public List<AttributeData> AttributesData;

        public NodeData (Node<INode> _node) {
            AttributesData = new List<AttributeData> ();
            Inputs = new List<InputData> ();
            Outputs = new List<OutputData> ();

            foreach (Input input in _node.GetInputs ()) {
                Inputs.Add (new InputData (input.Guid, input.isWarm, input.Type, input.Description));
            }

            foreach (Output output in _node.GetOutputs ()) {
                Outputs.Add (new OutputData (output.Guid, output.IsWarm, output.Type, output.Description));
            }

            foreach (Attribute attribute in _node.GetAttributes ()) {
                AttributesData.Add (new AttributeData (attribute.Type, attribute.Value));
            }

            if (_node.GetGuid () == null) {
                _node.Initialize (System.Guid.NewGuid ().ToString (), _node.Name);
            }

            XPosition =_node.XPosition;
            YPosition =_node.YPosition;
            Name = _node.Name;
            Namespace = _node.Namespace;
            Guid = _node.GetGuid ();
        }

        public NodeData (NodeData _node) {
            AttributesData = new List<AttributeData> ();
            Inputs = new List<InputData> ();
            Outputs = new List<OutputData> ();

            foreach (var input in _node.Inputs) {
                Inputs.Add (new InputData(input.Guid, input.IsWarm, input.Type, input.Description));
            }

            foreach (var output in _node.Outputs) {
                Outputs.Add (new OutputData(output.Guid, output.IsWarm, output.Type, output.Description));
            }
            
            foreach (var attribute in _node.AttributesData) {
                AttributesData.Add (new AttributeData (attribute.Type, attribute.Value));
            }

            XPosition = _node.XPosition;
            YPosition = _node.YPosition;
            Name = _node.Name;
            Namespace = _node.Namespace;
            Guid = _node.Guid;
        }

        public InputData[] GetInputs () {
            if (Inputs == null)
                Inputs = new List<InputData> ();
            return Inputs.ToArray ();
        }

        public AttributeData[] GetAttributes () {
            if (AttributesData == null)
                AttributesData = new List<AttributeData> ();
            return AttributesData.ToArray ();

        }

        public OutputData[] GetOutputs () {
            if (Outputs == null)
                Outputs = new List<OutputData> ();

            return Outputs.ToArray ();
        }
    }
}