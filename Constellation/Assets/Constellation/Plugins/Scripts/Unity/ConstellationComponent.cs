using System.Collections.Generic;
using UnityEngine;
namespace Constellation
{
    public class ConstellationComponent : MonoBehaviour
    {
        public static ConstellationEventSystem eventSystem;
        public static bool IsGCDone = false;
        protected NodesFactory nodeFactory;
        protected bool isInitialized = false;
        protected ConstellationError lastConstellationError = null;
        public List<BehaviourAttribute> Attributes;
        public ConstellationScript ConstellationData;
        public Constellation constellation;

        public ConstellationScript GetConstellationData()
        {
            return ConstellationData;
        }

        public void SetConstellationScript(ConstellationScript constellationScript)
        {
            ConstellationData = constellationScript;
        }

        public void HasThrownError(ConstellationError error)
        {
            lastConstellationError = error;
        }

        public ConstellationError GetLastError()
        {
            return lastConstellationError;
        }

        public void Initialize()
        {
            if (ConstellationData == null && Application.isPlaying)
            {
                this.enabled = false;
                throw new NoConstellationAttached(this);
            }

            if (isInitialized) // do not initialize twice
                return;

            if (ConstellationComponent.eventSystem == null)
                eventSystem = new ConstellationEventSystem();

            if (NodesFactory.Current == null)
                nodeFactory = new NodesFactory(ConstellationData.ScriptAssembly.GetAllScriptData());
            else
                nodeFactory = NodesFactory.Current;

            var nodes = ConstellationData.GetNodes();
            constellation = new Constellation();
            SetNodes(nodes);

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

            SetUnityObject();
            constellation.Initialize(System.Guid.NewGuid().ToString(), ConstellationData.name);
            if (constellation.GetInjector() is IAwakable)
                constellation.GetInjector().OnAwake();
            isInitialized = true;
        }

        public void UpdateAttributes(NodeData[] nodes)
        {
            var previousAttributes = Attributes;
            Attributes = new List<BehaviourAttribute>();
            foreach (NodeData node in nodes)
            {
                if (node == null || previousAttributes == null)
                    return;
                if (node.Name == "ValueAttribute")
                {
                    var previousAttribute = GetAttributeByName(node.AttributesData[0].Value.GetString(), previousAttributes.ToArray());
                    if (previousAttribute == null)
                        Attributes.Add(new BehaviourAttribute(new Variable().Set(0),
                            node.AttributesData[0].Value.GetString(),
                            BehaviourAttribute.Type.Value, node.Guid));
                    else
                        Attributes.Add(previousAttribute);
                }
                else if (node.Name == "WordAttribute")
                {
                    var previousAttribute = GetAttributeByName(node.AttributesData[0].Value.GetString(), previousAttributes.ToArray());
                    if (previousAttribute == null)
                        Attributes.Add(new BehaviourAttribute(new Variable().Set(0),
                            node.AttributesData[0].Value.GetString(),
                            BehaviourAttribute.Type.Word, node.Guid));
                    else
                        Attributes.Add(previousAttribute);
                }
                else if (node.Name == "ObjectAttribute")
                {
                    var previousAttribute = GetAttributeByName(node.AttributesData[0].Value.GetString(), previousAttributes.ToArray());
                    if (previousAttribute == null)
                        Attributes.Add(new BehaviourAttribute(new Variable().Set(null as object),
                            node.AttributesData[0].Value.GetString(),
                            BehaviourAttribute.Type.UnityObject, node.Guid));
                    else
                        Attributes.Add(previousAttribute);
                }
            }
        }

        public void SetUnityObject()
        {
            foreach (var node in constellation.GetNodes())
            {
                AddUnityObject(node);
            }
        }

        public void AddUnityObject(Node<INode> node)
        {
            if (node.NodeType as IRequireGameObject != null)
            {
                var igameObject = node.NodeType as IRequireGameObject;
                igameObject.Set(gameObject);
            }
        }

        BehaviourAttribute GetAttributeByName(string name, BehaviourAttribute[] attributes)
        {
            foreach (var attribute in attributes)
            {
                if (attribute.Name == name)
                    return attribute;
            }
            return null;
        }

        void SetNodes(NodeData[] nodes)
        {
            var attributesCounter = 0;
            foreach (NodeData node in nodes)
            {
                var newNode = nodeFactory.GetNode(node);
                constellation.AddNode(newNode, node.Guid, node);
                if (IsAttribute(node) && Attributes != null)
                {
                    IAttribute nodeAttribute = newNode.NodeType as IAttribute;
                    if (node.Name != "ObjectAttribute" && attributesCounter < Attributes.Count)
                        nodeAttribute.SetAttribute(Attributes[attributesCounter].Variable);
                    else if (attributesCounter < Attributes.Count)
                        nodeAttribute.SetAttribute(new Variable().Set(Attributes[attributesCounter].UnityObject as object));

                    attributesCounter++;
                }
            }
        }

        bool IsAttribute(NodeData node)
        {
            if (node.Name == "ValueAttribute" || node.Name == "WordAttribute" || node.Name == "ObjectAttribute")
                return true;

            return false;
        }

        public Constellation GetConstellation(bool throwOnNull = true)
        {
            if (constellation == null && throwOnNull)
                throw new TryingToAccessANullCosntellation(this);

            return constellation;
        }
    }
}