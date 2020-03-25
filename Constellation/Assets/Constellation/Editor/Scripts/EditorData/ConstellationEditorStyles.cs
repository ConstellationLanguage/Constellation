using UnityEngine;
using System.Collections.Generic;

namespace ConstellationEditor
{
    [CreateAssetMenu(fileName = "ConstellationStyle", menuName = "Constellation Editor/Editor style", order = 1)]
    public class ConstellationEditorStyles : ScriptableObject
    {
        //Nodes
        public GUIStyle NodeStyle;
        public GUIStyle NodeSelectedStyle;
        public GUIStyle NodeTitleStyle;
        public GUIStyle NodeResizeButtonStyle;
        public ConstellationIOStyles IOAnyStyle;
        public ConstellationIOStyles IOUnknownStyle;
        public ConstellationIOStyles IOUndefinedStyle;
        public List<ConstellationIOStyles> IOStyles;
        public GUIStyle NodeValueAttributeStyle;
        public GUIStyle NodeValueAttributeLabelStyle;
        public GUIStyle NodeWordAttributeStyle;
        public GUIStyle NodeXAtrributeStyle;
        public GUIStyle NodeYAtrributeStyle;
        public GUIStyle NodeZAtrributeStyle;
        public GUIStyle NodeRAtrributeStyle;
        public GUIStyle NodeGAtrributeStyle;
        public GUIStyle NodeBAtrributeStyle;
        public GUIStyle NodeAAtrributeStyle;
        public GUIStyle NodeReadOnlyAttributeStyle;
        public GUIStyle GenericDeleteStyle;
        public GUIStyle GenericQuestionStyle;
        public float nodeTitleHeight = 20;
        public float nodeDeleteSize = 15;
        public float resizeButtonSize = 10;
        public float inputSize = 13;
        public float outputSize = 13;
        public float spacing = 7;
        public float titleLeftMargin = 5;
        public float titleRightMargin = 5;
        public float leftAttributeMargin = 5;
        public float rightAttributeMargin = 5;
        public float attributeSpacing = 2;
        public float nodeButtonsTopMargin = 1;
        public float nodeButtonsSpacing = 5;

        //Node editor
        public GUIStyle SelectionAreaStyle;

        public ConstellationIOStyles GetConstellationIOStylesByType(string name)
        {
            if(name == "Any")
            {
                return IOAnyStyle;
            } else if(name == "Undefined")
            {
                return IOUndefinedStyle;
            }
            foreach(var constellationIOStyle in IOStyles)
            {
                if(constellationIOStyle.TypeName == name)
                {
                    return constellationIOStyle;
                }
            }
            return IOUnknownStyle;
        }
    }
}
