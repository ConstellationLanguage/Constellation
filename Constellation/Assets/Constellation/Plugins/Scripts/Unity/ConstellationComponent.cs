using System.Collections.Generic;
using UnityEngine;
namespace Constellation.Unity3D
{
    public class ConstellationComponent : MonoBehaviour
    {
        public static bool IsGCDone = false;
        //protected NodesFactory nodeFactory;
        protected bool isInitialized = false;
        protected ConstellationError lastConstellationError = null;
        public List<BehaviourAttribute> Attributes;
        public ConstellationScript ConstellationData;
        public Constellation constellation;
        protected NodesFactory nodeFactory;

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

            if (NodesFactory.Current == null)
                nodeFactory = new NodesFactory(ConstellationData?.ScriptAssembly?.GetAllScriptData());
            else
                nodeFactory = NodesFactory.Current;

            var nodes = ConstellationData.GetNodes();
            constellation = new Constellation(ConstellationData.script,
                nodeFactory,
                (newNode, node) =>
                {
                    var attributesCounter = 0;
                    if (IsAttribute(node) && Attributes != null)
                    {
                        IAttribute nodeAttribute = newNode.NodeType as IAttribute;
                        if (node.Name != "ObjectAttribute" && attributesCounter < Attributes.Count)
                            nodeAttribute.SetAttribute(Attributes[attributesCounter].Variable);
                        else if (attributesCounter < Attributes.Count)
                            nodeAttribute.SetAttribute(new Ray().Set(Attributes[attributesCounter].UnityObject as object));

                        attributesCounter++;
                    }
                });

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
                        Attributes.Add(new BehaviourAttribute(new Ray().Set(0),
                            node.AttributesData[0].Value.GetString(),
                            BehaviourAttribute.Type.Value, node.Guid));
                    else
                        Attributes.Add(previousAttribute);
                }
                else if (node.Name == "WordAttribute")
                {
                    var previousAttribute = GetAttributeByName(node.AttributesData[0].Value.GetString(), previousAttributes.ToArray());
                    if (previousAttribute == null)
                        Attributes.Add(new BehaviourAttribute(new Ray().Set(0),
                            node.AttributesData[0].Value.GetString(),
                            BehaviourAttribute.Type.Word, node.Guid));
                    else
                        Attributes.Add(previousAttribute);
                }
                else if (node.Name == "ObjectAttribute")
                {
                    var previousAttribute = GetAttributeByName(node.AttributesData[0].Value.GetString(), previousAttributes.ToArray());
                    if (previousAttribute == null)
                        Attributes.Add(new BehaviourAttribute(new Ray().Set(null as object),
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