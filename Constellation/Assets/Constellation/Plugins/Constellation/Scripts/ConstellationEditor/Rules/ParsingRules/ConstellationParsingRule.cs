
namespace Constellation
{
    public class ConstellationParsingRule : IParsingRule
    {
        public bool isNodeValid(NodeData node, Node<INode> nodeObject, NodesFactory nodesFactory)
        {
            if (nodeObject == null)
            {
                return false;
            }
            else if (node.Inputs.Count != nodeObject.Inputs.Count || node.Outputs.Count != nodeObject.Outputs.Count || node.GetParameters().Length != nodeObject.GetParameters().Length)
            {
                return false;
            }
            else if (node.Namespace == ConstellationNodes.NameSpace.NAME) // to be done only when the node is edited
            {
                return false;
            }

            var i = 0;
            if (node.GetParameters().Length != nodeObject.GetParameters().Length)
            {
                return false;
            }

            foreach (var parameter in node.GetParameters())
            {
                if (parameter.Type != nodeObject.GetParameters()[i].Type)
                {
                    return false;
                }
                i++;
            }

            i = 0;
            foreach (var input in node.GetInputs())
            {
                if (input.IsBright != nodeObject.Inputs[i].isBright || input.Description != nodeObject.Inputs[i].Description)
                {
                    return false;
                }
                i++;
            }
            i = 0;
            foreach (var output in node.GetOutputs())
            {
                if (output.IsBright != nodeObject.Outputs[i].IsBright || output.Description != nodeObject.Outputs[i].Description)
                {
                    return false;
                }
                i++;
            }

            return true;
        }
    }
}
