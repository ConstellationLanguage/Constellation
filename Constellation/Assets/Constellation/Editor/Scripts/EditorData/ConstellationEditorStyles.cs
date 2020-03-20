using UnityEngine;
using System.Collections.Generic;

namespace ConstellationEditor
{
    [CreateAssetMenu(fileName = "ConstellationStyle", menuName = "Constellation Editor/Editor style", order = 1)]
    public class ConstellationEditorStyles : ScriptableObject
    {
        public GUIStyle NodeStyle;
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
        public GUIStyle GenericDeleteStyle;

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
