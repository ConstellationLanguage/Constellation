using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeConfig {
        public float NodeWidth = 100;
        public float NodeWidthAsAttributes = 125;
        public float InputSize = 20;
        public float OutputSize = 20;
        public float TopMargin = 20;
        public float LinkButtonSize = 16;
        public GUIStyle WarmInputStyle = GUI.skin.GetStyle("sv_label_5");
        public GUIStyle ColdInputStyle = GUI.skin.GetStyle("sv_label_4");
        public GUIStyle WarmOutputStyle = GUI.skin.GetStyle("sv_label_5");
        public GUIStyle ColdOutputStyle = GUI.skin.GetStyle("sv_label_4");
        public GUIStyle WarmInputObjectStyle = GUI.skin.GetStyle("sv_label_2");
        public GUIStyle ColdInputObjectStyle = GUI.skin.GetStyle("sv_label_1");
        public GUIStyle WarmOutputObjectStyle = GUI.skin.GetStyle("sv_label_2");
        public GUIStyle ColdOutputObjectStyle = GUI.skin.GetStyle("sv_label_1");
        public Rect AtrributeSize = new Rect(18, 15, 88, 20);
        private GUIStyle helpStyle;
        public Color WarmInputColor = new Color(0.8f, 0.5f, 0.3f);
        public Color ColdInputColor = Color.yellow;
        public Color WarmInputObjectColor = new Color(0.2f, 0.6f, 0.55f);
        public Color ColdInputObjectColor = new Color(0.2f, 0.3f, 0.6f);

        private GUIStyle InitHelpStyle () {
            helpStyle = new GUIStyle();
            HelpStyle.normal.background = (Texture2D)EditorGUIUtility.IconContent("_Help").image;
            return helpStyle;
        }
        
        public GUIStyle HelpStyle {
            get {
                return helpStyle ?? InitHelpStyle();
            }
        }
    }
}