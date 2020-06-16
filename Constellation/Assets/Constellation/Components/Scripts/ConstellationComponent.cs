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
        public List<ConstellationParameter> Parameters;
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

            nodeFactory = new NodesFactory(ConstellationData?.ScriptAssembly?.GetAllStaticScriptData());

            var nodes = ConstellationData.GetNodes();
            constellation = new Constellation(ConstellationData.script,
                nodeFactory,
                (newNode, node) =>
                {
                    var attributesCounter = 0;
                    if (IsAttribute(node) && Parameters != null)
                    {
                        IParameter nodeParameter = newNode.NodeType as IParameter;
                        if (node.Name != "ObjectParameter" && attributesCounter < Parameters.Count)
                            nodeParameter.SetParameter(Parameters[attributesCounter].Variable);
                        else if (attributesCounter < Parameters.Count)
                            nodeParameter.SetParameter(new Ray().Set(Parameters[attributesCounter].UnityObject as object));

                        attributesCounter++;
                    }
                });

            SetUnityObject();
            constellation.Initialize(System.Guid.NewGuid().ToString(), ConstellationData.name);
            if (constellation.GetInjector() is IAwakable)
                constellation.GetInjector().OnAwake();
            isInitialized = true;
        }

        public void UpdateParameters(NodeData[] nodes)
        {
            var previousParameters = Parameters;
            Parameters = new List<ConstellationParameter>();
            foreach (NodeData node in nodes)
            {
                if (node == null || previousParameters == null)
                    return;
                if (node.Name == "ValueParameter")
                {
                    var previousAttribute = GetParameterByName(node.ParametersData[0].Value.GetString(), previousParameters.ToArray());
                    if (previousAttribute == null)
                        Parameters.Add(new ConstellationParameter(new Ray().Set(0),
                            node.ParametersData[0].Value.GetString(),
                            ConstellationParameter.Type.Value, node.Guid));
                    else
                        Parameters.Add(previousAttribute);
                }
                else if (node.Name == "WordParameter")
                {
                    var previousAttribute = GetParameterByName(node.ParametersData[0].Value.GetString(), previousParameters.ToArray());
                    if (previousAttribute == null)
                        Parameters.Add(new ConstellationParameter(new Ray().Set(0),
                            node.ParametersData[0].Value.GetString(),
                            ConstellationParameter.Type.Word, node.Guid));
                    else
                        Parameters.Add(previousAttribute);
                }
                else if (node.Name == "ObjectParameter")
                {
                    var previousAttribute = GetParameterByName(node.ParametersData[0].Value.GetString(), previousParameters.ToArray());
                    if (previousAttribute == null)
                        Parameters.Add(new ConstellationParameter(new Ray().Set(null as object),
                            node.ParametersData[0].Value.GetString(),
                            ConstellationParameter.Type.UnityObject, node.Guid));
                    else
                        Parameters.Add(previousAttribute);
                }
            }
        }

        public void SetUnityObject()
        {
            foreach (var node in constellation.GetAllNodesAndSubNodes())
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

        ConstellationParameter GetParameterByName(string name, ConstellationParameter[] parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.Name == name)
                    return parameter;
            }
            return null;
        }

        bool IsAttribute(NodeData node)
        {
            if (node.Name == "ValueParameter" || node.Name == "WordParameter" || node.Name == "ObjectParameter")
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