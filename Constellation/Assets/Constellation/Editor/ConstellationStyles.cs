using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public static class ConstellationStyles {
        private static GUIStyle helpStyle;

        private static GUIStyle InitHelpStyle() {
            helpStyle = new GUIStyle();
            HelpStyle.normal.background = (Texture2D)EditorGUIUtility.IconContent("_Help").image;
            return helpStyle;
        }

        public static GUIStyle HelpStyle {
            get {
                return helpStyle ?? InitHelpStyle();
            }
        }

    }
}
