using System.Collections.Generic;
using UnityEngine;
using Constellation.Parameters;

namespace Constellation.ConstellationNodes
{
    public class StaticConstellation : INode, IReceiver, ICustomNode, IDiscreteNode, IRayReceiver, ISubNodes
    {
        private ISender sender;
        private Parameter nameParameter; // attributes are setted in the editor.
        private Parameter constellationNodeData; // attributes are setted in the editor.
        public const string NAME = "StaticConstellation"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)
        Constellation constellation;
        ConstellationScriptData ConstellationData;
        NodesFactory nodesFactory;
        ConstellationEventSystem eventSystem;
        bool isInitialized = false;
        private List<Ray> NodeParameters;
        INodeParameters node;
        private List<Node<INode>> Entries;
        private List<BrightEntryInfos> BrightEntriesInfos;
        private List<IExitNode> ExitNodes;
        private List<IReceiver> parameters;

        public void Setup(INodeParameters _node)
        {
            node = _node;
            var wordValue = new Ray();
            sender = _node.GetSender();
            nameParameter = _node.AddDiscreteParameter(new Ray().Set("Constellation Name"), Parameter.ParameterType.Word, "The name");// setting an attribute (Used only for the editor)
            constellationNodeData = _node.AddDiscreteParameter(new Ray().Set("Constellation"), Parameter.ParameterType.Word, "The script");
        }

        public string GetDisplayName()
        {
            return nameParameter.Value.GetString();
        }

        public void SetupNodeIO()
        {
            var constellation = JsonUtility.FromJson<ConstellationScriptData>(constellationNodeData.Value.GetString());
            NodeParameters = new List<Ray>();
            foreach (var nestedNode in constellation.Nodes)
            {
                if (nestedNode.Name == CoreNodes.Entry.NAME || nestedNode.Name == CoreNodes.BrightEntry.NAME)
                {
                    node.AddInput(this, nestedNode.Outputs[0].IsBright, nestedNode.Outputs[0].Type, nestedNode.GetParameters()[0].Value.GetString());
                }

                if (nestedNode.Name == CoreNodes.Exit.NAME || nestedNode.Name == CoreNodes.BrightExit.NAME)
                {
                    node.AddOutput(nestedNode.Inputs[0].IsBright, nestedNode.Inputs[0].Type, nestedNode.GetParameters()[0].Value.GetString());
                }

                if (nestedNode.Name == Parameters.ValueParameter.NAME)
                {
                    var attributeVariable = new Ray(0);
                    node.AddParameter(attributeVariable, Parameter.ParameterType.Value, "The value");
                    NodeParameters.Add(attributeVariable);
                }

                if (nestedNode.Name == Parameters.WordParameter.NAME)
                {
                    var attributeVariable = new Ray("Word");
                    node.AddParameter(attributeVariable, Parameter.ParameterType.Word, "The value");
                    NodeParameters.Add(attributeVariable);
                }
            }
        }

        public void UpdateNode(ConstellationScriptData constellation)
        {
            nameParameter.Value = new Ray().Set(constellation.Name);
            constellationNodeData.Value = new Ray().Set(UnityEngine.JsonUtility.ToJson(constellation));
        }

        public void InitializeConstellation(ConstellationScriptData[] constellationScripts)
        {
            Entries = new List<Node<INode>>();
            BrightEntriesInfos = new List<BrightEntryInfos>();
            ExitNodes = new List<IExitNode>();
            parameters = new List<IReceiver>();
            if (isInitialized) // do not initialize twice
                return;

            nodesFactory = new NodesFactory(constellationScripts);

            var parametersCounter = 0;
            var entryCounter = 0;
            var exitCounter = 0;
            constellation = new Constellation(UnityEngine.JsonUtility.FromJson<ConstellationScriptData>(constellationNodeData.Value.GetString()), nodesFactory, (newNode, node) =>
            {
                if (newNode.NodeType is IExitNode)
                {
                    ExitNodes.Add(newNode.NodeType as IExitNode);
                    if(newNode.NodeType is IBrightExitNode)
                    {
                        (newNode.NodeType as IBrightExitNode).SubscribeReceiver(this, exitCounter);
                    }
                    exitCounter++;
                }

                if (newNode.Name == CoreNodes.Entry.NAME || newNode.Name == CoreNodes.BrightEntry.NAME)
                {

                    if (newNode.Name == CoreNodes.BrightEntry.NAME)
                    {
                        BrightEntriesInfos.Add(new BrightEntryInfos(newNode, entryCounter));
                    }

                    entryCounter++;
                    Entries.Add(newNode);
                }

                if(newNode.NodeType.NodeName() == Parameters.ValueParameter.NAME || newNode.NodeType.NodeName() == Parameters.WordParameter.NAME)
                {
                    parameters.Add(newNode.NodeType as IReceiver);
                }

                if (IsAttribute(node) && NodeParameters[parametersCounter] != null)
                {
                    IParameter nodeParameter = newNode.NodeType as IParameter;
                    if (node.Name != "ObjectParameter" && parametersCounter < NodeParameters.Count)
                        nodeParameter.SetParameter(NodeParameters[parametersCounter]);

                    parametersCounter++;
                }
            });
            constellation.Initialize(System.Guid.NewGuid().ToString(), nameParameter.Value.GetString());
            /*if (constellation.GetInjector() is IAwakable)
                constellation.GetInjector().OnAwake();*/

            isInitialized = true;
        }

        bool IsAttribute(NodeData node)
        {
            if (node.Name == ValueParameter.NAME || node.Name == WordParameter.NAME)
                return true;

            return false;
        }

        //return the node name (used in the factory).
        public string NodeName()
        {
            return NAME;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace()
        {
            return NameSpace.NAME;
        }

        //Receive from inputs.
        public void Receive(Ray _value, Input _input)
        {
            if(_input.InputId >= Parameter.ParameterInputID)
            {
                var parameterID = _input.InputId - 1000;
                parameters[parameterID].Receive(_value, _input);
            }

            var count = 0;
            foreach (var entry in Entries)
            {
                if (count == _input.InputId)
                {
                    entry.Send(_value, 0);
                    break;
                }
                count++;
            }

            foreach (var entry in BrightEntriesInfos)
            {
                if (entry.Id == _input.InputId)
                {
                    var exitCount = 0;
                    foreach (var exitNode in ExitNodes)
                    {
                        sender.Send(exitNode.GetExitValue(), exitCount);
                        exitCount++;
                    }
                    break;
                }
            }
        }

        public void SendRay(Ray ray, int id) 
        {
            sender.Send(ray, id);
        }

        public Node<INode> [] GetSubNodes()
        {
            return constellation.GetNodes();
        }
    }

    public class BrightEntryInfos
    {
        public Node<INode> BrightEntry;
        public int Id;
        
        public BrightEntryInfos(Node<INode> _brightEntry, int _id)
        {
            BrightEntry = _brightEntry;
            Id = _id;
        }
    }
}