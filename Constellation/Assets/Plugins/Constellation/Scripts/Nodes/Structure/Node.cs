using System.Collections.Generic;
using UnityEngine;
namespace Constellation {
	public class Node<T> : ConstellationObject, IReceiver, ISender, INodeParameters, ILogger where T : INode {
		public T NodeType;
		public List<Input> Inputs;
		public List<Output> Outputs;
		public List<Attribute> Attributes;
		public IReceiver Receiver;
		public float XPosition = 0;
		public float YPosition = 0;

		public Node (T _nodeType) {
			NodeType = _nodeType;
			NodeType.Setup (this, this);
			SetName(NodeType.NodeName(), NodeType.NodeNamespace());
		}

		private void SetName (string _name, string _nameSpace) {
			Name = _name;
			Namespace = _nameSpace;
		}

		public Input[] GetInputs () {
			if (Inputs == null)
				Inputs = new List<Input> ();
			return Inputs.ToArray ();
		}

		public Output[] GetOutputs () {
			if (Outputs == null)
				Outputs = new List<Output> ();
			return Outputs.ToArray ();
		}

		public Attribute[] GetAttributes () {
			if (Attributes == null)
				Attributes = new List<Attribute> ();

			return Attributes.ToArray ();
		}

		public Attribute AddAttribute (Variable value, Attribute.AttributeType _type, string _description) {
			if (Attributes == null)
				Attributes = new List<Attribute> ();

			var newAttribute = new Attribute (_type);
			newAttribute.Value = value;
			Attributes.Add (newAttribute);
			return newAttribute;
		}

		public Input AddInput (IReceiver receiver, bool isWarm, string _description) {
			return AddInput (receiver, isWarm,"", _description);
		}

		public Input AddInput (IReceiver receiver, bool isWarm, string type, string _description) {
			if (Inputs == null)
				Inputs = new List<Input> ();
			var newInput = new Input (System.Guid.NewGuid ().ToString (), Inputs.Count, isWarm, type, _description);
			newInput.Register (this);
			Inputs.Add (newInput);
			Receiver = receiver;
			return newInput;
		}

		public ISender AddOutput (bool isWarm, string _description) {
			return AddOutput(isWarm,"", _description);
		}

		public ISender AddOutput (bool isWarm, string type, string _description) {
			if (Outputs == null)
				Outputs = new List<Output> ();

			var newOutput = new Output (System.Guid.NewGuid ().ToString (), isWarm, type, _description);
			Outputs.Add (newOutput);
			return this;
		}

		public virtual void Start (ConstellationBehaviour _galaxy) { }

		public void Log (Variable value) {
			Debug.Log (value.GetString ());
		}

		public virtual void Destroy () {
			foreach(var input in Inputs)
				input.Unregister();
			
			foreach(var output in Outputs)
				output.Unregister(this);

			Receiver = null;
		}

		public virtual void Receive (Variable value, Input _input) {
			Receiver.Receive (value, _input);
		}

		public virtual void Send (Variable value, Output _output) {
			_output.Send (value);
		}

		public virtual void Send (Variable value, int _output) {
			Outputs[_output].Send (value);
		}

	}
}