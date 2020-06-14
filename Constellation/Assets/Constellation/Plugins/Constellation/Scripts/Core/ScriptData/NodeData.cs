using System.Collections.Generic;

namespace Constellation {
    [System.Serializable]
    public class NodeData {
        public string Guid;
        public string Name;
        public string Namespace;
        public string OverrideDisplayedName = "";
        public float XPosition;
        public float YPosition;
        public float SizeX;
        public float SizeY;
        public List<InputData> Inputs;
        public List<OutputData> Outputs;
        public List<ParameterData> ParametersData;
        public List<ParameterData> DiscreteParametersData;

        public NodeData (Node<INode> _node) {
            ParametersData = new List<ParameterData> ();
            DiscreteParametersData = new List<ParameterData>();
            Inputs = new List<InputData> ();
            Outputs = new List<OutputData> ();

            foreach (Input input in _node.GetInputs ()) {
                Inputs.Add (new InputData (input.Guid, input.isBright, input.Type, input.Description));
            }

            foreach (Output output in _node.GetOutputs ()) {
                Outputs.Add (new OutputData (output.Guid, output.IsWarm, output.Type, output.Description));
            }

            foreach (Parameter parameter in _node.GetParameters ()) {
                ParametersData.Add (new ParameterData (parameter.Type, parameter.Value));
            }

            foreach(Parameter discreteParameters in _node.GetDiscreteParameters())
            {
                DiscreteParametersData.Add(new ParameterData(discreteParameters.Type, discreteParameters.Value));
            }

            if (_node.GetGuid () == null) {
                _node.Initialize (System.Guid.NewGuid ().ToString (), _node.Name);
            }
           

            XPosition =_node.XPosition;
            YPosition =_node.YPosition;
            SizeX = _node.XSize;
            SizeY = _node.YSize;
            Name = _node.Name;
            Namespace = _node.Namespace;
            Guid = _node.GetGuid ();

            var customNode = _node.NodeType as ICustomNode;
            if (customNode != null)
            {
                Name = customNode.GetDisplayName();
            }
            
        }

        public NodeData (NodeData _node) {
            ParametersData = new List<ParameterData> ();
            DiscreteParametersData = new List<ParameterData>();
            Inputs = new List<InputData> ();
            Outputs = new List<OutputData> ();

            foreach (var input in _node.Inputs) {
                Inputs.Add (new InputData(input.Guid, input.IsBright, input.Type, input.Description));
            }

            foreach (var output in _node.Outputs) {
                Outputs.Add (new OutputData(output.Guid, output.IsBright, output.Type, output.Description));
            }
            
            foreach (var attribute in _node.ParametersData) {
                ParametersData.Add (new ParameterData (attribute.Type, attribute.Value));
            }

            foreach (var discreteParameters in _node.DiscreteParametersData)
            {
                DiscreteParametersData.Add(new ParameterData(discreteParameters.Type, discreteParameters.Value));
            }

            XPosition = _node.XPosition;
            YPosition = _node.YPosition;
            Name = _node.Name;
            Namespace = _node.Namespace;
            Guid = _node.Guid;
            OverrideDisplayedName = _node.OverrideDisplayedName;
        }

        public InputData[] GetInputs () {
            if (Inputs == null)
                Inputs = new List<InputData> ();
            return Inputs.ToArray ();
        }

        public ParameterData[] GetParameters () {
            if (ParametersData == null)
                ParametersData = new List<ParameterData> ();
            return ParametersData.ToArray ();

        }

        public OutputData[] GetOutputs () {
            if (Outputs == null)
                Outputs = new List<OutputData> ();

            return Outputs.ToArray ();
        }
    }
}