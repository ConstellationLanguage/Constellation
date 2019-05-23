using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class ExtendedEditorWindow : EditorWindow, IGUI {
        public bool shouldRepaint = false;

        public void DragWindow() {
            GUI.DragWindow();
        }

        public void SetColor(Color color) {
            GUI.color = color;
        }

        public float VerticalScrollBar(Rect position, float value, float size, float topValue, float bottomValue) {
            return GUI.VerticalScrollbar(position, value, 1.0F, topValue, bottomValue);
        }

        public float HorizontalScrollBar(Rect position, float value, float size, float topValue, float bottomValue) {
            return GUI.HorizontalScrollbar(position, value, 1.0F, topValue, bottomValue);
        }

        public void DrawTexture(Rect rect, Texture2D texture) {
            GUI.DrawTexture(rect, texture);
        }

        public void RequestRepaint() {
            shouldRepaint = true;
        }

        public void RepaintIfRequested() {
            if (shouldRepaint) {
                shouldRepaint = false;
                Repaint();
            }
        }

        public bool DrawButton(Rect _rect, string _name) {
            if (GUI.Button(_rect, _name)) {
                return true;
            }

            return false;
        }
    }
}