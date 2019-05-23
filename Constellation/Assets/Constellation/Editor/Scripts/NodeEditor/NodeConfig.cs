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
        public Color WarmInputColor = new Color(0.8f, 0.5f, 0.3f);
        public Color ColdInputColor = Color.yellow;
        public Color WarmInputObjectColor = new Color(0.2f, 0.6f, 0.55f);
        public Color ColdInputObjectColor = new Color(0.2f, 0.3f, 0.6f);
        public GUIStyle NodeStyle = new GUIStyle(GUI.skin.GetStyle("flow node 0")) {
            alignment = TextAnchor.UpperRight,
            margin = new RectOffset(0, 0, -5, 0)
        };
        public GUIStyle NodeHoverStyle = new GUIStyle(GUI.skin.GetStyle("flow node 0 on")) {
            alignment = TextAnchor.UpperRight,
            margin = new RectOffset(0, 0, -5, 0)
        };
        public GUIStyle NoteStyle = GUI.skin.GetStyle("VCS_StickyNote");
        public GUIStyle RoundButton = GUI.skin.GetStyle("sv_label_0");
        public GUIStyle HexagonButton = GUI.skin.GetStyle("flow var 0");
        public GUIStyle CloseButton = GUI.skin.GetStyle("WinBtnClose"); //Node/Connection close 'X'
        public GUIStyle HeaderLabel = GUI.skin.GetStyle("MiniLabel");
        public GUIStyle Tooltip = GUI.skin.GetStyle("AnimationEventTooltip");
        private GUIStyle noteHoverStyle;
        private GUIStyle helpStyle;

        private GUIStyle InitHelpStyle () {
            helpStyle = new GUIStyle();
            HelpStyle.normal.background = (Texture2D)EditorGUIUtility.IconContent("_Help").image;
            return helpStyle;
        }

        private GUIStyle InitNoteHoverStyle () {
            noteHoverStyle = new GUIStyle(NoteStyle);
            noteHoverStyle.normal.background = EditorGUIUtility.Load(ConstellationEditor.GetEditorAssetPath() + "note_on.png") as Texture2D;
            noteHoverStyle.border = new RectOffset(10, 10, 10, 10);
            noteHoverStyle.overflow = new RectOffset(7, 7, 5, 10);
            return noteHoverStyle;
        }

        public GUIStyle GetConnectionStyle (bool _isWarm, string _type) {
            if (_isWarm) {
                if (_type == "Object")
                    return WarmInputObjectStyle;
                else
                    return WarmInputStyle;
            } else {
                if (_type == "Object")
                    return ColdInputObjectStyle;
                else
                    return ColdInputStyle;
            }
        }

        public Color GetConnectionColor (bool _isWarm, string _type) {
            if (_isWarm) {
                if (_type == "Object")
                    return WarmInputObjectColor;
                else
                    return WarmInputColor;
            } else {
                if (_type == "Object")
                    return ColdInputObjectColor;
                else
                    return ColdInputColor;
            }
        }

        public GUIStyle HelpStyle {
            get {
                return helpStyle ?? InitHelpStyle();
            }
        }

        public GUIStyle NoteHoverStyle {
            get {
                return noteHoverStyle ?? InitNoteHoverStyle();
            }
        }
    }
}