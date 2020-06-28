
namespace Constellation
{
    public class TypeConstParsingRule : IParsingRule
    {
        public bool isNodeValid(NodeData node, Node<INode> nodeObject, NodesFactory nodesFactory)
        {
            var i = 0;
            foreach (var input in node.GetInputs())
            {
                if ((input.Type != nodeObject.Inputs[i].Type && nodeObject.Inputs[i].Type != ConstellationEditorRules.ANY && nodeObject.Inputs[i].Type != ConstellationEditorRules.GENERIC && nodeObject.Inputs[i].Type != ConstellationEditorRules.UNDEFINED))
                {
                    return false;
                }
                i++;
            }
            i = 0;
            foreach (var output in node.GetOutputs())
            {
                if ((output.Type != nodeObject.Outputs[i].Type && nodeObject.Outputs[i].Type != ConstellationEditorRules.ANY && nodeObject.Outputs[i].Type != ConstellationEditorRules.GENERIC && nodeObject.Outputs[i].Type != ConstellationEditorRules.UNDEFINED))
                {
                    return false;
                }
                i++;
            }

            return true;
        }
    }
}
