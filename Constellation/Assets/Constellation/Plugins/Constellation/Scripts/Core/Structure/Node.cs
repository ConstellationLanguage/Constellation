using System.Collections.Generic;
using UnityEngine;
namespace Constellation {
	public class Node<T> : ConstellationObject, IReceiver, ISender, INodeParameters where T : INode {
		public T NodeType;
		public List<Input> Inputs;
		public List<Output> Outputs;
		public List<Parameter> NodeParameters;
		public List<Parameter> DiscretParameters;
		public IReceiver Receiver;
		public float XPosition = 0;
		public float YPosition = 0;
		public float XSize = 0;
		public float YSize = 0;

		/// <summary>
		///Create a new node that implements INode. 
		/// </summary>
		/// <param name="_nodeType">The type of your node that will set NodeType</param>   
		public Node (T _nodeType) {
			NodeType = _nodeType;
			NodeType.Setup (this);
			SetName (NodeType.NodeName (), NodeType.NodeNamespace ());
		}

		private void SetName (string _name, string _nameSpace) {
			Name = _name;
			Namespace = _nameSpace;
		}

		/// <summary>
		///Return the inputs of the node
		/// </summary>
		public Input[] GetInputs () {
			if (Inputs == null)
				Inputs = new List<Input> ();
			return Inputs.ToArray ();
		}

		/// <summary>
		///Return the outputs of the node
		/// </summary>
		public Output[] GetOutputs () {
			if (Outputs == null)
				Outputs = new List<Output> ();
			return Outputs.ToArray ();
		}

		/// <summary>
		///Return the parameters of the node
		/// </summary>
		public Parameter[] GetParameters () {
			if (NodeParameters == null)
				NodeParameters = new List<Parameter> ();

			return NodeParameters.ToArray ();
		}

		/// <summary>
		///Return the parameters of the node
		/// </summary>
		public Parameter[] GetDiscreteParameters()
		{
			if (DiscretParameters == null)
				DiscretParameters = new List<Parameter>();

			return DiscretParameters.ToArray();
		}

		/// <summary>
		///Return the attribute you created
		/// In most case this function should only be called inside the setup function of your node.
		/// </summary>
		/// <param name="value">the default value of the attribute</param>  
		/// <param name="type">The attribute type displayed in editor</param>
		/// <param name="description">Description of the attribute (Not implemented)</param>
		public Parameter AddParameter (Ray value, Parameter.ParameterType type, string description) {
			if (NodeParameters == null)
				NodeParameters = new List<Parameter> ();

			var newParameter = new Parameter (type);
			newParameter.Value = value;
			NodeParameters.Add (newParameter);
			return newParameter;
		}

        public Parameter AddParameter(Ray value, Parameter.ParameterType type, string description, Parameter.OnAttributeChanged onAttributeChanged)
        {
            if (NodeParameters == null)
                NodeParameters = new List<Parameter>();

            var newParameter = new Parameter(type);
            newParameter.Value = value;
            NodeParameters.Add(newParameter);
            return newParameter;
        }

		/// <summary>
		///Return the attribute you created
		/// In most case this function should only be called inside the setup function of your node.
		/// </summary>
		/// <param name="value">the default value of the attribute</param>  
		/// <param name="type">The attribute type displayed in editor</param>
		/// <param name="description">Description of the attribute (Not implemented)</param>
		public Parameter AddDiscreteParameter(Ray value, Parameter.ParameterType type, string description)
		{
			if (DiscretParameters == null)
				DiscretParameters = new List<Parameter>();

			var newParameter = new Parameter(type);
			newParameter.Value = value;
			DiscretParameters.Add(newParameter);
			return newParameter;
		}

		public Parameter AddDiscreteParameter(Ray value, Parameter.ParameterType type, string description, Parameter.OnAttributeChanged onAttributeChanged)
		{
			if (DiscretParameters == null)
				DiscretParameters = new List<Parameter>();

			var newParameter = new Parameter(type);
			newParameter.Value = value;
			DiscretParameters.Add(newParameter);
			return newParameter;
		}


		/// <summary>
		///Return the Input you created
		/// In most case this function should only be called inside the setup function of your node.
		/// </summary>
		/// <param name="receiver">If called from a node just put this</param>  
		/// <param name="isWarm">If the input is warm your Receiver should output a value when this is called</param>
		/// <param name="description">Description of the attribute (Not implemented)</param>
		public Input AddInput (IReceiver receiver, bool isWarm, string description) {
			return AddInput (receiver, isWarm, "Var", description);
		}

		/// <summary>
		/// Return the Input you created
		/// Use this function if you want to create an input which cannot receive a Variable. Ex "Object" for unity object only
		/// In most case this function should only be called inside the setup function of your node.
		/// </summary>
		/// <param name="receiver">If called from a node just set this parameter to this</param>  
		/// <param name="isWarm">Is the input is warm your Receiver should output a value when this is called</param>
		/// <param name="type">The type of the input.</param>
		/// <param name="description">Description of the attribute (Not implemented)</param>
		public Input AddInput (IReceiver receiver, bool isWarm, string type, string description) {
			if (Inputs == null)
				Inputs = new List<Input> ();
			var newInput = new Input (System.Guid.NewGuid ().ToString (), Inputs.Count, isWarm, type, description);
			newInput.Register (this);
			Inputs.Add (newInput);
			Receiver = receiver;
			return newInput;
		}

		/// <summary>
		/// Return the Output you created
		/// Use this function to create an input. 
		/// In most case this function should only be called inside the setup function of your node.
		/// </summary>
		/// <param name="isWarm">If called from a node just set this parameter to this</param>  
		/// <param name="description">Is the input is warm your Receiver should output a value when this is called</param>
		public void AddOutput (bool isWarm, string description) {
			AddOutput (isWarm, "Var", description);
		}

		/// <summary>
		/// Return the Output you created
		/// Use this function if you want to create an output which cannot receive a Variable. Ex "Object" for unity object only
		/// In most case this function should only be called inside the setup function of your node.
		/// </summary>
		/// <param name="isWarm">If called from a node just set this parameter to this</param>  
		/// <param name="type">the type of the input.</param>  
		/// <param name="description">Is the input is warm your Receiver should output a value when this is called</param>
		public void AddOutput (bool isWarm, string type, string description) {
			if (Outputs == null)
				Outputs = new List<Output> ();

			var newOutput = new Output (System.Guid.NewGuid ().ToString (), isWarm, type, description);
			Outputs.Add (newOutput);
		}

		/// <summary>
		/// return the node sender
		/// </summary>
		public ISender GetSender () {
			return this;
		}

		/// <summary>
		/// Use this function if you want to log a variable from your node.
		/// </summary>
		public void Log (Ray value) {
			Debug.Log (value.GetString ());
		}

		public virtual void Destroy () {
			foreach (var input in Inputs)
				input.Unregister ();

			foreach (var output in Outputs)
				output.Unregister (this);

			Receiver = null;
		}

		public virtual void Receive (Ray value, Input _input) {
            if(Receiver == null)
            {
                Receiver = NodeType as IReceiver;
            }
			Receiver.Receive (value, _input);
		}

		public virtual void Send (Ray value, Output _output) {
			_output.Send (value);
		}

		public virtual void Send (Ray value, int _output) {
			Outputs[_output].Send (value);
		}
    }
}