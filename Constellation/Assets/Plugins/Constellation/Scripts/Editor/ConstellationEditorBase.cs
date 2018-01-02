using UnityEngine;
using UnityEditor;

namespace ConstellationEditor
{
    public class ExtendedEditorWindow : EditorWindow, IGUI
    {
        public void DragWindow()
        {
            GUI.DragWindow();
        }

        public void SetColor(Color color)
        {
            GUI.color = color;
        }

        public float VerticalScrollBar(Rect position, float value, float size, float topValue, float bottomValue)
        {
            return GUI.VerticalScrollbar(position, value, 1.0F, topValue, bottomValue);
        }

        public float HorizontalScrollBar(Rect position, float value, float size, float topValue, float bottomValue)
        {
            return GUI.HorizontalScrollbar(position, value, 1.0F, topValue, bottomValue);
        }

        public void DrawTexture(Rect rect, Texture2D texture)
        {
            GUI.DrawTexture(rect, texture);
        }

        public bool DrawButton(Rect _rect)
        {
            if (GUI.Button(_rect, ""))
            {
                return true;
            }

            return false;
        }
    }
}
