namespace Constellation.Custom
{
    public class CustomNode : INode, IReceiver
    {
        private ISender sender;
        private Attribute attribute; // attributes are setted in the editor.
        public const string NAME = "Custom Node"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)
        Constellation constellation;
        ConstellationScript ConstellationData;
        NodesFactory nodesFactory;
        ConstellationEventSystem eventSystem;
        bool isInitialized = false;
        public void Setup(INodeParameters _node)
        {
            var wordValue = new Variable();
            sender = _node.GetSender();
            //_node.AddOutput(false, "Current setted word"); // setting a cold input
            attribute = _node.AddAttribute(wordValue.Set("Constellation Name"), Attribute.AttributeType.ReadOnlyValue, "The default word");// setting an attribute (Used only for the editor)
        }

        public void SetConstellation(ConstellationScript script, ConstellationScript [] constellationScripts)
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