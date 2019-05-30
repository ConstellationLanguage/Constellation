using System.Collections.Generic;

namespace Constellation.Custom
{
    public class CustomNode : INode, IReceiver, ICustomNode
    {
        private ISender sender;
        private Attribute attribute; // attributes are setted in the editor.
        public const string NAME = "Custom Node"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)
        Constellation constellation;
        ConstellationScript ConstellationData;
        NodesFactory nodesFactory;
        ConstellationEventSystem eventSystem;
        bool isInitialized = false;
        private List<Variable> NodeAttributes;
        INodeParameters node;

        public void Setup(INodeParameters _node)
        {
            node = _node;
            var wordValue = new Variable();
            sender = _node.GetSender();
            //attribute = _node.AddAttribute(wordValue.Set("Constellation Name"), Attribute.AttributeType.ReadOnlyValue, "The default word");// setting an attribute (Used only for the editor)
        }

        public void SetNode(ConstellationScriptData [] constellations)
        {
            NodeAttributes = new List<Variable>();
            foreach(var constellation in constellations)
            {
                foreach (var nestedNode in constellation.Nodes)
                {
                    if (nestedNode.Name == CoreNodes.Entry.NAME)
                    {
                        node.AddInput(this, false, nestedNode.GetAttributes()[0].Value.GetString());
                    }

                    if (nestedNode.Name == CoreNodes.Exit.NAME)
                    {
                        node.AddOutput(false, nestedNode.GetAttributes()[0].Value.GetString());
                    }

                    if(nestedNode.Name == Attributes.ValueAttribute.NAME)
                    {
                        var attributeVariable = new Variable(0);
                        node.AddAttribute(attributeVariable, Attribute.AttributeType.Value, "The value");
                        NodeAttributes.Add(attributeVariable);
                    }

                    if (nestedNode.Name == Attributes.WordAttribute.NAME)
                    {
                        var attributeVariable = new Variable("Word");
                        node.AddAttribute(attributeVariable, Attribute.AttributeType.Word, "The value");
                        NodeAttributes.Add(attributeVariable);
                    }
                }
            }
        }

        public void SetConstellation(ConstellationScript script, ConstellationScriptData[] constellationScripts)
        {
            if (isInitialized) // do not initialize twice
                return;

            constellation = new Constellation();

            if (ConstellationComponent.eventSystem == null)
                eventSystem = new ConstellationEventSystem();

            if (NodesFactory.Current == null)
                nodesFactory = new NodesFactory(constellationScripts);
            else
                nodesFactory = NodesFactory.Current;

            var nodes = ConstellationData.GetNodes();
            constellation = new Constellation();
            //SetNodes(nodes);

            var links = ConstellationData.GetLinks();
            foreach (LinkData link in links)
            {
                var input = constellation.GetInput(link.Input.Guid);
                var output = constellation.GetOutput(link.Output.Guid);
                if (input != null && output != null)
                    constellation.AddLink(new Link(constellation.GetInput(link.Input.Guid),
                        constellation.GetOutput(link.Output.Guid),
                        constellation.GetOutput(link.Output.Guid).Type), "none");
            }

            //SetUnityObject();
            constellation.Initialize(System.Guid.NewGuid().ToString(), ConstellationData.name);
            if (constellation.GetInjector() is IAwakable)
                constellation.GetInjector().OnAwake();

            isInitialized = true;
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
        public void Receive(Variable _value, Input _input)
        {
            if (_input.InputId == 0)
                attribute.Value.Set(_value);

            if (_input.InputId == 1)
                sender.Send(attribute.Value, 0);
        }
    }
}