using UnityEngine;

namespace ConstellationEditor {
    public interface IGUI {
        void DragWindow ();
        void SetColor (Color color);
        void DrawTexture (Rect rect, Texture2D texture);
        bool DrawButton (Rect rect, string name);
        float VerticalScrollBar (Rect position, float value, float size, float topValue, float bottomValue);
        float HorizontalScrollBar (Rect position, float value, float size, float topValue, float bottomValue);
        void RequestRepaint ();
    }
}